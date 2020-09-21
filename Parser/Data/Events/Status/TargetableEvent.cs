using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class TargetableEvent : AbstractStatusEvent
    {
        public bool Targetable { get; }

        internal TargetableEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            Targetable = evtcItem.DstAgent == 1;
        }

    }
}
