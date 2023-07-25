﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTFinalIncomingBarrierStat
    {
        public int BarrierReceived { get; }
        public int DownedBarrierReceived { get; }

        internal EXTFinalIncomingBarrierStat(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            foreach (EXTAbstractBarrierEvent barrierEvent in actor.EXTBarrier.GetIncomingBarrierEvents(target, log, start, end))
            {
                BarrierReceived += barrierEvent.BarrierGiven;
                if (barrierEvent.AgainstDowned)
                {
                    DownedBarrierReceived += barrierEvent.BarrierGiven;
                }
            }
        }

    }
    
}
