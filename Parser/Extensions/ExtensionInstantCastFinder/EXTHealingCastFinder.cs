using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Extensions
{
    internal class EXTHealingCastFinder : InstantCastFinder
    {
        public delegate bool HealingCastChecker(EXTAbstractHealingEvent evt, CombatData combatData);
        private readonly HealingCastChecker _triggerCondition;

        private readonly long _damageSkillID;
        public EXTHealingCastFinder(long skillID, long damageSkillID, long icd, HealingCastChecker checker = null) : base(skillID, icd)
        {
            NotAccurate = true;
            _triggerCondition = checker;
            _damageSkillID = damageSkillID;
        }

        public EXTHealingCastFinder(long skillID, long damageSkillID, long icd, ulong minBuild, ulong maxBuild, HealingCastChecker checker = null) : base(skillID, icd, minBuild, maxBuild)
        {
            NotAccurate = true;
            _triggerCondition = checker;
            _damageSkillID = damageSkillID;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            if (!combatData.HasEXTHealing)
            {
                return res;
            }
            var heals = combatData.EXTHealingCombatData.GetHealData(_damageSkillID).GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<Agent, List<EXTAbstractHealingEvent>> pair in heals)
            {
                long lastTime = int.MinValue;
                if (!HealingStatsExtensionHandler.SanitizeForSrc(pair.Value))
                {
                    continue;
                }
                foreach (EXTAbstractHealingEvent de in pair.Value)
                {
                    if (de.Time - lastTime < ICD)
                    {
                        lastTime = de.Time;
                        continue;
                    }
                    if (_triggerCondition != null)
                    {
                        if (_triggerCondition(de, combatData))
                        {
                            lastTime = de.Time;
                            res.Add(new InstantCastEvent(de.Time, skillData.Get(SkillID), de.From));
                        }
                    }
                    else
                    {
                        lastTime = de.Time;
                        res.Add(new InstantCastEvent(de.Time, skillData.Get(SkillID), de.From));
                    }
                }
            }
            return res;
        }
    }
}
