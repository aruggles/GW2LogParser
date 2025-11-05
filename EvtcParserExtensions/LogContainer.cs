using GW2EIEvtcParser;

namespace Gw2LogParser.EvtcParserExtensions
{
    public class LogContainer
    {
        public ParsedEvtcLog Log { get; set; }
        public FileInfo evctFile { get; set; }
        public LogContainer(ParsedEvtcLog log, FileInfo evctFile)
        {
            Log = log;
            this.evctFile = evctFile;
        }
    }
}
