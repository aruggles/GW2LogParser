using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public abstract class AbstractStatusEvent : AbstractTimeCombatEvent
    {
        public Agent Src { get; protected set; }

        protected AbstractStatusEvent(Combat evtcItem, AgentData agentData) : base(evtcItem.Time)
        {
            Src = agentData.GetAgent(evtcItem.SrcAgent);
        }

        protected AbstractStatusEvent(Agent src, long time) : base(time)
        {
            Src = src;
        }

    }
}
