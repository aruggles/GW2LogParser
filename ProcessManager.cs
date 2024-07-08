using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using Gw2LogParser.Exceptions;
using System.ComponentModel;
using Gw2LogParser.ExportModels;
using System.Collections.Concurrent;
using Gw2LogParser.ExportModels.Report;
using GW2EIEvtcParser;
using System.Windows.Forms;
using GW2EIGW2API;
using GW2EIEvtcParser.ParserHelpers;
using Gw2LogParser.EvtcParserExtensions;
using GW2EIBuilders.HtmlModels;
using GW2EIBuilders;

namespace Gw2LogParser
{
    public static class ProcessManager
    {
        internal static readonly string SkillAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SkillList.json";
        internal static readonly string SpecAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SpecList.json";
        internal static readonly string TraitAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/TraitList.json";
        internal static Version ParserVersion { get; } = new Version(Application.ProductVersion);

        internal static readonly GW2APIController apiController = new GW2APIController(SkillAPICacheLocation, SpecAPICacheLocation, TraitAPICacheLocation);
        public static ConcurrentBag<ParsedLog> CompletedLogs { get; set; } = new ConcurrentBag<ParsedLog>();
        /// <summary>
        /// Reports a status update for a log, updating the background worker and the related row with the new status
        /// </summary>
        /// <param name="bg"></param>
        /// <param name="row"></param>
        /// <param name="status"></param>
        /// <param name="percent"></param>
        public static void UpdateProgress(this BackgroundWorker bg, GridRow row, string status, int percent)
        {
            row.Status = status;
            bg.ReportProgress(percent, row);
            Console.WriteLine($"{row.Location}: {status}" + Environment.NewLine);
        }

        private static readonly HashSet<string> compressedFileTypes = new HashSet<string>()
        {
            ".zevtc",
            ".evtc.zip"
        };

        private static readonly HashSet<string> supportedFileTypes = new HashSet<string>(compressedFileTypes)
        {
            ".evtc"
        };

        public static bool IsCompressedLog(string fileName)
        {
            foreach (string fileType in compressedFileTypes)
            {
                if (fileName.EndsWith(fileType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSupportedLog(string fileName)
        {
            foreach (string fileType in supportedFileTypes)
            {
                if (fileName.EndsWith(fileType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static void ProcessRow(GridRow row, DataGridView dgv)
        {
            System.Globalization.CultureInfo before = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo("en-US");
            var fInfo = new FileInfo(row.Location);
            var operation = new FormOperationController(fInfo.FullName, "Ready to parse", dgv);
            try
            {
                if (!fInfo.Exists)
                {
                    throw new FileNotFoundException("File " + fInfo.FullName + " does not exist");
                }
                var parser = new EvtcParser(new EvtcParserSettings(Properties.Settings.Default.Anonymous,
                                                Properties.Settings.Default.SkipFailedTries,
                                                Properties.Settings.Default.ParsePhases,
                                                Properties.Settings.Default.ParseCombatReplay,
                                                Properties.Settings.Default.ComputeDamageModifiers,
                                                Properties.Settings.Default.CustomTooShort,
                                                Properties.Settings.Default.DetailledWvW), apiController);
                //Process evtc here
                ParsedEvtcLog parsedLog = parser.ParseLog(operation, fInfo, out ParsingFailureReason failureReason);
                if (failureReason != null)
                {
                    failureReason.Throw();
                }
                ParsedLog log = new ParsedLog(parsedLog);
                log.evctFile = fInfo;
                /*
                string fName = fInfo.Name.Split('.')[0];
                string result = log.FightData.Success ? "kill" : "fail";
                string encounterLengthTerm = Properties.Settings.Default.AddDuration ? "_" + (log.FightData.FightEnd / 1000).ToString() + "s" : "";
                string PoVClassTerm = Properties.Settings.Default.AddPoVProf ? "_" + log.LogData.PoV.Prof.ToLower() : "";
                var htmlAssets = new GW2EIBuilders.HTMLAssets();
                fName = $"{fName}{PoVClassTerm}_{log.FightData.Logic.Extension}{encounterLengthTerm}_{result}";
                string outputFile = Path.Combine(
                    fInfo.FullName,
                    $"{fName}.html"
                );
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    var builder = new GW2EIBuilders.HTMLBuilder(log, new GW2EIBuilders.HTMLSettings(Properties.Settings.Default.LightTheme, Properties.Settings.Default.HtmlExternalScripts), htmlAssets);
                    builder.CreateHTML(sw, GetSaveDirectory(fInfo).FullName);
                }
                */
                CompletedLogs.Add(log);
                //var exporter = new LogBuilder(log);
                //var data = exporter.buildLogData();
                //Console.WriteLine("Parsed Log File");
                //string[] uploadresult = UploadController.UploadOperation(row, fInfo);
                //Creating File
                //GenerateFiles(log, row, uploadresult, fInfo);
                row.BgWorker.UpdateProgress(row, "Parsed Log", 100);
            }
            catch (InvalidDataException invEx)
            {
                row.BgWorker.UpdateProgress(row, "Not EVTC", 100);
                throw new CancelledException(row, invEx);
            }
            catch (Exception ex)
            {
                throw new CancelledException(row, ex);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = before;
            }
        }

        private static DirectoryInfo GetSaveDirectory(FileInfo fInfo)
        {
            //save location
            DirectoryInfo saveDirectory;
            if (Properties.Settings.Default.SaveAtOut || Properties.Settings.Default.OutLocation == null)
            {
                //Default save directory
                saveDirectory = fInfo.Directory;
                if (!saveDirectory.Exists)
                {
                    throw new InvalidOperationException("Save directory does not exist");
                }
            }
            else
            {
                if (!Directory.Exists(Properties.Settings.Default.OutLocation))
                {
                    throw new InvalidOperationException("Save directory does not exist");
                }
                saveDirectory = new DirectoryInfo(Properties.Settings.Default.OutLocation);
            }
            return saveDirectory;
        }

        /// <summary>
        /// Throws a <see cref="CancellationException"/> if the background worker has been cancelled
        /// </summary>
        /// <param name="bg"></param>
        /// <param name="row"></param>
        /// <param name="cancelStatus"></param>
        public static void ThrowIfCanceled(this BackgroundWorker bg, GridRow row, string cancelStatus = "Canceled")
        {
            if (bg.CancellationPending)
            {
                row.Status = cancelStatus;
                throw new CancelledException(row);

            }
        }

        private static void WriteFightHTML(DirectoryInfo saveFolder, int count, ParsedLog log, LogDataDto logData)
        {
            if (saveFolder == null)
            {
                return;
            }
            var uploadResults = new UploadResults("", "");
            var htmlAssets = new HTMLAssets();
            string outputFile = Path.Combine(
                saveFolder.FullName,
                $"fight_{count}.html"
            );
            using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                var builder = new HTMLBuilder(log,
                    new HTMLSettings(
                        Properties.Settings.Default.LightTheme,
                        Properties.Settings.Default.HtmlExternalScripts,
                        Properties.Settings.Default.HtmlExternalScriptsPath,
                        Properties.Settings.Default.HtmlExternalScriptsCdn,
                        Properties.Settings.Default.HtmlCompressJson),
                    htmlAssets, ParserVersion, uploadResults);
                builder.CreateHTML(sw, saveFolder.FullName);
            }
        }

        public static void GenerateFile(BackgroundWorker worker)
        {
            var report = new Report();
            var uploadResults = new UploadResults("", "");
            var count = 0;
            var total = CompletedLogs.Count;
            FileInfo fileInfo = null;
            DirectoryInfo saveFolder = null;
            var sorted = CompletedLogs.OrderBy(c => c.LogData.LogEndRaw);
            var _firstLog = sorted.First<ParsedLog>();
            var _lastLog = sorted.Last<ParsedLog>();
            if (_firstLog != null)
            {
                fileInfo = _firstLog.evctFile;
                string savePath = Path.Combine(
                    fileInfo.Directory.FullName,
                    $"combat_report{DateTime.Now.ToFileTime()}"
                );
                saveFolder = Directory.CreateDirectory(savePath);
                report.LogsStart = _firstLog.LogData.LogStart;
            }
            if (_lastLog != null)
            {
                report.LogsEnd = _lastLog.LogData.LogEnd;
            }
            
            foreach (ParsedLog parsedLog in sorted)
            {
                var logReport = new LogReport();
                var exporter = new LogBuilder(parsedLog);
                var data = exporter.BuildLogData(ParserVersion, uploadResults);
                logReport.Name = parsedLog.evctFile.Name;
                fileInfo = parsedLog.evctFile;
                logReport.LogsStart = parsedLog.LogData.LogStart;
                logReport.lengthInSeconds = parsedLog.LogData.LogEndRaw - parsedLog.LogData.LogStartRaw;
                logReport.LogsEnd = parsedLog.LogData.LogEnd;
                logReport.PointOfView = data.RecordedBy;
                exporter.UpdateLogReport(logReport, data);
                
                report.Logs.Add(logReport);
                report.PointOfView = data.RecordedBy;
                foreach (PlayerReport player in logReport.players.Values)
                {
                    PlayerReport playerValue = null;
                    report.players.TryGetValue(player.Name, out playerValue);
                    if (playerValue == null)
                    {
                        report.players[player.Name] = player;
                    }
                    else
                    {
                        exporter.SumPlayerStats(playerValue, player);
                    }
                }
                WriteFightHTML(saveFolder, count, parsedLog, data);
                // Update Progress.
                if (count > 0)
                {
                    var percent = (int)(((float)count / (float)total) * 100);
                    worker.ReportProgress(percent);
                }
                count += 1;
            }
            if (saveFolder != null)
            {
                string fileName = "index";
                string outputFile = Path.Combine(
                    saveFolder.FullName,
                    $"{fileName}.html"
                );
                // Build HTML File.
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    var builder = new ExportModels.Report.HTMLReportBuilder(report);
                    builder.CreatHTML(sw, saveFolder.FullName);
                }
            }

            worker.ReportProgress(100);
        }
    }
}
