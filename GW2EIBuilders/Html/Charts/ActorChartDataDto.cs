using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
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
