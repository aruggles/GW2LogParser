using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class DeadEvent : AbstractStatusEvent
    {
        internal DeadEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {

        }

        internal DeadEvent(Agent src, long time) : base(src, time)
        {

        }

    }
}
