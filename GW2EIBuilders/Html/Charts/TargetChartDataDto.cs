﻿using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class TargetChartDataDto : ActorChartDataDto
    {
        public IReadOnlyList<int> Total { get; }
        public IReadOnlyList<int> TotalPower { get; }
        public IReadOnlyList<int> TotalCondition { get; }
        public List<object[]> BreakbarPercentStates { get; }

        public TargetChartDataDto(ParsedLog log, PhaseData phase, AbstractSingleActor target) : base(log, phase, target, false)
        {
            Total = target.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.All);
            TotalPower = target.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Power);
            TotalCondition = target.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Condition);
            BreakbarPercentStates = ChartDataDto.BuildBreakbarPercentStates(log, target, phase);
        }
    }
}
