
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class SkillTiming
    {
        public byte ActionAction { get; }

        public ArcDPSEnums.SkillAction Action { get; }

        public ulong AtMillisecond { get; }

        internal SkillTiming(Combat evtcItem)
        {
            ActionAction = (byte)evtcItem.SrcAgent;
            Action = ArcDPSEnums.GetSkillAction(ActionAction);
            AtMillisecond = evtcItem.DstAgent;
        }
    }
}
