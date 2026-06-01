namespace GW2EIEvtcParser.ParsedData;

public class MapIDEvent : MetaDataEvent
{
    public readonly long Time;
    public readonly int MapID;
    public readonly int MapType;

    internal MapIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        MapID = GetMapID(evtcItem);
        MapType = (int)evtcItem.DstAgent;
        Time = evtcItem.Time;
    }

    internal static int GetMapID(CombatItem evtcItem)
    {
        return (int)evtcItem.SrcAgent;
    }

}
