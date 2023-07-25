﻿using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class DirectBreakbarDamageEvent : AbstractBreakbarDamageEvent
    {
        internal DirectBreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BreakbarDamage = Math.Round(evtcItem.Value / 10.0,1);
        }

        public override bool ConditionDamageBased(ParsedEvtcLog log)
        {
            return false;
        }
    }
}
