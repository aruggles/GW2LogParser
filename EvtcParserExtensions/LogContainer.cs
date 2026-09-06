using Gw2LogParser.ExportModels.Report;

namespace Gw2LogParser.EvtcParserExtensions
{
    /// <summary>
    /// One completed parse, reduced to what the cross-fight summary needs. The per-fight
    /// <see cref="LogReport"/> is built on the parse worker thread right after the fight HTML,
    /// so the summary step only merges and the (large) ParsedEvtcLog can be collected.
    /// </summary>
    public class LogContainer
    {
        internal LogReport Report { get; }
        public FileInfo evctFile { get; }
        /// <summary>Wall-clock start (LogMetadata.DateStart), used for ordering fights.</summary>
        public string DateStart { get; }
        /// <summary>Wall-clock end (LogMetadata.DateEnd), used for ordering fights.</summary>
        public string DateEnd { get; }

        internal LogContainer(LogReport report, FileInfo evctFile, string dateStart, string dateEnd)
        {
            Report = report;
            this.evctFile = evctFile;
            DateStart = dateStart;
            DateEnd = dateEnd;
        }
    }
}
