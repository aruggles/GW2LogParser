using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Damage
{
    public class DirectBreakbarDamageEvent : AbstractBreakbarDamageEvent
    {
        internal DirectBreakbarDamageEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BreakbarDamage = evtcItem.Value / 10.0;
        }

        public override bool ConditionDamageBased(ParsedLog log)
        {
            return false;
        }
    }
}
