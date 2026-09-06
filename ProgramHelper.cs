using GW2EIBuilders;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using GW2EIParserCommons;
using GW2EIParserCommons.Exceptions;
using Gw2LogParser.EvtcParserExtensions;
using Gw2LogParser.ExportModels;
using Gw2LogParser.ExportModels.Report;
using System.Collections.Concurrent;
using System.Diagnostics;
using Tracing;

namespace Gw2LogParser;

public sealed class ProgramHelper : IDisposable
{
    public static IReadOnlyList<string> SupportedFormats => SupportedFileFormats.SupportedFormats;
    public static readonly string CacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/";
    public static readonly string SkillAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SkillList.json";
    public static readonly string SpecAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SpecList.json";
    public static readonly string TraitAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/TraitList.json";
    public static readonly string MapAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/MapList.json";
    public static readonly string EILogPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Logs/";
    public readonly Version ParserVersion;
    public static readonly GW2APIController apiController = new(SkillAPICacheLocation, SpecAPICacheLocation, TraitAPICacheLocation, MapAPICacheLocation);
    internal readonly static HTMLAssets htmlAssets = new();
    // Max input log file size (MB) the parser will attempt. Beyond this, parsing aborts with TooLongException.
    public const long DefaultTooBigLimitMB = 1500;
    public static bool MemoryCheck { get; set; } = false;
    public static bool EnableTracing { get; set; } = false;
    public static ConcurrentBag<LogContainer> CompletedLogs { get; set; } = [];
    public static long timestamp = DateTime.Now.ToFileTime();

    public ProgramHelper(Version parserVersion)
    {
        ParserVersion = parserVersion;
        SaveAPICacheIfMissing();
    }

    /// <summary>
    /// The static apiController downloads skills/specs/maps from the GW2 API when its cache files are absent,
    /// but nothing persisted them, so every launch repeated the ~10s download until the user pressed
    /// "Refresh API Cache". Write whatever was just loaded so the next launch reads from disk instead.
    /// </summary>
    private static void SaveAPICacheIfMissing()
    {
        static bool Missing(string path) => !File.Exists(path) || new FileInfo(path).Length == 0;
        if (!Missing(SkillAPICacheLocation) && !Missing(SpecAPICacheLocation) && !Missing(MapAPICacheLocation))
        {
            return;
        }
        try
        {
            Directory.CreateDirectory(CacheLocation);
            apiController.WriteCachedAPIToFile(SkillAPICacheLocation, SpecAPICacheLocation, MapAPICacheLocation);
        }
        catch (Exception)
        {
            // Read-only install location or similar: the in-memory cache still serves this session.
        }
    }

    private CancellationTokenSource? RunningMemoryCheck = null;

    public int GetMaxParallelRunning()
    {
        return 3;
    }

    public bool ParseMultipleLogs()
    {
        return true;
    }

    public void DoWork(OperationController operation)
    {
        System.Globalization.CultureInfo before = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo("en-US");
        operation.Reset();
        try
        {
            operation.Start();
            var fInfo = new FileInfo(operation.InputFile);
            var parser = new EvtcParser(new EvtcParserSettings(Properties.Settings.Default.CustomTooShort,
                                            DefaultTooBigLimitMB)
                                        {
                                            AnonymousPlayers = Properties.Settings.Default.Anonymous,
                                            SkipFailedTries = Properties.Settings.Default.SkipFailedTries,
                                            ComputePhases = Properties.Settings.Default.ParsePhases,
                                            ComputeCombatReplay = Properties.Settings.Default.ParseCombatReplay,
                                            ComputeDamageModifiers = Properties.Settings.Default.ComputeDamageModifiers,
                                            DetailedWvWParse = Properties.Settings.Default.DetailledWvW,
                                        }, apiController);

            //Process evtc here
            ParsedEvtcLog? log = parser.ParseLog(operation, fInfo, out var failureReason, true);
            if (failureReason != null)
            {
                failureReason.Throw();
            }
            if (log == null)
            {
                throw new ProgramException("Parsed log is null");
            }
            operation.BasicMetaData = new OperationController.OperationBasicMetaData(log);

            //Creating File
            string fightFile = GenerateFiles(log, operation, fInfo);
            // Build this fight's slice of the cross-fight summary here, on the worker thread,
            // so GenerateSummary only has to merge (and the ParsedEvtcLog can be collected).
            operation.UpdateProgressWithCancellationCheck("Program: Building summary data");
            LogReport logReport = BuildLogReport(log, fInfo, fightFile, ParserVersion);
            // A cancel requested while this parse was in flight must not leak the log into the
            // batch: Cancel clears CompletedLogs, and anything added afterwards would produce a
            // summary from a partial set.
            operation.UpdateProgressWithCancellationCheck("Program: Summary data ready");
            CompletedLogs.Add(new LogContainer(logReport, fInfo, log.LogMetadata.DateStart, log.LogMetadata.DateEnd));
        }
        catch (Exception ex)
        {
            throw new ProgramException(ex);
        }
        finally
        {
            operation.Stop();
            Thread.CurrentThread.CurrentCulture = before;
        }
    }

    /// <summary>Writes the per-fight HTML and returns its file name (fight_&lt;n&gt;.html).</summary>
    private string GenerateFiles(ParsedEvtcLog log, OperationController operation, FileInfo fInfo)
    {
        using var _t = new AutoTrace("Generate files");
        operation.UpdateProgressWithCancellationCheck("Program: Creating File(s)");

        DirectoryInfo saveDirectory = GetSaveDirectory(fInfo);
        var formOperation = operation as FormOperationController;
        var index = formOperation == null ? DateTime.Now.ToFileTime() : formOperation.Index;
        string result = log.LogData.GetMainPhase(log).Success ? "kill" : "fail";
        var uploadResults = new UploadResults();
        operation.OutLocation = saveDirectory.FullName;

        using var _t1 = new AutoTrace("Generate HTML");
        operation.UpdateProgressWithCancellationCheck("Program: Creating HTML");
        string outputFile = Path.Combine(saveDirectory.FullName, $"fight_{index}.html");
        operation.AddOpenableFile(outputFile);
        using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
        using (var sw = new StreamWriter(fs))
        {
            var builder = new HTMLBuilder(log,
            new HTMLSettings(
                Properties.Settings.Default.HtmlExternalScriptsPath,
                Properties.Settings.Default.HtmlExternalScriptsCdn)
            {
                HTMLLightTheme = Properties.Settings.Default.LightTheme,
                ExternalHTMLScripts = Properties.Settings.Default.HtmlExternalScripts,
                CompressJson = Properties.Settings.Default.HtmlCompressJson,
            },
            htmlAssets, ParserVersion, uploadResults);
            builder.CreateHTML(sw, saveDirectory.FullName);
        }
        operation.UpdateProgressWithCancellationCheck("Program: HTML created");
        operation.UpdateProgressWithCancellationCheck($"Completed for {result}ed {log.LogData.Logic.Extension}");
        return Path.GetFileName(outputFile);
    }

    /// <summary>
    /// Reduces one parsed log to the per-fight <see cref="LogReport"/> the summary merges.
    /// Runs on the parse worker thread; must not touch UI state.
    /// </summary>
    internal static LogReport BuildLogReport(ParsedEvtcLog log, FileInfo fInfo, string fightFile, Version parserVersion)
    {
        var logReport = new LogReport();
        var exporter = new LogBuilder(log);
        var data = exporter.BuildLogData(parserVersion, new UploadResults());
        logReport.Name = fInfo.Name;
        logReport.FileName = fightFile;
        logReport.LogsStart = log.LogData.LogStart;
        // Same quantity DurationString formats (fight end, trimmed at success), so the numeric
        // cell and its tooltip agree. EvtcLogEnd is the raw recording end and can differ.
        logReport.Duration = log.LogData.LogDuration;
        logReport.DurationString = log.LogData.DurationString;
        logReport.LogsEnd = log.LogData.LogEnd;
        logReport.PointOfView = data.RecordedBy;
        logReport.StartTime = log.LogMetadata.DateStart;
        logReport.MapName = data.Wvw ? data.LogName : "";
        logReport.Success = log.LogData.GetMainPhase(log).Success;
        exporter.UpdateLogReport(logReport, data);
        // WvW logs have no real victory condition — EI hardcodes Success = true (see
        // WvWLogic.CheckSuccess). Derive a meaningful outcome from the squad's kills vs deaths:
        // a win means we killed more enemies than we lost squad members.
        if (data.Wvw)
        {
            logReport.Success = logReport.EnemyDeaths > logReport.AlliesDead;
        }
        return logReport;
    }

    public static DirectoryInfo GetSaveDirectory(FileInfo fInfo)
    {
        //save location
        DirectoryInfo? saveDirectory = fInfo.Directory;
        if (saveDirectory == null)
        {
            throw new InvalidOperationException("FileInfo.Directory is null");
        }
        string savePath = Path.Combine(
                saveDirectory.FullName,
                $"combat_report_{ProgramHelper.timestamp}"
            );
        saveDirectory = Directory.CreateDirectory(savePath);

        if (saveDirectory == null || !saveDirectory.Exists)
        {
            throw new InvalidOperationException("Save directory does not exist");
        }
        return saveDirectory;
    }

    public static void GenerateSummary(Version parserVersion)
    {
        if (CompletedLogs.Count == 0)
        {
            return;
        }
        var report = new Report();

        // Snapshot: with AutoParse a new drop can start parsing while this runs.
        var sorted = CompletedLogs.ToList().OrderBy(c => c.DateEnd).ToList();
        if (sorted.Count == 0)
        {
            return;
        }
        LogContainer firstLog = sorted[0];
        LogContainer lastLog = sorted[sorted.Count - 1];

        report.LogsStart = firstLog.DateStart;
        report.LogsEnd = lastLog.DateEnd;
        DirectoryInfo saveDirectory = GetSaveDirectory(firstLog.evctFile);

        foreach (LogContainer parsedLog in sorted)
        {
            // Per-fight data was built on the parse worker (ProgramHelper.DoWork); this is a
            // pure merge so the UI isn't blocked re-deriving every log's stats.
            LogReport logReport = parsedLog.Report;
            report.Logs.Add(logReport);
            report.PointOfView = logReport.PointOfView;
            // Skill metadata is identical for the same ID across fights; first one wins.
            foreach (var kv in logReport.Skills)
            {
                report.Skills.TryAdd(kv.Key, kv.Value);
            }
            foreach (PlayerReport player in logReport.players.Values)
            {
                // Aggregate by account + character + class so each distinct combination is
                // tracked separately: an account swapping characters or elite specs across
                // fights produces one summary row per combination, not a single merged row.
                if (player.Name != null)
                {
                    if (report.players.TryGetValue(player.Key, out PlayerReport? playerValue))
                    {
                        LogBuilder.SumPlayerStats(playerValue, player);
                    }
                    else
                    {
                        // Copy: the per-fight report is retained in CompletedLogs and must
                        // not be mutated by later merges (the summary can be regenerated).
                        report.players[player.Key] = new PlayerReport(player);
                    }
                }
            }
        }
        if (saveDirectory != null)
        {
            string fileName = "index";
            string outputFile = Path.Combine(
                saveDirectory.FullName,
                $"{fileName}.html"
            );
            // Build HTML File.
            using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                var builder = new ExportModels.Report.HTMLReportBuilder(report);
                builder.CreatHTML(sw, saveDirectory.FullName);
            }
        }
    }

    public void GenerateTraceFile(OperationController operation)
    {
        if (EnableTracing)
        {
            var fInfo = new FileInfo(operation.InputFile);

            string fName = Path.GetFileNameWithoutExtension(fInfo.FullName);
            if (!fInfo.Exists)
            {
                fInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory);
            }

            DirectoryInfo saveDirectory = GetSaveDirectory(fInfo);

            string outputFile = Path.Combine(
            saveDirectory.FullName,
            $"{fName}.log"
            );
            operation.AddFile(outputFile);
            using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                operation.WriteLogMessages(sw);
            }
            operation.OutLocation = saveDirectory.FullName;
        }
    }

    public void ExecuteMemoryCheckTask()
    {
        if (RunningMemoryCheck != null)
        {
            RunningMemoryCheck.Cancel();
            RunningMemoryCheck.Dispose();
            RunningMemoryCheck = null;
        }

        if (!MemoryCheck || RunningMemoryCheck != null)
        {
            return;
        }

        RunningMemoryCheck = new CancellationTokenSource();// Prepare task
        Task.Run(async () =>
        {
            using var proc = Process.GetCurrentProcess();

            while (true)
            {
                await Task.Delay(500).ConfigureAwait(false);
                //NOTE(Rennorb): cannot wait for GC here because this is just a task (not a thread) and we would potentially be blocking other things from happening.
                proc.Refresh();
                if (proc.PrivateMemorySize64 > Math.Max(0, 100) * 1024L * 1024L)
                {
                    Environment.Exit(2);
                }
            }

        }, RunningMemoryCheck.Token);
    }

    public void Dispose()
    {
        if (RunningMemoryCheck != null)
        {
            RunningMemoryCheck.Cancel();
            RunningMemoryCheck.Dispose();
            RunningMemoryCheck = null;
        }
    }
}
