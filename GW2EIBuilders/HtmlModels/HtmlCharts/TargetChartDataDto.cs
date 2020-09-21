using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    public class TargetChartDataDto
    {
        public List<int> Total { get; internal set; }
        public List<object[]> HealthStates { get; internal set; }
        public List<object[]> BreakbarPercentStates { get; internal set; }

        internal static TargetChartDataDto BuildTargetGraphData(ParsedLog log, int phaseIndex, NPC target)
        {
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            return new TargetChartDataDto
            {
                Total = target.Get1SDamageList(log, phaseIndex, phase, null),
                HealthStates = ChartDataDto.BuildHealthGraphStates(log, target, log.FightData.GetPhases(log)[phaseIndex], false),
                BreakbarPercentStates = ChartDataDto.BuildBreakbarPercentStates(log, target, log.FightData.GetPhases(log)[phaseIndex]),
            };
        }
    }
}
