using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class EXTBarrierStatsPlayerChartDto
    {
        public PlayerDamageChartDto<int> Barrier { get; }

        private EXTBarrierStatsPlayerChartDto(ParsedLog log, PhaseData phase, AbstractSingleActor p)
        {
            Barrier = new PlayerDamageChartDto<int>()
            {
                Total = p.EXTBarrier.Get1SBarrierList(log, phase.Start, phase.End, null),
                Targets = new List<IReadOnlyList<int>>()
            };
            foreach (AbstractSingleActor target in log.Friendlies)
            {
                Barrier.Targets.Add(p.EXTBarrier.Get1SBarrierList(log, phase.Start, phase.End, target));
            }
        }

        public static List<EXTBarrierStatsPlayerChartDto> BuildPlayersBarrierGraphData(ParsedLog log, PhaseData phase)
        {
            var list = new List<EXTBarrierStatsPlayerChartDto>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new EXTBarrierStatsPlayerChartDto(log, phase, actor));
            }
            return list;
        }
    }
}
