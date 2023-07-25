using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class ChartDataDto
    {
        public List<PhaseChartDataDto> Phases { get; } = new List<PhaseChartDataDto>();
        public List<MechanicChartDataDto> Mechanics { get; } = new List<MechanicChartDataDto>();

        private static List<object[]> BuildGraphStates(IReadOnlyList<Segment> segments, PhaseData phase, bool nullable, double defaultState)
        {
            if (!segments.Any())
            {
                return nullable ? null : new List<object[]>()
                {
                    new object[] { 0.0, defaultState},
                    new object[] { Math.Round(phase.DurationInMS/1000.0, 3), defaultState},
                };
            }
            var res = new List<object[]>();
            var subSegments = segments.Where(x => x.End >= phase.Start && x.Start <= phase.End
            ).ToList();
            return Segment.ToObjectList(subSegments, phase.Start, phase.End);
        }

        public static List<object[]> BuildHealthStates(ParsedLog log, AbstractSingleActor actor, PhaseData phase, bool nullable)
        {
            return BuildGraphStates(actor.GetHealthUpdates(log), phase, nullable, 100.0);
        }

        public static List<object[]> BuildBarrierStates(ParsedLog log, AbstractSingleActor actor, PhaseData phase)
        {
            var barriers = new List<Segment>(actor.GetBarrierUpdates(log));
            if (!barriers.Any(x => x.Value > 0))
            {
                barriers.Clear();
            }
            return BuildGraphStates(barriers, phase, true, 0.0);
        }

        public static List<object[]> BuildBreakbarPercentStates(ParsedLog log, AbstractSingleActor npc, PhaseData phase)
        {
            return BuildGraphStates(npc.GetBreakbarPercentUpdates(log), phase, true, 100.0);
        }

        public ChartDataDto(ParsedLog log)
        {
            var phaseChartData = new List<PhaseChartDataDto>();
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                phaseChartData.Add(new PhaseChartDataDto(log, phases[i], i == 0));
            }
            Phases = phaseChartData;
            Mechanics = MechanicChartDataDto.BuildMechanicsChartData(log);
        }
    }
}
