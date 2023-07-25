﻿using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class FinalSupport
    {
        public Dictionary<long, (int count, long time)> Removals { get; } = new Dictionary<long, (int count, long time)>();

        internal FinalSupport(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor to)
        {
            foreach (long buffID in log.Buffs.BuffsByIds.Keys)
            {
                int count = 0;
                long time = 0;
                foreach (BuffRemoveAllEvent brae in log.CombatData.GetBuffRemoveAllData(buffID))
                {
                    if (brae.Time >= start && brae.Time <= end && brae.CreditedBy == actor.AgentItem)
                    {
                        if (to != null && brae.To != to.AgentItem)
                        {
                            continue;
                        }
                        count++;
                        time = Math.Max(time + brae.RemovedDuration, log.FightData.FightDuration);
                    }
                }
                if (count > 0)
                {
                    Removals[buffID] = (count, time);
                }
            }
        }

    }
}
