using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Damage
{
    public class NonDirectBreakbarDamageEvent : AbstractBreakbarDamageEvent
    {
        private int _isCondi = -1;
        internal NonDirectBreakbarDamageEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BreakbarDamage = evtcItem.BuffDmg / 10.0;
        }

        public override bool ConditionDamageBased(ParsedLog log)
        {
            if (_isCondi == -1 && log.Buffs.BuffsByIds.TryGetValue(SkillId, out Buff b))
            {
                _isCondi = b.Nature == Buff.BuffNature.Condition ? 1 : 0;
            }
            return _isCondi == 1;
        }
    }
}
