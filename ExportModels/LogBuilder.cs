using Gw2LogParser.ExportModels.Report;
using Gw2LogParser.GW2EIBuilders;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;
using Gw2LogParser.Parser.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.ExportModels
{
    internal class LogBuilder
    {
        private readonly ParsedLog log;
        private readonly Dictionary<long, Skill> usedSkills = new Dictionary<long, Skill>();
        private readonly Dictionary<long, Buff> usedBuffs = new Dictionary<long, Buff>();
        private readonly HashSet<DamageModifier> usedDamageMods = new HashSet<DamageModifier>();
        public bool cr = true;
        public bool light = false;
        public string[] uploadLinks = new string[] {};

        public LogBuilder(ParsedLog parsedLog)
        {
            log = parsedLog;
        }

        internal LogDataDto BuildLogData(Version parserVersion, UploadResults uploadResults)
        {
            bool combatReplay = true;
            bool lightTheme = true;
            return LogDataDto.BuildLogData(log, combatReplay, lightTheme, parserVersion, uploadResults.ToArray());
        }

        public SummaryItem BuildSummary(object[] item, LogDataDto data)
        {
            var isBuff = (bool)item[0];
            var id = (long)item[1];
            var icon = "";
            var name = "Unknown";
            if (isBuff)
            {
                var buff = data.BuffMap["b" + id];
                icon = buff.Icon;
                name = buff.Name;
            }
            else
            {
                var skill = data.SkillMap["s" + id];
                icon = skill.Icon;
                name = skill.Name;
            }
            var summaryItem = new SummaryItem()
            {
                Icon = icon,
                Skill = name

            };
            summaryItem.Damage = (long)Convert.ChangeType(item[2], typeof(long));
            summaryItem.BarrierDamage = (long)Convert.ChangeType(item[12], typeof(long));
            summaryItem.Min = (int)Convert.ChangeType(item[3], typeof(int));
            if (summaryItem.Min < 0)
            {
                summaryItem.Min = 0;
            }
            summaryItem.Max = (int)Convert.ChangeType(item[4], typeof(int));
            summaryItem.Casts = (int)Convert.ChangeType(item[5], typeof(int));
            summaryItem.Hits = (int)Convert.ChangeType(item[6], typeof(int));
            summaryItem.Wasted = (float)Convert.ChangeType(item[10], typeof(float));
            summaryItem.Saved = (float)Convert.ChangeType(item[11], typeof(float));
            summaryItem.HitsPerCast = (summaryItem.Casts == 0) ? 0 : (float)summaryItem.Hits / (float)summaryItem.Casts;
            summaryItem.Crit = (int)Convert.ChangeType(item[7], typeof(int));
            summaryItem.Flank = (int)Convert.ChangeType(item[8], typeof(int));
            summaryItem.Glance = (int)Convert.ChangeType(item[9], typeof(int));
            return summaryItem;
        }

        public void SumSummaryStats(SummaryItem original, SummaryItem item)
        {
            original.Avg += item.Avg;
            original.BarrierDamage += item.BarrierDamage;
            original.Casts += item.Casts;
            original.Crit += item.Crit;
            original.Damage += item.Damage;
            original.Flank += item.Flank;
            original.Glance += item.Glance;
            original.Hits += item.Hits;
            original.HitsPerCast += (original.Casts == 0) ? 0 : (float)original.Hits / (float)original.Casts;
            original.Max = Math.Max(original.Max, item.Max);
            original.Min = Math.Min(original.Min, item.Min);
            original.Saved += item.Saved;
            original.Wasted += item.Wasted;
        }

        public void SumPlayerStats(PlayerReport original, PlayerReport report)
        {
            original.numberOfFights++;
            original.TimeInCombat += report.TimeInCombat;
            original.Damage.AllDamage += report.Damage.AllDamage;
            original.Damage.Power += report.Damage.Power;
            original.Damage.Condi += report.Damage.Condi;
            original.Damage.TargetDamage += report.Damage.TargetDamage;
            original.Damage.TargetPower += report.Damage.TargetPower;
            original.Damage.TargetCondi += report.Damage.TargetCondi;

            original.Support.BoonStrips += report.Support.BoonStrips;
            original.Support.CleanseOnOther += report.Support.CleanseOnOther;
            original.Support.CleanseOnSelf += report.Support.CleanseOnSelf;
            original.Support.Resurrects += report.Support.Resurrects;

            original.Defense.DamageTaken += report.Defense.DamageTaken;
            original.Defense.DamageBarrier += report.Defense.DamageBarrier;
            original.Defense.Blocked += report.Defense.Blocked;
            original.Defense.Invulned += report.Defense.Invulned;
            original.Defense.Interrupted += report.Defense.Interrupted;
            original.Defense.Evaded += report.Defense.Evaded;
            original.Defense.Dodges += report.Defense.Dodges;
            original.Defense.Missed += report.Defense.Missed;
            original.Defense.Downed += report.Defense.Downed;            
            original.Defense.Dead += report.Defense.Dead;

            original.Gameplay.AvgDistanceToSquad += report.Gameplay.AvgDistanceToSquad;
            original.Gameplay.AvgDistanceToTag += report.Gameplay.AvgDistanceToTag;
            original.Gameplay.Blined += report.Gameplay.Blined;
            original.Gameplay.Blocked += report.Gameplay.Blocked;
            original.Gameplay.ConnectedHits += report.Gameplay.ConnectedHits;
            original.Gameplay.CritableHits += report.Gameplay.CritableHits;
            original.Gameplay.CriticalHits += report.Gameplay.CriticalHits;
            original.Gameplay.Evaded += report.Gameplay.Evaded;
            original.Gameplay.Flanking += report.Gameplay.Flanking;
            original.Gameplay.Glancing += report.Gameplay.Glancing;
            original.Gameplay.Interrupted += report.Gameplay.Interrupted;
            original.Gameplay.Invulnerable += report.Gameplay.Invulnerable;
            original.Gameplay.Downed += report.Gameplay.Downed;
            original.Gameplay.Killed += report.Gameplay.Killed;
            original.Gameplay.Saved += report.Gameplay.Saved;
            original.Gameplay.SavedCount += report.Gameplay.SavedCount;
            original.Gameplay.Wasted += report.Gameplay.Wasted;
            original.Gameplay.WastedCount += report.Gameplay.WastedCount;
            original.Gameplay.WeaponSwapped += report.Gameplay.WeaponSwapped;
            // Damage Summary
            var dictionary = new Dictionary<String, int?>();
            for (int i = 0; i < original.DamageSummary.Count; i++)
            {
                var summaryItem = original.DamageSummary[i];
                dictionary[summaryItem.Skill] = i;
            }
            foreach (var summaryItem in report.DamageSummary)
            {
                dictionary.TryGetValue(summaryItem.Skill, out int? index);
                if (index == null)
                {
                    original.DamageSummary.Add(new SummaryItem(summaryItem));
                }
                else if (index is int i)
                {
                    SumSummaryStats(original.DamageSummary[i], summaryItem);
                }
            }
            // Damage Taken Summary.
            dictionary = new Dictionary<String, int?>();
            for (int i = 0; i < original.TakenSummary.Count; i++)
            {
                var summaryItem = original.TakenSummary[i];
                dictionary[summaryItem.Skill] = i;
            }
            foreach (var summaryItem in report.TakenSummary)
            {
                dictionary.TryGetValue(summaryItem.Skill, out int? index);
                if (index == null)
                {
                    original.TakenSummary.Add(new SummaryItem(summaryItem));
                }
                else if (index is int i)
                {
                    SumSummaryStats(original.TakenSummary[i], summaryItem);
                }
            }
            // Boon Summary
            for (var i = 0; i < original.BoonStats.Count; i++)
            {
                var originalBoon = original.BoonStats[i];
                
                if (report.BoonStats.Count > i)
                {
                    var reportBoon = report.BoonStats[i];
                    originalBoon.Value += reportBoon.Value;
                    originalBoon.Wasted += reportBoon.Wasted;
                    originalBoon.Uptime += reportBoon.Uptime;
                    originalBoon.Extended += reportBoon.Extended;
                }
            }
            for (var i = 0; i < original.BoonGenSelfStats.Count; i++)
            {
                var originalBoon = original.BoonGenSelfStats[i];
                if (report.BoonGenSelfStats.Count > i)
                {
                    var reportBoon = report.BoonGenSelfStats[i];
                    originalBoon.Value += reportBoon.Value;
                    originalBoon.Wasted += reportBoon.Wasted;
                    originalBoon.Uptime += reportBoon.Uptime;
                    originalBoon.Extended += reportBoon.Extended;
                }
            }
            for (var i = 0; i < original.BoonGenGroupStats.Count; i++)
            {
                var originalBoon = original.BoonGenGroupStats[i];
                if (report.BoonGenGroupStats.Count > i)
                {
                    var reportBoon = report.BoonGenGroupStats[i];
                    originalBoon.Value += reportBoon.Value;
                    originalBoon.Wasted += reportBoon.Wasted;
                    originalBoon.Uptime += reportBoon.Uptime;
                    originalBoon.Extended += reportBoon.Extended;
                }
            }
            for (var i = 0; i < original.BoonGenOGroupStats.Count; i++)
            {
                var originalBoon = original.BoonGenOGroupStats[i];
                if (report.BoonGenOGroupStats.Count > i)
                {
                    var reportBoon = report.BoonGenOGroupStats[i];
                    originalBoon.Value += reportBoon.Value;
                    originalBoon.Wasted += reportBoon.Wasted;
                    originalBoon.Uptime += reportBoon.Uptime;
                    originalBoon.Extended += reportBoon.Extended;
                }
            }
            for (var i = 0; i < original.BoonGenSquadStats.Count; i++)
            {
                var originalBoon = original.BoonGenSquadStats[i];
                if (report.BoonGenSquadStats.Count > i)
                {
                    var reportBoon = report.BoonGenSquadStats[i];
                    originalBoon.Value += reportBoon.Value;
                    originalBoon.Wasted += reportBoon.Wasted;
                    originalBoon.Uptime += reportBoon.Uptime;
                    originalBoon.Extended += reportBoon.Extended;
                }
            }
            if (report.healing != null)
            {
                if (original.healing == null)
                {
                    original.healing = report.healing;
                } else
                {
                    original.healing.IncomingConversion += report.healing.IncomingConversion;
                    original.healing.IncomingDowned += report.healing.IncomingDowned;
                    original.healing.IncomingHealed += report.healing.IncomingHealed;
                    original.healing.IncomingHealingPower += report.healing.IncomingHealingPower;
                    original.healing.OutgoingAll += report.healing.OutgoingAll;
                    original.healing.OutgoingAllConversion += report.healing.OutgoingAllConversion;
                    original.healing.OutgoingAllDowned += report.healing.OutgoingAllDowned;
                    original.healing.OutgoingAllHealingPower += report.healing.OutgoingAllHealingPower;
                    original.healing.OutgoingTargetAll += report.healing.OutgoingTargetAll;
                    original.healing.OutgoingTargetConversion += report.healing.OutgoingTargetConversion;
                    original.healing.OutgoingTargetDowned += report.healing.OutgoingTargetDowned;
                    original.healing.OutgoingTargetHealingPower += report.healing.OutgoingTargetHealingPower;
                }
            }
        }

        private T Parse<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public void UpdateLogReport(LogReport report, LogDataDto data)
        {
            for (int playerIndex = 0; playerIndex < data.Players.Count; playerIndex++)
            {
                var player = data.Players[playerIndex];
                var playerReport = new PlayerReport()
                {
                    Account = player.Acc,
                    Name = player.Name,
                    Profession = player.Profession,
                    Group = player.Group,
                };
                foreach (object[] item in player.Details.DmgDistributions[0].Distribution)
                {
                    playerReport.DamageSummary.Add(BuildSummary(item, data));
                }
                foreach (object[] item in player.Details.DmgDistributionsTaken[0].Distribution)
                {
                    playerReport.TakenSummary.Add(BuildSummary(item, data));
                }
                //var count = data.Wvw ? 1 : data.Phases.Count;
                for (int phaseIndex = 0; phaseIndex < data.Phases.Count; phaseIndex++)
                {
                    var phase = data.Phases[phaseIndex];
                    playerReport.TimeInCombat = phase.PlayerActiveTimes[playerIndex];


                    playerReport.Damage.AllDamage = Parse<long>(phase.DpsStats[playerIndex][0]);
                    playerReport.Damage.Power = Parse<long>(phase.DpsStats[playerIndex][1]);
                    playerReport.Damage.Condi = Parse<long>(phase.DpsStats[playerIndex][2]);
                    var sumstates = new int[3];
                    var dpsStatsTargetsForPlayer = phase.DpsStatsTargets[playerIndex];
                    for (int i = 0; i < dpsStatsTargetsForPlayer.Count; i++)
                    {
                        var forTarget = dpsStatsTargetsForPlayer[i];
                        sumstates[0] += (int)forTarget[0];
                        sumstates[1] += (int)forTarget[1];
                        sumstates[2] += (int)forTarget[2];
                    }
                    playerReport.Damage.TargetDamage = sumstates[0];
                    playerReport.Damage.TargetPower = sumstates[1];
                    playerReport.Damage.TargetCondi = sumstates[2];

                    playerReport.Support.CleanseOnOther = Parse<int>(phase.SupportStats[playerIndex][0]);
                    playerReport.Support.CleanseOnSelf = Parse<int>(phase.SupportStats[playerIndex][2]);
                    playerReport.Support.BoonStrips = Parse<int>(phase.SupportStats[playerIndex][4]);
                    playerReport.Support.Resurrects = Parse<int>(phase.SupportStats[playerIndex][6]);

                    playerReport.Defense.DamageTaken = Parse<long>(phase.DefStats[playerIndex][0]);
                    playerReport.Defense.DamageBarrier = Parse<long>(phase.DefStats[playerIndex][1]);
                    playerReport.Defense.Blocked = Parse<int>(phase.DefStats[playerIndex][2]);
                    playerReport.Defense.Invulned = Parse<int>(phase.DefStats[playerIndex][3]);
                    playerReport.Defense.Interrupted = Parse<int>(phase.DefStats[playerIndex][4]);
                    playerReport.Defense.Evaded = Parse<int>(phase.DefStats[playerIndex][5]);
                    playerReport.Defense.Dodges = Parse<int>(phase.DefStats[playerIndex][6]);
                    playerReport.Defense.Missed = Parse<int>(phase.DefStats[playerIndex][7]);
                    playerReport.Defense.Downed = Parse<int>(phase.DefStats[playerIndex][8]);
                    playerReport.Defense.Dead = Parse<int>(phase.DefStats[playerIndex][10]);
                    playerReport.Gameplay = BuildGameplayReport(phase.DmgStats[playerIndex]);
                    
                    playerReport.setBoonStats(phase.BoonStats[playerIndex].Data);
                    playerReport.setBoonGenSquadStats(phase.BoonGenSquadStats[playerIndex].Data);
                    playerReport.setBoonGenSelfStats(phase.BoonGenSelfStats[playerIndex].Data);
                    playerReport.setBoonGenOGroupStats(phase.BoonGenOGroupStats[playerIndex].Data);
                    playerReport.setBoonGenGroupStats(phase.BoonGenGroupStats[playerIndex].Data);
                    if (data.HealingStatsExtension != null)
                    {
                        var healingReport = new HealingReport();
                        healingReport.IncomingHealed = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].IncomingHealingStats[playerIndex][0]);
                        healingReport.IncomingHealingPower = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].IncomingHealingStats[playerIndex][1]);
                        healingReport.IncomingConversion = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].IncomingHealingStats[playerIndex][2]);
                        healingReport.IncomingDowned = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].IncomingHealingStats[playerIndex][3]);
                        healingReport.OutgoingAll = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].OutgoingHealingStats[playerIndex][0]);
                        healingReport.OutgoingAllHealingPower = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].OutgoingHealingStats[playerIndex][1]);
                        healingReport.OutgoingAllConversion = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].OutgoingHealingStats[playerIndex][2]);
                        healingReport.OutgoingAllDowned = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].OutgoingHealingStats[playerIndex][3]);
                        int OutgoingTargetAll = 0;
                        int OutgoingTargetHealingPower = 0;
                        int OutgoingTargetConversion = 0;
                        int OutgoingTargetDowned = 0;
                        var targetStats = data.HealingStatsExtension.HealingPhases[phaseIndex].OutgoingHealingStatsTargets[playerIndex];
                        for (int target = 0; target < targetStats.Count; target++)
                        {   
                            OutgoingTargetAll += Parse<int>(targetStats[target][0]);
                            OutgoingTargetHealingPower += Parse<int>(targetStats[target][1]);
                            OutgoingTargetConversion += Parse<int>(targetStats[target][2]);
                            OutgoingTargetDowned += Parse<int>(targetStats[target][3]);
                        }
                        healingReport.OutgoingTargetAll = OutgoingTargetAll;
                        healingReport.OutgoingTargetHealingPower = OutgoingTargetHealingPower;
                        healingReport.OutgoingTargetConversion = OutgoingTargetConversion;
                        healingReport.OutgoingTargetDowned = OutgoingTargetDowned;
                        playerReport.healing = healingReport;
                    }
                    
                }
                report.players[playerReport.Name] = playerReport;
            }
        }

        private GameplayReport BuildGameplayReport(List<object> DmgStats)
        {
            var report = new GameplayReport();
            report.CriticalHits = Parse<int>(DmgStats[1]);
            report.CritableHits = Parse<int>(DmgStats[2]);
            report.Flanking = Parse<int>(DmgStats[4]);
            report.Glancing = Parse<int>(DmgStats[5]);
            report.ConnectedHits = Parse<int>(DmgStats[11]);
            report.Blined = Parse<int>(DmgStats[6]);
            report.Interrupted = Parse<int>(DmgStats[7]);
            report.Invulnerable = Parse<int>(DmgStats[8]);
            report.Evaded = Parse<int>(DmgStats[9]);
            report.Blocked = Parse<int>(DmgStats[10]);
            report.Killed = Parse<int>(DmgStats[12]);
            report.Downed = Parse<int>(DmgStats[13]);
            report.Wasted = Parse<float>(DmgStats[17]);
            report.WastedCount = Parse<int>(DmgStats[18]);
            report.Saved = Parse<float>(DmgStats[19]);
            report.SavedCount = Parse<int>(DmgStats[20]);
            report.WeaponSwapped = Parse<int>(DmgStats[21]);
            report.AvgDistanceToSquad = Parse<double>(DmgStats[22]);
            report.AvgDistanceToTag = Parse<double>(DmgStats[23]);
            return report;
        }
    }
}
