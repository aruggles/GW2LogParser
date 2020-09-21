using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class SpawnEvent : AbstractStatusEvent
    {
        internal SpawnEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {

        }

    }
}
