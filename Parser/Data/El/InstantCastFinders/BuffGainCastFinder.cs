﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.InstantCastFinders
{
    internal class BuffGainCastFinder : BuffCastFinder
    {
        public delegate bool BuffGainCastChecker(BuffApplyEvent evt, CombatData combatData);
        private readonly BuffGainCastChecker _triggerCondition;

        protected virtual Agent GetCasterAgent(Agent agent)
        {
            return agent;
        }

        public BuffGainCastFinder(long skillID, long buffID, long icd, BuffGainCastChecker checker = null) : base(skillID, buffID, icd)
        {
            _triggerCondition = checker;
        }

        public BuffGainCastFinder(long skillID, long buffID, long icd, ulong minBuild, ulong maxBuild, BuffGainCastChecker checker = null) : base(skillID, buffID, icd, minBuild, maxBuild)
        {
            _triggerCondition = checker;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var applies = combatData.GetBuffData(BuffID).OfType<BuffApplyEvent>().GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
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
                            res.Add(new InstantCastEvent(bae.Time, skillData.Get(SkillID), GetCasterAgent(bae.To)));
                        }
                    }
                    else
                    {
                        lastTime = bae.Time;
                        res.Add(new InstantCastEvent(bae.Time, skillData.Get(SkillID), GetCasterAgent(bae.To)));
                    }
                }
            }

            return res;
        }
    }
}
