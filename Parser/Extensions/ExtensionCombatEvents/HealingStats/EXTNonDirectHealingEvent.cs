using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Extensions
{
    class EXTNonDirectHealingEvent : EXTAbstractHealingEvent
    {
        internal EXTNonDirectHealingEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            HealingDone = -evtcItem.BuffDmg;
            AgainstDowned = evtcItem.Pad1 == 1;
        }
    }
}
