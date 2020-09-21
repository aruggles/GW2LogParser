using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class EnterCombatEvent : AbstractStatusEvent
    {
        internal EnterCombatEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {

        }

    }
}
