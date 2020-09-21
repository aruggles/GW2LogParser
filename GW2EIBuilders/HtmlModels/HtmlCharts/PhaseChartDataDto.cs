using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    public class PhaseChartDataDto
    {
        public List<PlayerChartDataDto> Players { get; internal set; } = new List<PlayerChartDataDto>();
        public List<TargetChartDataDto> Targets { get; internal set; } = new List<TargetChartDataDto>();

        public List<List<object[]>> TargetsHealthStatesForCR { get; internal set; } = null;
        public List<List<object[]>> TargetsBreakbarPercentStatesForCR { get; internal set; } = null;
    }
}
