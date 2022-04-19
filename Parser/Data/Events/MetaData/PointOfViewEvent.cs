using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class PointOfViewEvent : AbstractMetaDataEvent
    {
        public Agent PoV { get; }

        public PointOfViewEvent(Combat evtcItem, AgentData agentData) : base(evtcItem)
        {
            PoV = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        }

    }
}
