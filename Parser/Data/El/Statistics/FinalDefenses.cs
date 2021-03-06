﻿using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Statistics
{
    public class FinalDefenses
    {
        //public long allHealReceived;
        public long DamageTaken { get; }
        public int BlockedCount { get; }
        public int MissedCount { get; }
        public int EvadedCount { get; }
        public int DodgeCount { get; }
        public int InvulnedCount { get; }
        public int DamageBarrier { get; }
        public int InterruptedCount { get; }

        internal FinalDefenses(ParsedLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor from)
        {
            long start = phase.Start;
            long end = phase.End;
            List<AbstractDamageEvent> damageLogs = actor.GetDamageTakenLogs(from, log, start, end);

            DamageTaken = damageLogs.Sum(x => (long)x.Damage);
            BlockedCount = damageLogs.Count(x => x.IsBlocked);
            MissedCount = damageLogs.Count(x => x.IsBlind);
            InvulnedCount = damageLogs.Count(x => x.IsAbsorbed);
            EvadedCount = damageLogs.Count(x => x.IsEvaded);
            DodgeCount = actor.GetCastLogs(log, start, end).Count(x => x.SkillId == Skill.DodgeId || x.SkillId == Skill.MirageCloakDodgeId);
            DamageBarrier = damageLogs.Sum(x => x.ShieldDamage);
            InterruptedCount = damageLogs.Count(x => x.HasInterrupted);
        }
    }
}
