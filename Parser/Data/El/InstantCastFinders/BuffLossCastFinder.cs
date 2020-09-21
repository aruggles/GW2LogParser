using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.InstantCastFinders
{
    internal class BuffLossCastFinder : BuffCastFinder
    {
        public delegate bool BuffLossCastChecker(BuffRemoveAllEvent evt, CombatData combatData);
        private readonly BuffLossCastChecker _triggerCondition;


        public BuffLossCastFinder(long skillID, long buffID, long icd, BuffLossCastChecker checker = null) : base(skillID, buffID, icd)
        {
            _triggerCondition = checker;
        }

        public BuffLossCastFinder(long skillID, long buffID, long icd, ulong minBuild, ulong maxBuild, BuffLossCastChecker checker = null) : base(skillID, buffID, icd, minBuild, maxBuild)
        {
            _triggerCondition = checker;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var removals = combatData.GetBuffData(BuffID).OfType<BuffRemoveAllEvent>().GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<Agent, List<BuffRemoveAllEvent>> pair in removals)
            {
                long lastTime = int.MinValue;
                foreach (BuffRemoveAllEvent brae in pair.Value)
                {
                    if (brae.Time - lastTime < ICD)
                    {
                        lastTime = brae.Time;
                        continue;
                    }
                    if (_triggerCondition != null)
                    {
                        if (_triggerCondition(brae, combatData))
                        {
                            lastTime = brae.Time;
                            res.Add(new InstantCastEvent(brae.Time, skillData.Get(SkillID), brae.To));
                        }
                    }
                    else
                    {
                        lastTime = brae.Time;
                        res.Add(new InstantCastEvent(brae.Time, skillData.Get(SkillID), brae.To));
                    }
                }
            }
            return res;
        }
    }
}
