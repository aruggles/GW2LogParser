﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    public class FinalDefenses
    {
        //public long allHealReceived;
        public long DamageTaken { get; }
        public double BreakbarDamageTaken { get; }
        public int BlockedCount { get; }
        public int MissedCount { get; }
        public int EvadedCount { get; }
        public int DodgeCount { get; }
        public int InvulnedCount { get; }
        public int DamageBarrier { get; }
        public int InterruptedCount { get; }
        public int BoonStrips { get; }
        public double BoonStripsTime { get; }
        public int ConditionCleanses { get; }
        public double ConditionCleansesTime { get; }

        private static (int, double) GetStripData(IReadOnlyList<Buff> buffs, ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor from, bool excludeSelf)
        {
            double stripTime = 0;
            int strip = 0;
            foreach (Buff buff in buffs)
            {
                double currentBoonStripTime = 0;
                IReadOnlyList<BuffRemoveAllEvent> removeAllArray = log.CombatData.GetBuffRemoveAllData(buff.ID);
                foreach (BuffRemoveAllEvent brae in removeAllArray)
                {
                    if (brae.Time >= start && brae.Time <= end && brae.To == actor.AgentItem)
                    {
                        if (from != null && brae.CreditedBy != from.AgentItem || brae.CreditedBy == ParserHelper._unknownAgent || (excludeSelf && brae.CreditedBy == actor.AgentItem))
                        {
                            continue;
                        }
                        currentBoonStripTime = Math.Max(currentBoonStripTime + brae.RemovedDuration, log.FightData.FightDuration);
                        strip++;
                    }
                }
                stripTime += currentBoonStripTime;
            }
            stripTime = Math.Round(stripTime / 1000.0, ParserHelper.TimeDigit);
            return (strip, stripTime);
        }

        internal FinalDefenses(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor from)
        {
            IReadOnlyList<AbstractHealthDamageEvent> damageLogs = actor.GetDamageTakenEvents(from, log, start, end);

            DamageTaken = damageLogs.Sum(x => (long)x.HealthDamage);
            BreakbarDamageTaken = Math.Round(actor.GetBreakbarDamageTakenEvents(from, log, start, end).Sum(x => x.BreakbarDamage), 1);
            BlockedCount = damageLogs.Count(x => x.IsBlocked);
            MissedCount = damageLogs.Count(x => x.IsBlind);
            InvulnedCount = damageLogs.Count(x => x.IsAbsorbed);
            EvadedCount = damageLogs.Count(x => x.IsEvaded);
            DodgeCount = actor.GetCastEvents(log, start, end).Count(x => x.Skill.IsDodge(log.SkillData));
            DamageBarrier = damageLogs.Sum(x => x.ShieldDamage);
            InterruptedCount = damageLogs.Count(x => x.HasInterrupted);
            (BoonStrips, BoonStripsTime) = GetStripData(log.Buffs.BuffsByClassification[BuffClassification.Boon], log, start, end, actor, from, true);
            (ConditionCleanses, ConditionCleansesTime) = GetStripData(log.Buffs.BuffsByClassification[BuffClassification.Condition], log, start, end, actor, from, false);
        }
    }
}
