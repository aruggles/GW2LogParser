﻿using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class EXTBarrierStatsPhaseDto
    {
        public List<List<object>> OutgoingBarrierStats { get; set; }
        public List<List<List<object>>> OutgoingBarrierStatsTargets { get; set; }
        public List<List<object>> IncomingBarrierStats { get; set; }

        public EXTBarrierStatsPhaseDto(PhaseData phase, ParsedLog log)
        {
            OutgoingBarrierStats = BuildOutgoingBarrierStatData(log, phase);
            OutgoingBarrierStatsTargets = BuildOutgoingBarrierFriendlyStatData(log, phase);
            IncomingBarrierStats = BuildIncomingBarrierStatData(log, phase);
        }


        // helper methods

        private static List<object> GetOutgoingBarrierStatData(EXTFinalOutgoingBarrierStat outgoingBarrierStats)
        {
            var data = new List<object>
                {
                    outgoingBarrierStats.Barrier,
                };
            return data;
        }

        private static List<object> GetIncomingBarrierStatData(EXTFinalIncomingBarrierStat incomingBarrierStats)
        {
            var data = new List<object>
                {
                    incomingBarrierStats.BarrierReceived,
                };
            return data;
        }
        public static List<List<object>> BuildOutgoingBarrierStatData(ParsedLog log, PhaseData phase)
        {
            var list = new List<List<object>>(log.Friendlies.Count);
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                EXTFinalOutgoingBarrierStat outgoingBarrierStats = actor.EXTBarrier.GetOutgoingBarrierStats(null, log, phase.Start, phase.End);
                list.Add(GetOutgoingBarrierStatData(outgoingBarrierStats));
            }
            return list;
        }

        public static List<List<List<object>>> BuildOutgoingBarrierFriendlyStatData(ParsedLog log, PhaseData phase)
        {
            var list = new List<List<List<object>>>(log.Friendlies.Count);

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                var playerData = new List<List<object>>();

                foreach (AbstractSingleActor target in log.Friendlies)
                {
                    playerData.Add(GetOutgoingBarrierStatData(actor.EXTBarrier.GetOutgoingBarrierStats(target, log, phase.Start, phase.End)));
                }
                list.Add(playerData);
            }
            return list;
        }

        public static List<List<object>> BuildIncomingBarrierStatData(ParsedLog log, PhaseData phase)
        {
            var list = new List<List<object>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                EXTFinalIncomingBarrierStat incomingBarrierStats = actor.EXTBarrier.GetIncomingBarrierStats(null, log, phase.Start, phase.End);
                list.Add(GetIncomingBarrierStatData(incomingBarrierStats));
            }

            return list;
        }
    }
}
