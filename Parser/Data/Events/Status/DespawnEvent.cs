using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class DespawnEvent : AbstractStatusEvent
    {
        internal DespawnEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {

        }

    }
}
