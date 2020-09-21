using Gw2LogParser.GW2EIBuilders;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.ExportModels
{
    public class LoggedPlayerDetails
    {
        public List<DamageDistribution> DamageDistribution { get; internal set; }
        public List<List<DamageDistribution>> DamageDistributionTargets { get; set; }
        public List<DamageDistribution> DamageDistributionTaken { get; set; }
        public List<Food> Foods { get; internal set; }
        public List<List<object[]>> Rotation { get; internal set; }
        public List<List<BuffChartDataDto>> BoonGraph { get; internal set; }
        public List<LoggedPlayerDetails> Minions { get; internal set; }
        public List<DeathRecapDto> DeathRecap { get; internal set; }

        // helpers

        internal static LoggedPlayerDetails BuildPlayerData(ParsedLog log, Player player, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedPlayerDetails
            {
                DamageDistribution = new List<DamageDistribution>(),
                DamageDistributionTargets = new List<List<DamageDistribution>>(),
                DamageDistributionTaken = new List<DamageDistribution>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>(),
                Foods = Food.BuildPlayerFoodData(log, player, usedBuffs),
                Minions = new List<LoggedPlayerDetails>(),
                DeathRecap = DeathRecapDto.BuildDeathRecap(log, player)
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                dto.Rotation.Add(SkillDto.BuildRotationData(log, player, i, usedSkills));
                
                dto.DamageDistribution.Add(ExportModels.DamageDistribution.BuildPlayerDMGDistData(log, player, null, i, usedSkills, usedBuffs));
                var dmgTargetsDto = new List<DamageDistribution>();
                foreach (NPC target in log.FightData.GetPhases(log)[i].Targets)
                {
                    dmgTargetsDto.Add(ExportModels.DamageDistribution.BuildPlayerDMGDistData(log, player, target, i, usedSkills, usedBuffs));
                }
                dto.DamageDistributionTargets.Add(dmgTargetsDto);
                dto.DamageDistributionTaken.Add(ExportModels.DamageDistribution.BuildDMGTakenDistData(log, player, i, usedSkills, usedBuffs));
                dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, player, i, usedBuffs));
            }
            foreach (KeyValuePair<long, Minions> pair in player.GetMinions(log))
            {
                dto.Minions.Add(BuildPlayerMinionsData(log, player, pair.Value, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static LoggedPlayerDetails BuildPlayerMinionsData(ParsedLog log, Player player, Minions minion, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedPlayerDetails
            {
                DamageDistribution = new List<DamageDistribution>(),
                DamageDistributionTargets = new List<List<DamageDistribution>>()
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                var dmgTargetsDto = new List<DamageDistribution>();
                foreach (NPC target in log.FightData.GetPhases(log)[i].Targets)
                {
                    dmgTargetsDto.Add(ExportModels.DamageDistribution.BuildPlayerMinionDMGDistData(log, player, minion, target, i, usedSkills, usedBuffs));
                }
                dto.DamageDistributionTargets.Add(dmgTargetsDto);
                dto.DamageDistribution.Add(ExportModels.DamageDistribution.BuildPlayerMinionDMGDistData(log, player, minion, null, i, usedSkills, usedBuffs));
            }
            return dto;
        }

        internal static LoggedPlayerDetails BuildTargetData(ParsedLog log, NPC target, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs, bool cr)
        {
            var dto = new LoggedPlayerDetails
            {
                DamageDistribution = new List<DamageDistribution>(),
                DamageDistributionTaken = new List<DamageDistribution>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>()
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                if (log.FightData.GetPhases(log)[i].Targets.Contains(target))
                {
                    dto.DamageDistribution.Add(ExportModels.DamageDistribution.BuildTargetDMGDistData(log, target, i, usedSkills, usedBuffs));
                    dto.DamageDistributionTaken.Add(ExportModels.DamageDistribution.BuildDMGTakenDistData(log, target, i, usedSkills, usedBuffs));
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, i, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, target, i, usedBuffs));
                }
                // rotation + buff graph for CR
                else if (i == 0 && cr)
                {
                    dto.DamageDistribution.Add(new DamageDistribution());
                    dto.DamageDistributionTaken.Add(new DamageDistribution());
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, i, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, target, i, usedBuffs));
                }
                else
                {
                    dto.DamageDistribution.Add(new DamageDistribution());
                    dto.DamageDistributionTaken.Add(new DamageDistribution());
                    dto.Rotation.Add(new List<object[]>());
                    dto.BoonGraph.Add(new List<BuffChartDataDto>());
                }
            }

            dto.Minions = new List<LoggedPlayerDetails>();
            foreach (KeyValuePair<long, Minions> pair in target.GetMinions(log))
            {
                dto.Minions.Add(BuildTargetsMinionsData(log, target, pair.Value, usedSkills, usedBuffs));
            }
            return dto;
        }

        private static LoggedPlayerDetails BuildTargetsMinionsData(ParsedLog log, NPC target, Minions minion, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedPlayerDetails
            {
                DamageDistribution = new List<DamageDistribution>()
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                if (log.FightData.GetPhases(log)[i].Targets.Contains(target))
                {
                    dto.DamageDistribution.Add(ExportModels.DamageDistribution.BuildTargetMinionDMGDistData(log, target, minion, i, usedSkills, usedBuffs));
                }
                else
                {
                    dto.DamageDistribution.Add(new DamageDistribution());
                }
            }
            return dto;
        }
    }
}
