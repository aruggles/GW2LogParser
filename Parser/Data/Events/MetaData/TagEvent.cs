using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class TagEvent : AbstractMetaDataEvent
    {
        public int TagID { get; }
        public Agent Src { get; }

        internal TagEvent(Combat evtcItem, AgentData agentData) : base(evtcItem)
        {
            TagID = evtcItem.Value;
            Src = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
            Src.SetCommanderTag(this);
        }
    }
}
