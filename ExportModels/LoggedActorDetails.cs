using Gw2LogParser.GW2EIBuilders;
using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser;

namespace Gw2LogParser.ExportModels
{
    internal class LoggedActorDetails
    {
        public List<DamageDistribution> DmgDistributions { get; internal set; }
        public List<List<DamageDistribution>> DmgDistributionsTargets { get; internal set; }
        public List<DamageDistribution> DmgDistributionsTaken { get; internal set; }
        public List<List<object[]>> Rotation { get; internal set; }
        public List<List<BuffChartDataDto>> BoonGraph { get; internal set; }
        public List<List<List<BuffChartDataDto>>> BoonGraphPerSource { get; set; }
        public List<Food> Food { get; internal set; }
        public List<LoggedActorDetails> Minions { get; internal set; }
        public List<LoggedDeathRecap> DeathRecap { get; internal set; }

        // helpers

        internal static LoggedActorDetails BuildPlayerData(ParsedLog log, AbstractSingleActor actor, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedActorDetails
            {
                DmgDistributions = new List<DamageDistribution>(),
                DmgDistributionsTargets = new List<List<DamageDistribution>>(),
                DmgDistributionsTaken = new List<DamageDistribution>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>(),
                Food = ExportModels.Food.BuildFoodData(log, actor, usedBuffs),
                Minions = new List<LoggedActorDetails>(),
                DeathRecap = LoggedDeathRecap.BuildDeathRecap(log, actor)
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {

                dto.Rotation.Add(SkillDto.BuildRotationData(log, actor, phase, usedSkills));
                dto.DmgDistributions.Add(DamageDistribution.BuildFriendlyDMGDistData(log, actor, null, phase, usedSkills, usedBuffs));
                var dmgTargetsDto = new List<DamageDistribution>();
                foreach (AbstractSingleActor target in phase.Targets)
                {
                    dmgTargetsDto.Add(DamageDistribution.BuildFriendlyDMGDistData(log, actor, target, phase, usedSkills, usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributionsTaken.Add(DamageDistribution.BuildDMGTakenDistData(log, actor, phase, usedSkills, usedBuffs));
                dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, actor, phase, usedBuffs));
            }
            foreach (KeyValuePair<long, Minions> pair in actor.GetMinions(log))
            {
                dto.Minions.Add(BuildFriendlyMinionsData(log, actor, pair.Value, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static LoggedActorDetails BuildFriendlyMinionsData(ParsedLog log, AbstractSingleActor actor, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedActorDetails
            {
                DmgDistributions = new List<DamageDistribution>(),
                DmgDistributionsTargets = new List<List<DamageDistribution>>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var dmgTargetsDto = new List<DamageDistribution>();
                foreach (AbstractSingleActor target in phase.Targets)
                {
                    dmgTargetsDto.Add(DamageDistribution.BuildFriendlyMinionDMGDistData(log, actor, minion, target, phase, usedSkills, usedBuffs));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributions.Add(DamageDistribution.BuildFriendlyMinionDMGDistData(log, actor, minion, null, phase, usedSkills, usedBuffs));
            }
            return dto;
        }

        internal static LoggedActorDetails BuildTargetData(ParsedLog log, AbstractSingleActor target, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, bool cr)
        {
            var dto = new LoggedActorDetails
            {
                DmgDistributions = new List<DamageDistribution>(),
                DmgDistributionsTaken = new List<DamageDistribution>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                BoonGraphPerSource = new List<List<List<BuffChartDataDto>>>(),
                Rotation = new List<List<object[]>>()
            };
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (phase.Targets.Contains(target))
                {
                    dto.DmgDistributions.Add(DamageDistribution.BuildTargetDMGDistData(log, target, phase, usedSkills, usedBuffs));
                    dto.DmgDistributionsTaken.Add(DamageDistribution.BuildDMGTakenDistData(log, target, phase, usedSkills, usedBuffs));
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, target, phase, usedBuffs));
                    dto.BoonGraphPerSource.Add(log.Friendlies.Select(p => BuffChartDataDto.BuildBuffGraphData(log, target, p, phase, usedBuffs)).ToList());
                }
                // rotation + buff graph for CR
                else if (i == 0 && cr)
                {
                    dto.DmgDistributions.Add(new DamageDistribution());
                    dto.DmgDistributionsTaken.Add(new DamageDistribution());
                    dto.Rotation.Add(SkillDto.BuildRotationData(log, target, phase, usedSkills));
                    dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, target, phase, usedBuffs));
                    dto.BoonGraphPerSource.Add(new List<List<BuffChartDataDto>>());
                }
                else
                {
                    dto.DmgDistributions.Add(new DamageDistribution());
                    dto.DmgDistributionsTaken.Add(new DamageDistribution());
                    dto.Rotation.Add(new List<object[]>());
                    dto.BoonGraph.Add(new List<BuffChartDataDto>());
                    dto.BoonGraphPerSource.Add(new List<List<BuffChartDataDto>>());
                }
            }

            dto.Minions = new List<LoggedActorDetails>();
            foreach (KeyValuePair<long, Minions> pair in target.GetMinions(log))
            {
                dto.Minions.Add(BuildTargetsMinionsData(log, target, pair.Value, usedSkills, usedBuffs));
            }
            return dto;
        }

        private static LoggedActorDetails BuildTargetsMinionsData(ParsedLog log, AbstractSingleActor target, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var dto = new LoggedActorDetails
            {
                DmgDistributions = new List<DamageDistribution>()
            };
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                if (phase.Targets.Contains(target))
                {
                    dto.DmgDistributions.Add(DamageDistribution.BuildTargetMinionDMGDistData(log, target, minion, phase, usedSkills, usedBuffs));
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
