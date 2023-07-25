﻿using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class NonDirectBreakbarDamageEvent : AbstractBreakbarDamageEvent
    {
        private int _isCondi = -1;
        internal NonDirectBreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BreakbarDamage = evtcItem.BuffDmg / 10.0;
        }

        public override bool ConditionDamageBased(ParsedEvtcLog log)
        {
            if (_isCondi == -1 && log.Buffs.BuffsByIds.TryGetValue(SkillId, out Buff b))
            {
                _isCondi = b.Classification == Buff.BuffClassification.Condition ? 1 : 0;
            }
            return _isCondi == 1;
        }
    }
}
