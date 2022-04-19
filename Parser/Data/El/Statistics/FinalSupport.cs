using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves;
using System;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Statistics
{
    public class FinalSupport
    {
        public Dictionary<long, (int count, long time)> Removals { get; } = new Dictionary<long, (int count, long time)>();

        internal FinalSupport(ParsedLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor to)
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
                        // discard removals done on actor's minion
                        if (brae.To.Master != null && brae.To.GetFinalMaster() == actor.AgentItem)
                        {
                            continue;
                        }
                        count++;
                        time = Math.Max(time + brae.RemovedDuration, log.FightData.FightEnd);
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
