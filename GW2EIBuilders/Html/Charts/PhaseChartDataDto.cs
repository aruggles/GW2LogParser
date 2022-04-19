using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class PhaseChartDataDto
    {
        public List<PlayerChartDataDto> Players { get; set; } = new List<PlayerChartDataDto>();
        public List<TargetChartDataDto> Targets { get; set; } = new List<TargetChartDataDto>();

        public List<List<object[]>> TargetsHealthStatesForCR { get; set; } = null;
        public List<List<object[]>> TargetsBreakbarPercentStatesForCR { get; set; } = null;
        public List<List<object[]>> TargetsBarrierStatesForCR { get; set; } = null;

        public PhaseChartDataDto(ParsedLog log, PhaseData phase, bool addCRData)
        {
            Players = PlayerChartDataDto.BuildPlayersGraphData(log, phase);
            foreach (AbstractSingleActor target in phase.Targets)
            {
                Targets.Add(new TargetChartDataDto(log, phase, target));
            }
            if (addCRData)
            {
                TargetsHealthStatesForCR = new List<List<object[]>>();
                TargetsBreakbarPercentStatesForCR = new List<List<object[]>>();
                TargetsBarrierStatesForCR = new List<List<object[]>>();
                foreach (AbstractSingleActor target in log.FightData.Logic.Targets)
                {
                    TargetsHealthStatesForCR.Add(ChartDataDto.BuildHealthStates(log, target, phase, false));
                    TargetsBreakbarPercentStatesForCR.Add(ChartDataDto.BuildBreakbarPercentStates(log, target, phase));
                    TargetsBarrierStatesForCR.Add(ChartDataDto.BuildBarrierStates(log, target, phase));
                }
            }
        }
    }
}
