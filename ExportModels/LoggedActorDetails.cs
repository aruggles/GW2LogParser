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
    public class LoggedActorDetails
    {
        public List<DamageDistribution> DmgDistributions { get; internal set; }
        public List<List<DamageDistribution>> DmgDistributionsTargets { get; internal set; }
        public List<DamageDistribution> DmgDistributionsTaken { get; internal set; }
        public List<List<object[]>> Rotation { get; internal set; }
        public List<List<BuffChartDataDto>> BoonGraph { get; internal set; }
        public List<Food> Food { get; internal set; }
        public List<LoggedActorDetails> Minions { get; internal set; }
        public List<LoggedDeathRecap> DeathRecap { get; internal set; }

        // helpers

        internal static LoggedActorDetails BuildPlayerData(ParsedLog log, Player player, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedActorDetails
            {
                DmgDistributions = new List<DamageDistribution>(),
                DmgDistributionsTargets = new List<List<DamageDistribution>>(),
                DmgDistributionsTaken = new List<DamageDistribution>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>(),
                Food = ExportModels.Food.BuildPlayerFoodData(log, player, usedBuffs),
                Minions = new List<LoggedActorDetails>(),
                DeathRecap = LoggedDeathRecap.BuildDeathRecap(log, player)
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                dto.Rotation.Add(LoggedSkill.BuildRotationData(log, player, i, usedSkills));
                dto.DmgDistributions.Add(DamageDistribution.BuildPlayerDMGDistData(log, player, null, i, usedSkills, usedBuffs));
                var dmgTargetsDto = new List<DamageDistribution>();
                foreach (NPC target in log.FightData.GetPhases(log)[i].Targets)
                {
                    dmgTargetsDto.Add(DamageDistribution.BuildPlayerDMGDistData(log, player, target, i, usedSkills, usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributionsTaken.Add(DamageDistribution.BuildDMGTakenDistData(log, player, i, usedSkills, usedBuffs));
                dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, player, i, usedBuffs));
            }
            foreach (KeyValuePair<long, Minions> pair in player.GetMinions(log))
            {
                dto.Minions.Add(BuildPlayerMinionsData(log, player, pair.Value, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static LoggedActorDetails BuildPlayerMinionsData(ParsedLog log, Player player, Minions minion, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedActorDetails
            {
                DmgDistributions = new List<DamageDistribution>(),
                DmgDistributionsTargets = new List<List<DamageDistribution>>()
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                var dmgTargetsDto = new List<DamageDistribution>();
                foreach (NPC target in log.FightData.GetPhases(log)[i].Targets)
                {
                    dmgTargetsDto.Add(DamageDistribution.BuildPlayerMinionDMGDistData(log, player, minion, target, i, usedSkills, usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributions.Add(DamageDistribution.BuildPlayerMinionDMGDistData(log, player, minion, null, i, usedSkills, usedBuffs));
            }
            return dto;
        }

        internal static LoggedActorDetails BuildTargetData(ParsedLog log, NPC target, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs, bool cr)
        {
            var dto = new LoggedActorDetails
            {
                DmgDistributions = new List<DamageDistribution>(),
                DmgDistributionsTaken = new List<DamageDistribution>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>()
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                if (log.FightData.GetPhases(log)[i].Targets.Contains(target))
                {
                    dto.DmgDistributions.Add(DamageDistribution.BuildTargetDMGDistData(log, target, i, usedSkills, usedBuffs));
                    dto.DmgDistributionsTaken.Add(DamageDistribution.BuildDMGTakenDistData(log, target, i, usedSkills, usedBuffs));
                    dto.Rotation.Add(LoggedSkill.BuildRotationData(log, target, i, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, target, i, usedBuffs));
                }
                // rotation + buff graph for CR
                else if (i == 0 && cr)
                {
                    dto.DmgDistributions.Add(new DamageDistribution());
                    dto.DmgDistributionsTaken.Add(new DamageDistribution());
                    dto.Rotation.Add(LoggedSkill.BuildRotationData(log, target, i, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, target, i, usedBuffs));
                }
                else
                {
                    dto.DmgDistributions.Add(new DamageDistribution());
                    dto.DmgDistributionsTaken.Add(new DamageDistribution());
                    dto.Rotation.Add(new List<object[]>());
                    dto.BoonGraph.Add(new List<BuffChartDataDto>());
                }
            }

            dto.Minions = new List<LoggedActorDetails>();
            foreach (KeyValuePair<long, Minions> pair in target.GetMinions(log))
            {
                dto.Minions.Add(BuildTargetsMinionsData(log, target, pair.Value, usedSkills, usedBuffs));
            }
            return dto;
        }

        private static LoggedActorDetails BuildTargetsMinionsData(ParsedLog log, NPC target, Minions minion, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedActorDetails
            {
                DmgDistributions = new List<DamageDistribution>()
            };
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                if (log.FightData.GetPhases(log)[i].Targets.Contains(target))
                {
                    dto.DmgDistributions.Add(DamageDistribution.BuildTargetMinionDMGDistData(log, target, minion, i, usedSkills, usedBuffs));
                }
                else
                {
                    dto.DmgDistributions.Add(new DamageDistribution());
                }
            }
            return dto;
        }
    }
}
