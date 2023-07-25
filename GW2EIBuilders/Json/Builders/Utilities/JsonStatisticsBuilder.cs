using GW2EIEvtcParser.EIData;
using static Gw2LogParser.GW2EIBuilders.JsonStatistics;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonStatisticsBuilder
    {
        public static JsonDefensesAll BuildJsonDefensesAll(FinalDefensesAll defenses)
        {
            var jsonDefensesAll = new JsonDefensesAll();
            jsonDefensesAll.DamageTaken = defenses.DamageTaken;
            jsonDefensesAll.BreakbarDamageTaken = defenses.BreakbarDamageTaken;
            jsonDefensesAll.BlockedCount = defenses.BlockedCount;
            jsonDefensesAll.DodgeCount = defenses.DodgeCount;
            jsonDefensesAll.MissedCount = defenses.MissedCount;
            jsonDefensesAll.EvadedCount = defenses.EvadedCount;
            jsonDefensesAll.InvulnedCount = defenses.InvulnedCount;
            jsonDefensesAll.DamageBarrier = defenses.DamageBarrier;
            jsonDefensesAll.InterruptedCount = defenses.InterruptedCount;
            jsonDefensesAll.DownCount = defenses.DownCount;
            jsonDefensesAll.DownDuration = defenses.DownDuration;
            jsonDefensesAll.DeadCount = defenses.DeadCount;
            jsonDefensesAll.DeadDuration = defenses.DeadDuration;
            jsonDefensesAll.DcCount = defenses.DcCount;
            jsonDefensesAll.DcDuration = defenses.DcDuration;
            return jsonDefensesAll;
        }


        public static JsonDPS BuildJsonDPS(FinalDPS stats)
        {
            var jsonDPS = new JsonDPS
            {
                Dps = stats.Dps,
                Damage = stats.Damage,
                CondiDps = stats.CondiDps,
                CondiDamage = stats.CondiDamage,
                PowerDps = stats.PowerDps,
                PowerDamage = stats.PowerDamage,
                BreakbarDamage = stats.BreakbarDamage,

                ActorDps = stats.ActorDps,
                ActorDamage = stats.ActorDamage,
                ActorCondiDps = stats.ActorCondiDps,
                ActorCondiDamage = stats.ActorCondiDamage,
                ActorPowerDps = stats.ActorPowerDps,
                ActorPowerDamage = stats.ActorPowerDamage,
                ActorBreakbarDamage = stats.ActorBreakbarDamage
            };

            return jsonDPS;
        }

        private static void FillJsonGamePlayStats(JsonGameplayStats jsonGameplayStats, FinalOffensiveStats offStats)
        {
            jsonGameplayStats.TotalDamageCount = offStats.TotalDamageCount;
            jsonGameplayStats.DirectDamageCount = offStats.DirectDamageCount;
            jsonGameplayStats.ConnectedDirectDamageCount = offStats.ConnectedDirectDamageCount;
            jsonGameplayStats.ConnectedDamageCount = offStats.ConnectedDamageCount;
            jsonGameplayStats.CritableDirectDamageCount = offStats.CritableDirectDamageCount;
            jsonGameplayStats.CriticalRate = offStats.CriticalCount;
            jsonGameplayStats.CriticalDmg = offStats.CriticalDmg;
            jsonGameplayStats.FlankingRate = offStats.FlankingCount;
            jsonGameplayStats.GlanceRate = offStats.GlanceCount;
            jsonGameplayStats.AgainstMovingRate = offStats.AgainstMovingCount;
            jsonGameplayStats.Missed = offStats.Missed;
            jsonGameplayStats.Blocked = offStats.Blocked;
            jsonGameplayStats.Evaded = offStats.Evaded;
            jsonGameplayStats.Interrupts = offStats.Interrupts;
            jsonGameplayStats.Invulned = offStats.Invulned;
            jsonGameplayStats.Killed = offStats.Killed;
            jsonGameplayStats.Downed = offStats.Downed;
        }

        public static JsonGameplayStats BuildJsonGameplayStats(FinalOffensiveStats stats)
        {
            var jsonGameplayStats = new JsonGameplayStats();
            FillJsonGamePlayStats(jsonGameplayStats, stats);
            return jsonGameplayStats;
        }

        public static JsonGameplayStatsAll BuildJsonGameplayStatsAll(FinalGameplayStats stats, FinalOffensiveStats offStats)
        {
            var jsonGameplayStatsAll = new JsonGameplayStatsAll
            {
                Wasted = stats.Wasted,
                TimeWasted = stats.TimeWasted,
                Saved = stats.Saved,
                TimeSaved = stats.TimeSaved,
                StackDist = stats.StackDist,
                DistToCom = stats.DistToCom,
                AvgBoons = stats.AvgBoons,
                AvgActiveBoons = stats.AvgActiveBoons,
                AvgConditions = stats.AvgConditions,
                AvgActiveConditions = stats.AvgActiveConditions,
                SwapCount = stats.SwapCount
            };
            FillJsonGamePlayStats(jsonGameplayStatsAll, offStats);
            return jsonGameplayStatsAll;
        }


        public static JsonPlayerSupport BuildJsonPlayerSupport(FinalToPlayersSupport stats)
        {
            var jsonPlayerSupport = new JsonPlayerSupport
            {
                Resurrects = stats.Resurrects,
                ResurrectTime = stats.ResurrectTime,
                CondiCleanse = stats.CondiCleanse,
                CondiCleanseTime = stats.CondiCleanseTime,
                CondiCleanseSelf = stats.CondiCleanseSelf,
                CondiCleanseTimeSelf = stats.CondiCleanseTimeSelf,
                BoonStrips = stats.BoonStrips,
                BoonStripsTime = stats.BoonStripsTime
            };
            return jsonPlayerSupport;
        }
    }
}
