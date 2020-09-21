
namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class SkillTiming
    {
        public ulong Action { get; }

        public ulong AtMillisecond { get; }

        internal SkillTiming(Combat evtcItem)
        {
            Action = evtcItem.SrcAgent;
            AtMillisecond = evtcItem.DstAgent;
        }
    }
}
