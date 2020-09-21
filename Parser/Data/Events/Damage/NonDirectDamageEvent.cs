using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.Events.Damage
{
    public class NonDirectDamageEvent : AbstractDamageEvent
    {
        private int _isCondi = -1;

        internal NonDirectDamageEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Damage = evtcItem.BuffDmg;
            ArcDPSEnums.ConditionResult result = ArcDPSEnums.GetConditionResult(evtcItem.Result);

            IsAbsorbed = result == ArcDPSEnums.ConditionResult.InvulByBuff ||
                result == ArcDPSEnums.ConditionResult.InvulByPlayerSkill1 ||
                result == ArcDPSEnums.ConditionResult.InvulByPlayerSkill2 ||
                result == ArcDPSEnums.ConditionResult.InvulByPlayerSkill3;
            HasHit = result == ArcDPSEnums.ConditionResult.ExpectedToHit;
            ShieldDamage = evtcItem.IsShields > 0 ? Damage : 0;
        }

        public override bool IsCondi(ParsedLog log)
        {
            if (_isCondi == -1 && log.Buffs.BuffsByIds.TryGetValue(SkillId, out Buff b))
            {
                _isCondi = b.Nature == Buff.BuffNature.Condition ? 1 : 0;
            }
            return _isCondi == 1;
        }
    }
}
