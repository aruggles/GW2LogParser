using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Extensions
{
    public class EXTDirectHealingEvent : EXTAbstractHealingEvent
    {
        internal EXTDirectHealingEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            HealingDone = -evtcItem.Value;
            AgainstDowned = ((evtcItem.IsOffcycle & ~SrcPeerMask) & ~DstPeerMask) == 1;
        }
    }
}
