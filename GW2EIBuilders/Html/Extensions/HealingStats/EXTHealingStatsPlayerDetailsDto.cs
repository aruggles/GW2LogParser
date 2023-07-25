﻿using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class EXTHealingStatsPlayerDetailsDto
    {
        public List<EXTHealingStatsHealingDistributionDto> healingDistributions { get; set; }
        public List<List<EXTHealingStatsHealingDistributionDto>> healingDistributionsTargets { get; set; }
        public List<EXTHealingStatsHealingDistributionDto> IncomingHealingDistributions { get; set; }
        public List<EXTHealingStatsPlayerDetailsDto> Minions { get; set; }

        // helpers

        public static EXTHealingStatsPlayerDetailsDto BuildPlayerHealingData(ParsedLog log, AbstractSingleActor actor, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTHealingStatsPlayerDetailsDto
            {
                healingDistributions = new List<EXTHealingStatsHealingDistributionDto>(),
                healingDistributionsTargets = new List<List<EXTHealingStatsHealingDistributionDto>>(),
                IncomingHealingDistributions = new List<EXTHealingStatsHealingDistributionDto>(),
                Minions = new List<EXTHealingStatsPlayerDetailsDto>(),
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                dto.healingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyHealingDistData(log, actor, null, phase, usedSkills, usedBuffs));
                var dmgTargetsDto = new List<EXTHealingStatsHealingDistributionDto>();
                foreach (AbstractSingleActor target in log.Friendlies)
                {
                    dmgTargetsDto.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyHealingDistData(log, actor, target, phase, usedSkills, usedBuffs));
                }
                dto.healingDistributionsTargets.Add(dmgTargetsDto);
                dto.IncomingHealingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildIncomingHealingDistData(log, actor, phase, usedSkills, usedBuffs));
            }
            foreach (KeyValuePair<long, Minions> pair in actor.GetMinions(log))
            {
                dto.Minions.Add(BuildFriendlyMinionsHealingData(log, actor, pair.Value, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static EXTHealingStatsPlayerDetailsDto BuildFriendlyMinionsHealingData(ParsedLog log, AbstractSingleActor actor, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new EXTHealingStatsPlayerDetailsDto
            {
                healingDistributions = new List<EXTHealingStatsHealingDistributionDto>(),
                healingDistributionsTargets = new List<List<EXTHealingStatsHealingDistributionDto>>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var dmgTargetsDto = new List<EXTHealingStatsHealingDistributionDto>();
                foreach (AbstractSingleActor target in log.Friendlies)
                {
                    dmgTargetsDto.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyMinionHealingDistData(log, actor, minion, target, phase, usedSkills, usedBuffs));
                }
                dto.healingDistributionsTargets.Add(dmgTargetsDto);
                dto.healingDistributions.Add(EXTHealingStatsHealingDistributionDto.BuildFriendlyMinionHealingDistData(log, actor, minion, null, phase, usedSkills, usedBuffs));
            }
            return dto;
        }
    }
}
