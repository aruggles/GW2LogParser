using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class TeamChangeEvent : AbstractStatusEvent
    {
        public ulong TeamID { get; }

        internal TeamChangeEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            TeamID = evtcItem.DstAgent;
        }

    }
}
