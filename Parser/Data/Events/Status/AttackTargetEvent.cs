using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class AttackTargetEvent : AbstractStatusEvent
    {
        public Agent AttackTarget { get; }

        internal AttackTargetEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            AttackTarget = Src;
            Src = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        }

    }
}
