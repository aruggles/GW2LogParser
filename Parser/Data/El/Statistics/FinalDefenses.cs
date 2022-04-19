﻿using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Damage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Statistics
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

        internal FinalDefenses(ParsedLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor from)
        {
            IReadOnlyList<AbstractHealthDamageEvent> damageLogs = actor.GetDamageTakenEvents(from, log, start, end);

            DamageTaken = damageLogs.Sum(x => (long)x.HealthDamage);
            BreakbarDamageTaken = Math.Round(actor.GetBreakbarDamageTakenEvents(from, log, start, end).Sum(x => x.BreakbarDamage), 1);
            BlockedCount = damageLogs.Count(x => x.IsBlocked);
            MissedCount = damageLogs.Count(x => x.IsBlind);
            InvulnedCount = damageLogs.Count(x => x.IsAbsorbed);
            EvadedCount = damageLogs.Count(x => x.IsEvaded);
            DodgeCount = actor.GetCastEvents(log, start, end).Count(x => x.Skill.IsDodge);
            DamageBarrier = damageLogs.Sum(x => x.ShieldDamage);
            InterruptedCount = damageLogs.Count(x => x.HasInterrupted);
        }
    }
}
