using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class BreakbarStateEvent : AbstractStatusEvent
    {
        public ArcDPSEnums.BreakbarState State { get; }

        internal BreakbarStateEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            State = ArcDPSEnums.GetBreakbarState(evtcItem.Value);
        }
    }
}
