using Gw2LogParser.GW2EIBuilders;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
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
    internal class LoggedPlayerDetails
    {
        public List<DamageDistribution> DamageDistribution { get; set; }
        public List<List<DamageDistribution>> DamageDistributionTargets { get; set; }
        public List<DamageDistribution> DamageDistributionTaken { get; set; }
        public List<Food> Foods { get; set; }
        public List<List<object[]>> Rotation { get; set; }
        public List<List<BuffChartDataDto>> BoonGraph { get; set; }
        public List<LoggedPlayerDetails> Minions { get; set; }
        public List<DeathRecapDto> DeathRecap { get; set; }

        // helpers

        public static LoggedPlayerDetails BuildPlayerData(ParsedLog log, AbstractSingleActor actor, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedPlayerDetails
            {
                DamageDistribution = new List<DamageDistribution>(),
                DamageDistributionTargets = new List<List<DamageDistribution>>(),
                DamageDistributionTaken = new List<DamageDistribution>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>(),
                Foods = Food.BuildFoodData(log, actor, usedBuffs),
                Minions = new List<LoggedPlayerDetails>(),
                DeathRecap = DeathRecapDto.BuildDeathRecap(log, actor)
            };
            
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                
                dto.Rotation.Add(SkillDto.BuildRotationData(log, actor, phase, usedSkills));
                dto.DamageDistribution.Add(ExportModels.DamageDistribution.BuildFriendlyDMGDistData(log, actor, null, phase, usedSkills, usedBuffs));
                var dmgTargetsDto = new List<DamageDistribution>();
                foreach (AbstractSingleActor target in phase.Targets)
                {
                    dmgTargetsDto.Add(ExportModels.DamageDistribution.BuildFriendlyDMGDistData(log, actor, target, phase, usedSkills, usedBuffs));
                }
                dto.DamageDistributionTargets.Add(dmgTargetsDto);
                dto.DamageDistributionTaken.Add(ExportModels.DamageDistribution.BuildDMGTakenDistData(log, actor, phase, usedSkills, usedBuffs));
                dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, actor, phase, usedBuffs));
            }
            foreach (KeyValuePair<long, Minions> pair in actor.GetMinions(log))
            {
                dto.Minions.Add(BuildFriendlyMinionsData(log, actor, pair.Value, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static LoggedPlayerDetails BuildFriendlyMinionsData(ParsedLog log, AbstractSingleActor actor, Minions minion, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedPlayerDetails
            {
                DamageDistribution = new List<DamageDistribution>(),
                DamageDistributionTargets = new List<List<DamageDistribution>>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var dmgTargetsDto = new List<DamageDistribution>();
                foreach (AbstractSingleActor target in phase.Targets)
                {
                    dmgTargetsDto.Add(ExportModels.DamageDistribution.BuildFriendlyMinionDMGDistData(log, actor, minion, target, phase, usedSkills, usedBuffs));
                }
                dto.DamageDistributionTargets.Add(dmgTargetsDto);
                dto.DamageDistribution.Add(ExportModels.DamageDistribution.BuildFriendlyMinionDMGDistData(log, actor, minion, null, phase, usedSkills, usedBuffs));
            }
            return dto;
        }

        public static LoggedPlayerDetails BuildTargetData(ParsedLog log, AbstractSingleActor target, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs, bool cr)
        {
            var dto = new LoggedPlayerDetails
            {
                DamageDistribution = new List<DamageDistribution>(),
                DamageDistributionTaken = new List<DamageDistribution>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>()
            };
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (phase.Targets.Contains(target))
                {
                    dto.DamageDistribution.Add(ExportModels.DamageDistribution.BuildTargetDMGDistData(log, target, phase, usedSkills, usedBuffs));
                    dto.DamageDistributionTaken.Add(ExportModels.DamageDistribution.BuildDMGTakenDistData(log, target, phase, usedSkills, usedBuffs));
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, target, phase, usedBuffs));
                }
                // rotation + buff graph for CR
                else if (i == 0 && cr)
                {
                    dto.DamageDistribution.Add(new ExportModels.DamageDistribution());
                    dto.DamageDistributionTaken.Add(new ExportModels.DamageDistribution());
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBoonGraphData(log, target, phase, usedBuffs));
                }
                else
                {
                    dto.DamageDistribution.Add(new ExportModels.DamageDistribution());
                    dto.DamageDistributionTaken.Add(new ExportModels.DamageDistribution());
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

        private static LoggedPlayerDetails BuildTargetsMinionsData(ParsedLog log, AbstractSingleActor target, Minions minion, Dictionary<long, Skill> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedPlayerDetails
            {
                DamageDistribution = new List<DamageDistribution>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                if (phase.Targets.Contains(target))
                {
                    dto.DamageDistribution.Add(ExportModels.DamageDistribution.BuildTargetMinionDMGDistData(log, target, minion, phase, usedSkills, usedBuffs));
                }
                else
                {
                    dto.DamageDistribution.Add(new ExportModels.DamageDistribution());
                }
            }
            return dto;
        }
    }
}
