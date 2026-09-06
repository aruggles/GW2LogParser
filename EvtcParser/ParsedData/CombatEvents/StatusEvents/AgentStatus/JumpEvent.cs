namespace GW2EIEvtcParser.ParsedData;

public class JumpEvent : StatusEvent
{
    public readonly bool OnLand;
    private readonly uint SomethingBehaviorRelated;
    internal JumpEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        OnLand = evtcItem.DstAgent == 0;
        SomethingBehaviorRelated = evtcItem.OverstackValue;
    }

}
