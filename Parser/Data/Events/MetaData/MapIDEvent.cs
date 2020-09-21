
namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class MapIDEvent : AbstractMetaDataEvent
    {
        public int MapID { get; }

        public MapIDEvent(Combat evtcItem) : base(evtcItem)
        {
            MapID = (int)evtcItem.SrcAgent;
        }

    }
}
