using GW2EIEvtcParser.ParsedData;
using Gw2LogParser.GW2EIBuilders.Html.Extensions.BarrierStats;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class BarrierStatsExtension
    {
        public List<EXTBarrierStatsPhaseDto> BarrierPhases { get; }

        public List<EXTBarrierStatsPlayerDetailsDto> PlayerBarrierDetails { get; }

        public List<List<EXTBarrierStatsPlayerChartDto>> PlayerBarrierCharts { get; }

        public BarrierStatsExtension(ParsedLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            BarrierPhases = new List<EXTBarrierStatsPhaseDto>();
            PlayerBarrierCharts = new List<List<EXTBarrierStatsPlayerChartDto>>();
            PlayerBarrierDetails = new List<EXTBarrierStatsPlayerDetailsDto>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                BarrierPhases.Add(new EXTBarrierStatsPhaseDto(phase, log));
                PlayerBarrierCharts.Add(EXTBarrierStatsPlayerChartDto.BuildPlayersBarrierGraphData(log, phase));
            }
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                PlayerBarrierDetails.Add(EXTBarrierStatsPlayerDetailsDto.BuildPlayerBarrierData(log, actor, usedSkills, usedBuffs));
            }
        }
    }
}
