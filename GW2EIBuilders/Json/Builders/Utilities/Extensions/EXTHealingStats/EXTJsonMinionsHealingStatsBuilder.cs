﻿using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class EXTJsonMinionsHealingStatsBuilder
    {
        public static EXTJsonMinionsHealingStats BuildMinionsHealingStats(Minions minions, ParsedLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var totalHealing = new List<int>();
            var totalAlliedHealing = new List<List<int>>();
            var alliedHealingDist = new List<List<List<EXTJsonHealingDist>>>();
            var totalHealingDist = new List<List<EXTJsonHealingDist>>();
            var res = new EXTJsonMinionsHealingStats()
            {
                TotalHealing = totalHealing,
                TotalAlliedHealing = totalAlliedHealing,
                AlliedHealingDist = alliedHealingDist,
                TotalHealingDist = totalHealingDist
            };
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (AbstractSingleActor friendly in log.Friendlies)
            {
                var totalAllyHealing = new List<int>();
                totalAlliedHealing.Add(totalAllyHealing);
                //
                var allyHealingDist = new List<List<EXTJsonHealingDist>>();
                alliedHealingDist.Add(allyHealingDist);
                foreach (PhaseData phase in phases)
                {
                    IReadOnlyList<GW2EIEvtcParser.Extensions.EXTAbstractHealingEvent> list = minions.EXTHealing.GetOutgoingHealEvents(friendly, log, phase.Start, phase.End);
                    totalAllyHealing.Add(list.Sum(x => x.HealingDone));
                    allyHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(list.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc));
                }
            }
            foreach (PhaseData phase in phases)
            {
                IReadOnlyList<GW2EIEvtcParser.Extensions.EXTAbstractHealingEvent> list = minions.EXTHealing.GetOutgoingHealEvents(null, log, phase.Start, phase.End);
                totalHealing.Add(list.Sum(x => x.HealingDone));
                totalHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(list.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc));
            }
            return res;
        }
    }
}
