﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.InstantCastFinders
{
    internal class BuffGiveCastFinder : BuffCastFinder
    {
        public delegate bool BuffGiveCastChecker(BuffApplyEvent evt, CombatData combatData);
        private readonly BuffGiveCastChecker _triggerCondition;
        public BuffGiveCastFinder(long skillID, long buffID, long icd, BuffGiveCastChecker checker = null) : base(skillID, buffID, icd)
        {
            _triggerCondition = checker;
        }

        public BuffGiveCastFinder(long skillID, long buffID, long icd, ulong minBuild, ulong maxBuild, BuffGiveCastChecker checker = null) : base(skillID, buffID, icd, minBuild, maxBuild)
        {
            _triggerCondition = checker;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var applies = combatData.GetBuffData(BuffID).OfType<BuffApplyEvent>().GroupBy(x => x.CreditedBy).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<Agent, List<BuffApplyEvent>> pair in applies)
            {
                long lastTime = int.MinValue;
                foreach (BuffApplyEvent bae in pair.Value)
                {
                    if (bae.Initial)
                    {
                        continue;
                    }
                    if (bae.Time - lastTime < ICD)
                    {
                        lastTime = bae.Time;
                        continue;
                    }
                    if (_triggerCondition != null)
                    {
                        if (_triggerCondition(bae, combatData))
                        {
                            lastTime = bae.Time;
                            res.Add(new InstantCastEvent(bae.Time, skillData.Get(SkillID), bae.CreditedBy));
                        }
                    }
                    else
                    {
                        lastTime = bae.Time;
                        res.Add(new InstantCastEvent(bae.Time, skillData.Get(SkillID), bae.CreditedBy));
                    }
                }
            }

            return res;
        }
    }
}
