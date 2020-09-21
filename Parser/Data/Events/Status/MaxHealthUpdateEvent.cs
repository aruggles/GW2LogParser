using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class MaxHealthUpdateEvent : AbstractStatusEvent
    {
        public int MaxHealth { get; }

        internal MaxHealthUpdateEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            MaxHealth = (int)evtcItem.DstAgent;
        }

    }
}
