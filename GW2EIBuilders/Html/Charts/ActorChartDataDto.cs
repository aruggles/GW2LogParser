using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal abstract class ActorChartDataDto
    {
        public List<object[]> HealthStates { get; }
        public List<object[]> BarrierStates { get; }

        public ActorChartDataDto(ParsedLog log, PhaseData phase, AbstractSingleActor actor, bool nullableHPStates)
        {
            HealthStates = ChartDataDto.BuildHealthStates(log, actor, phase, nullableHPStates);
            BarrierStates = ChartDataDto.BuildBarrierStates(log, actor, phase);
        }
    }
}
