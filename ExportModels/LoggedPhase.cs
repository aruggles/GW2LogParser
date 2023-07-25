using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System;
using System.Collections.Generic;

namespace Gw2LogParser.ExportModels
{
    public class LoggedPhase
    {
        public string Name { get; set; }
        public long Duration { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public List<List<object>> DpsStats { get; set; }
        public List<List<List<object>>> DpsStatsTargets { get; set; }
        public List<List<List<object>>> DmgStatsTargets { get; set; }
        public List<List<object>> DmgStats { get; set; }
        public List<List<object>> DefStats { get; set; }
        public List<List<object>> SupportStats { get; set; }
        public List<long> PlayerActiveTimes { get; set; }

        public LoggedPhase(PhaseData phaseData, ParsedLog log)
        {
            Name = phaseData.Name;
            Duration = phaseData.DurationInMS;
            Start = phaseData.Start / 1000.0;
            End = phaseData.End / 1000.0;
            PlayerActiveTimes = new List<long>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                PlayerActiveTimes.Add(actor.GetActiveDuration(log, phaseData.Start, phaseData.End));
            }
        }

        public static List<object> GetDMGStatData(FinalGameplayStats stats)
        {
            var data = new List<object>
                {
                    // commons
                    stats.TimeWasted, // 0
                    stats.Wasted, // 1

                    stats.TimeSaved, // 2
                    stats.Saved, // 3

                    stats.SwapCount, // 4
                    Math.Round(stats.StackDist, 2), // 5
                    Math.Round(stats.DistToCom, 2), // 6
                    stats.SkillCastUptime, // 7
                    stats.SkillCastUptimeNoAA, // 8
                };
            return data;
        }

        public static List<object> GetDMGTargetStatData(FinalOffensiveStats stats)
        {
            var data = new List<object>
                {
                    stats.DirectDamageCount, // 0
                    stats.CritableDirectDamageCount, // 1
                    stats.CriticalCount, // 2
                    stats.CriticalDmg, // 3

                    stats.FlankingCount, // 4

                    stats.GlanceCount, // 5

                    stats.Missed,// 6
                    stats.Interrupts, // 7
                    stats.Invulned, // 8
                    stats.Evaded,// 9
                    stats.Blocked,// 10
                    stats.ConnectedDirectDamageCount, // 11
                    stats.Killed, // 12
                    stats.Downed, // 13
                    stats.AgainstMovingCount, // 14
                    stats.ConnectedDamageCount, // 15
                    stats.TotalDamageCount, // 16
                    stats.DownContribution, // 17
                    stats.ConnectedDmg, // 18
                    stats.ConnectedDirectDmg, // 19
                };
            return data;
        }

        public static List<object> GetDPSStatData(FinalDPS dpsAll)
        {
            var data = new List<object>
                {
                    dpsAll.Damage,
                    dpsAll.PowerDamage,
                    dpsAll.CondiDamage,
                    dpsAll.BreakbarDamage,
                };
            return data;
        }

        public static List<object> GetSupportStatData(FinalToPlayersSupport support)
        {
            var data = new List<object>()
                {
                    support.CondiCleanse,
                    support.CondiCleanseTime,
                    support.CondiCleanseSelf,
                    support.CondiCleanseTimeSelf,
                    support.BoonStrips,
                    support.BoonStripsTime,
                    support.Resurrects,
                    support.ResurrectTime
                };
            return data;
        }

        public static List<object> GetDefenseStatData(FinalDefensesAll defenses, PhaseData phase)
        {
            var data = new List<object>
                {
                    defenses.DamageTaken,
                    defenses.DamageBarrier,
                    defenses.MissedCount,
                    defenses.InterruptedCount,
                    defenses.InvulnedCount,
                    defenses.EvadedCount,
                    defenses.BlockedCount,
                    defenses.DodgeCount,
                    defenses.ConditionCleanses,
                    defenses.ConditionCleansesTime,
                    defenses.BoonStrips,
                    defenses.BoonStripsTime,
                };

            if (defenses.DownDuration > 0)
            {
                var downDuration = TimeSpan.FromMilliseconds(defenses.DownDuration);
                data.Add(defenses.DownCount);
                data.Add(downDuration.TotalSeconds + " seconds downed, " + Math.Round((downDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1) + "% Downed");
            }
            else
            {
                data.Add(0);
                data.Add("0% downed");
            }

            if (defenses.DeadDuration > 0)
            {
                var deathDuration = TimeSpan.FromMilliseconds(defenses.DeadDuration);
                data.Add(defenses.DeadCount);
                data.Add(deathDuration.TotalSeconds + " seconds dead, " + (100.0 - Math.Round((deathDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1)) + "% Alive");
            }
            else
            {
                data.Add(0);
                data.Add("100% Alive");
            }
            return data;
        }
    }
}
