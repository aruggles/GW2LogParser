﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    public class ActorDetailsDto
    {
        public List<DmgDistributionDto> DmgDistributions { get; internal set; }
        public List<List<DmgDistributionDto>> DmgDistributionsTargets { get; internal set; }
        public List<DmgDistributionDto> DmgDistributionsTaken { get; internal set; }
        public List<List<object[]>> Rotation { get; internal set; }
        public List<List<BuffChartDataDto>> BoonGraph { get; internal set; }
        public List<FoodDto> Food { get; internal set; }
        public List<ActorDetailsDto> Minions { get; internal set; }
        public List<DeathRecapDto> DeathRecap { get; internal set; }

        // helpers

        internal static ActorDetailsDto BuildPlayerData(ParsedLog log, Player player, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>(),
                DmgDistributionsTaken = new List<DmgDistributionDto>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>(),
                Food = FoodDto.BuildPlayerFoodData(log, player, usedBuffs),
                Minions = new List<ActorDetailsDto>(),
                DeathRecap = DeathRecapDto.BuildDeathRecap(log, player)
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                dto.Rotation.Add(SkillDto.BuildRotationData(log, player, i, usedSkills));
                dto.DmgDistributions.Add(DmgDistributionDto.BuildPlayerDMGDistData(log, player, null, i, usedSkills, usedBuffs));
                var dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (NPC target in log.FightData.GetPhases(log)[i].Targets)
                {
                    dmgTargetsDto.Add(DmgDistributionDto.BuildPlayerDMGDistData(log, player, target, i, usedSkills, usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributionsTaken.Add(DmgDistributionDto.BuildDMGTakenDistData(log, player, i, usedSkills, usedBuffs));
                dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, player, i, usedBuffs));
            }
            foreach (KeyValuePair<long, Minions> pair in player.GetMinions(log))
            {
                dto.Minions.Add(BuildPlayerMinionsData(log, player, pair.Value, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static ActorDetailsDto BuildPlayerMinionsData(ParsedLog log, Player player, Minions minion, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>()
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                var dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (NPC target in log.FightData.GetPhases(log)[i].Targets)
                {
                    dmgTargetsDto.Add(DmgDistributionDto.BuildPlayerMinionDMGDistData(log, player, minion, target, i, usedSkills, usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributions.Add(DmgDistributionDto.BuildPlayerMinionDMGDistData(log, player, minion, null, i, usedSkills, usedBuffs));
            }
            return dto;
        }

        internal static ActorDetailsDto BuildTargetData(ParsedLog log, NPC target, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs, bool cr)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTaken = new List<DmgDistributionDto>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>()
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                if (log.FightData.GetPhases(log)[i].Targets.Contains(target))
                {
                    dto.DmgDistributions.Add(DmgDistributionDto.BuildTargetDMGDistData(log, target, i, usedSkills, usedBuffs));
                    dto.DmgDistributionsTaken.Add(DmgDistributionDto.BuildDMGTakenDistData(log, target, i, usedSkills, usedBuffs));
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, i, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, target, i, usedBuffs));
                }
                // rotation + buff graph for CR
                else if ( i == 0 && cr)
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                    dto.DmgDistributionsTaken.Add(new DmgDistributionDto());
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, i, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, target, i, usedBuffs));
                }
                else
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                    dto.DmgDistributionsTaken.Add(new DmgDistributionDto());
                    dto.Rotation.Add(new List<object[]>());
                    dto.BoonGraph.Add(new List<BuffChartDataDto>());
                }
            }

            dto.Minions = new List<ActorDetailsDto>();
            foreach (KeyValuePair<long, Minions> pair in target.GetMinions(log))
            {
                dto.Minions.Add(BuildTargetsMinionsData(log, target, pair.Value, usedSkills, usedBuffs));
            }
            return dto;
        }

        private static ActorDetailsDto BuildTargetsMinionsData(ParsedLog log, NPC target, Minions minion, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>()
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                if (log.FightData.GetPhases(log)[i].Targets.Contains(target))
                {
                    dto.DmgDistributions.Add(DmgDistributionDto.BuildTargetMinionDMGDistData(log, target, minion, i, usedSkills, usedBuffs));
                }
                else
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                }
            }
            return dto;
        }
    }
}
