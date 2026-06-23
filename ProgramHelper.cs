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
            GenerateFiles(log, operation, fInfo);
            LogContainer logContainer = new LogContainer(log, fInfo);
            CompletedLogs.Add(logContainer);
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

    private void GenerateFiles(ParsedEvtcLog log, OperationController operation, FileInfo fInfo)
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
        var uploadResults = new UploadResults();
        
        FileInfo? fileInfo = null;
        DirectoryInfo? saveDirectory = null;

        var sorted = CompletedLogs.OrderBy(c => c.Log.LogMetadata.DateEnd);
        var _firstLog = sorted.First<LogContainer>();
        var _lastLog = sorted.Last<LogContainer>();

        if (_firstLog != null)
        {
            report.LogsStart = _firstLog.Log.LogMetadata.DateStart;
            fileInfo = _firstLog.evctFile;
            saveDirectory = GetSaveDirectory(fileInfo);
        }
        if (_lastLog != null)
        {
            report.LogsEnd = _lastLog.Log.LogMetadata.DateEnd;
        }

        foreach (LogContainer parsedLog in sorted)
        {
            var logReport = new LogReport();
            var exporter = new LogBuilder(parsedLog);
            var data = exporter.BuildLogData(parserVersion, uploadResults);
            logReport.Name = parsedLog.evctFile.Name;
            fileInfo = parsedLog.evctFile;
            logReport.LogsStart = parsedLog.Log.LogData.LogStart;
            logReport.Duration = parsedLog.Log.LogData.EvtcLogEnd;
            logReport.DurationString = parsedLog.Log.LogData.DurationString;
            logReport.LogsEnd = parsedLog.Log.LogData.LogEnd;
            logReport.PointOfView = data.RecordedBy;
            logReport.StartTime = parsedLog.Log.LogMetadata.DateStart;
            logReport.MapName = data.Wvw ? data.LogName : "";
            logReport.Success = parsedLog.Log.LogData.GetMainPhase(parsedLog.Log).Success;
            exporter.UpdateLogReport(logReport, data);
            // WvW logs have no real victory condition — EI hardcodes Success = true (see
            // WvWLogic.CheckSuccess). Derive a meaningful outcome from the squad's kills vs deaths:
            // a win means we killed more enemies than we lost squad members.
            if (data.Wvw)
            {
                logReport.Success = logReport.EnemyDeaths > logReport.AlliesDead;
            }

            report.Logs.Add(logReport);
            report.PointOfView = data.RecordedBy;
            foreach (PlayerReport player in logReport.players.Values)
            {
                // Aggregate by account + character + class so each distinct combination is
                // tracked separately: an account swapping characters or elite specs across
                // fights produces one summary row per combination, not a single merged row.
                if (player.Name != null)
                {
                    if (report.players.TryGetValue(player.Key, out PlayerReport? playerValue))
                    {
                        exporter.SumPlayerStats(playerValue, player);
                    }
                    else
                    {
                        report.players[player.Key] = player;
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
