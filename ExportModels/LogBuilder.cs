using GW2EIBuilders;
using GW2EIBuilders.HtmlModels;
using GW2EIBuilders.HtmlModels.EXTHealing;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using Gw2LogParser.EvtcParserExtensions;
using Gw2LogParser.ExportModels.Report;

namespace Gw2LogParser.ExportModels
{
    internal class LogBuilder
    {
        private readonly ParsedEvtcLog log;
        private readonly Dictionary<long, SkillItem> usedSkills = new Dictionary<long, SkillItem>();
        private readonly Dictionary<long, Buff> usedBuffs = new Dictionary<long, Buff>();
        private readonly HashSet<DamageModifier> usedDamageMods = new HashSet<DamageModifier>();
        public bool cr = true;
        public bool light = false;
        public string[] uploadLinks = new string[] {};

        public LogBuilder(LogContainer parsedLog)
        {
            log = parsedLog.Log;
        }

        // arcDPS records reviving a downed ally under the generic "Resurrect" skill (id 1006)
        // and reports it as outgoing healing with a wildly inflated magnitude. It isn't real
        // healing output, so it's stripped from the summary's healing stats.
        private const long ResurrectSkillId = 1006;

        // Sum the Resurrect (id 1006) healing in a single healing distribution. Distribution
        // items are [isIndirect, skillId, totalHealing, ...] (see EXTHealingStatsHealingDistributionDto).
        private static long ResurrectHealingIn(EXTHealingStatsHealingDistributionDto? distribution)
        {
            long sum = 0;
            if (distribution?.Distribution == null) { return 0; }
            foreach (var item in distribution.Distribution)
            {
                if (item.Length > 2 && Convert.ToInt64(item[1]) == ResurrectSkillId)
                {
                    sum += Convert.ToInt64(item[2]);
                }
            }
            return sum;
        }

        // Pick the requested phase from a per-phase list of healing distributions and return
        // its Resurrect healing.
        private static long ResurrectHealing(List<EXTHealingStatsHealingDistributionDto>? distributionsPerPhase, int phaseIndex)
        {
            if (distributionsPerPhase == null || phaseIndex < 0 || phaseIndex >= distributionsPerPhase.Count)
            {
                return 0;
            }
            return ResurrectHealingIn(distributionsPerPhase[phaseIndex]);
        }

        internal LogDataDto BuildLogData(Version parserVersion, UploadResults uploadResults)
        {
            bool combatReplay = true;
            bool lightTheme = true;
            return LogDataDto.BuildLogData(log, combatReplay, lightTheme, parserVersion, uploadResults);
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

        // Merge boon stats from one fight into the running total, keyed by buff ID.
        private static void MergeBoons(List<BoonReport> original, List<BoonReport> report)
        {
            var byId = new Dictionary<long, BoonReport>();
            foreach (var b in original) { byId[b.Id] = b; }
            foreach (var rb in report)
            {
                if (byId.TryGetValue(rb.Id, out var ob))
                {
                    ob.Value += rb.Value;
                    ob.Uptime += rb.Uptime;
                    ob.Extended += rb.Extended;
                    ob.Wasted += rb.Wasted;
                    ob.Overstack += rb.Overstack;
                }
                else
                {
                    var clone = new BoonReport { Id = rb.Id, Value = rb.Value, Uptime = rb.Uptime, Extended = rb.Extended, Wasted = rb.Wasted, Overstack = rb.Overstack };
                    original.Add(clone);
                    byId[rb.Id] = clone;
                }
            }
        }

        public void SumPlayerStats(PlayerReport original, PlayerReport report)
        {
            original.numberOfFights++;
            // Fold this fight's subgroup into the running tally so original.Group ends up
            // being the subgroup this account was in across the most fights, not the first.
            foreach (var kv in report.GroupCounts)
            {
                original.RecordGroup(kv.Key, kv.Value);
            }
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
            // Boon Summary - merge by buff ID, not position. Logs can have different
            // PresentBoons (e.g. Alacrity present in some fights only), so a by-index
            // merge would add different boons together and misalign every later column.
            MergeBoons(original.BoonStats, report.BoonStats);
            MergeBoons(original.BoonGenSelfStats, report.BoonGenSelfStats);
            MergeBoons(original.BoonGenGroupStats, report.BoonGenGroupStats);
            MergeBoons(original.BoonGenOGroupStats, report.BoonGenOGroupStats);
            MergeBoons(original.BoonGenSquadStats, report.BoonGenSquadStats);
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
                    original.healing.IncomingBarrier += report.healing.IncomingBarrier;
                    original.healing.OutgoingBarrier += report.healing.OutgoingBarrier;
                    original.healing.OutgoingTargetBarrier += report.healing.OutgoingTargetBarrier;
                }
            }
        }

        private T Parse<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public void UpdateLogReport(LogReport report, LogDataDto data)
        {
            int alliesOutside = 0;
            long outBarrier = 0;
            for (int playerIndex = 0; playerIndex < data.Players?.Count; playerIndex++)
            {
                var player = data.Players[playerIndex];
                if (player == null || player.Group == 51)
                {
                    if (player != null) { alliesOutside++; }
                    continue;
                }
                var playerReport = new PlayerReport()
                {
                    Account = player.Acc,
                    Name = player.Name,
                    Profession = player.Profession,
                    Group = player.Group,
                    Icon = player.Icon
                };
                // Seed this account's group tally with the subgroup it was in this fight.
                playerReport.RecordGroup(player.Group);
                foreach (object[] item in player.Details.DmgDistributions[0].Distribution)
                {
                    playerReport.DamageSummary.Add(BuildSummary(item, data));
                }
                foreach (object[] item in player.Details.DmgDistributionsTaken[0].Distribution)
                {
                    playerReport.TakenSummary.Add(BuildSummary(item, data));
                }
                // Squad outgoing barrier damage (damage absorbed by enemy barrier).
                foreach (var s in playerReport.DamageSummary) { outBarrier += s.BarrierDamage; }
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
                    playerReport.Defense.Blocked = Parse<int>(phase.DefStats[playerIndex][6]);
                    playerReport.Defense.Invulned = Parse<int>(phase.DefStats[playerIndex][4]);
                    playerReport.Defense.Interrupted = Parse<int>(phase.DefStats[playerIndex][3]);
                    playerReport.Defense.Evaded = Parse<int>(phase.DefStats[playerIndex][5]);
                    playerReport.Defense.Dodges = Parse<int>(phase.DefStats[playerIndex][7]);
                    playerReport.Defense.Missed = Parse<int>(phase.DefStats[playerIndex][2]);
                    playerReport.Defense.Downed = Parse<int>(phase.DefStats[playerIndex][12]);
                    playerReport.Defense.Dead = Parse<int>(phase.DefStats[playerIndex][14]);
                    playerReport.Gameplay = BuildGameplayReport(phase.GameplayStats[playerIndex], phase.OffensiveStats[playerIndex]);
                    // Tag each boon with its buff ID (BoonStats columns follow data.Boons /
                    // PresentBoons). Dedupe by ID so multi-phase logs keep the first (full-fight) entry.
                    void AddBoons(List<BoonReport> target, GW2EIBuilders.HtmlModels.HTMLStats.BuffData buffData)
                    {
                        for (int b = 0; b < buffData.Data.Count; b++)
                        {
                            long id = (data.Boons != null && b < data.Boons.Count) ? data.Boons[b] : 0;
                            if (target.Exists(x => x.Id == id)) { continue; }
                            target.Add(new BoonReport(buffData.Data[b]) { Id = id });
                        }
                    }
                    AddBoons(playerReport.BoonStats, phase.BuffsStatContainer.BoonStats[playerIndex]);
                    AddBoons(playerReport.BoonGenSquadStats, phase.BuffsStatContainer.BoonGenSquadStats[playerIndex]);
                    AddBoons(playerReport.BoonGenSelfStats, phase.BuffsStatContainer.BoonGenSelfStats[playerIndex]);
                    AddBoons(playerReport.BoonGenOGroupStats, phase.BuffsStatContainer.BoonGenOGroupStats[playerIndex]);
                    AddBoons(playerReport.BoonGenGroupStats, phase.BuffsStatContainer.BoonGenGroupStats[playerIndex]);
                    if (data.HealingStatsExtension != null)
                    {
                        var healingReport = new HealingReport();
                        // The arcDPS healing addon counts reviving downed allies (the generic
                        // "Resurrect" skill, id 1006) as outgoing healing, and the reported value
                        // is wildly inflated (a single fight credited a DPS millions of healing,
                        // topping the leaderboard above real healers). Resurrect isn't sustained
                        // healing output, so strip its contribution out of the squad healing stats.
                        var healDetails = (data.HealingStatsExtension.PlayerHealingDetails != null
                            && playerIndex < data.HealingStatsExtension.PlayerHealingDetails.Count)
                            ? data.HealingStatsExtension.PlayerHealingDetails[playerIndex] : null;
                        long resurrectAll = ResurrectHealing(healDetails?.HealingDistributions, phaseIndex);
                        long resurrectIncoming = ResurrectHealing(healDetails?.IncomingHealingDistributions, phaseIndex);
                        long resurrectTarget = 0;
                        var targetDists = (healDetails?.HealingDistributionsTargets != null
                            && phaseIndex < healDetails.HealingDistributionsTargets.Count)
                            ? healDetails.HealingDistributionsTargets[phaseIndex] : null;
                        if (targetDists != null)
                        {
                            foreach (var td in targetDists) { resurrectTarget += ResurrectHealingIn(td); }
                        }

                        healingReport.IncomingHealed = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].IncomingHealingStats[playerIndex][0]) - (int)resurrectIncoming;
                        healingReport.IncomingHealingPower = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].IncomingHealingStats[playerIndex][1]);
                        healingReport.IncomingConversion = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].IncomingHealingStats[playerIndex][2]);
                        healingReport.IncomingDowned = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].IncomingHealingStats[playerIndex][3]);
                        healingReport.OutgoingAll = Parse<int>(data.HealingStatsExtension.HealingPhases[phaseIndex].OutgoingHealingStats[playerIndex][0]) - (int)resurrectAll;
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
                        healingReport.OutgoingTargetAll = OutgoingTargetAll - (int)resurrectTarget;
                        healingReport.OutgoingTargetHealingPower = OutgoingTargetHealingPower;
                        healingReport.OutgoingTargetConversion = OutgoingTargetConversion;
                        healingReport.OutgoingTargetDowned = OutgoingTargetDowned;
                        playerReport.healing = healingReport;
                    }
                    if (data.BarrierStatsExtension != null && playerReport.healing != null)
                    {
                        playerReport.healing.IncomingBarrier = Parse<int>(data.BarrierStatsExtension.BarrierPhases[phaseIndex].IncomingBarrierStats[playerIndex][0]);
                        playerReport.healing.OutgoingBarrier = Parse<int>(data.BarrierStatsExtension.BarrierPhases[phaseIndex].OutgoingBarrierStats[playerIndex][0]);
                        int OutgoingTargetBarrier = 0;
                        var targetStats = data.BarrierStatsExtension.BarrierPhases[phaseIndex].OutgoingBarrierStatsTargets[playerIndex];
                        for (int target = 0; target < targetStats.Count; target++)
                        {
                            OutgoingTargetBarrier += Parse<int>(targetStats[target][0]);
                        }
                        playerReport.healing.OutgoingTargetBarrier = OutgoingTargetBarrier;
                    }
                    
                }
                // Key by full identity (account|character|profession) so a player who swaps
                // character or class within the batch isn't overwritten by another row.
                report.players[playerReport.Key] = playerReport;
            }

            // Roll up per-fight aggregates for the "Fights In This Report" table.
            report.SquadSize = report.players.Count;
            report.AlliesOutsideSquad = alliesOutside;
            report.TotalEnemies = data.Targets?.Count ?? 0;
            foreach (var p in report.players.Values)
            {
                report.OutgoingDamage += p.Damage.AllDamage;
                report.IncomingDamage += p.Defense.DamageTaken;
                report.SquadBarrierAbsorbed += p.Defense.DamageBarrier;
                report.AlliesDowned += p.Defense.Downed;
                report.AlliesDead += p.Defense.Dead;
                report.AlliesRevived += p.Support.Resurrects;
                report.EnemyDowns += p.Gameplay.Downed;
                report.EnemyDeaths += p.Gameplay.Killed;
            }
            report.EnemyBarrierAbsorbed = outBarrier;
            report.DamageDelta = report.OutgoingDamage - report.IncomingDamage;
            report.BarrierDelta = report.SquadBarrierAbsorbed - report.EnemyBarrierAbsorbed;
        }

        private GameplayReport BuildGameplayReport(List<double> DmgStats, List<double> OffStats)
        {
            var report = new GameplayReport();
            report.Wasted = Parse<float>(DmgStats[0]);
            report.WastedCount = Parse<int>(DmgStats[1]);
            report.Saved = Parse<float>(DmgStats[2]);
            report.SavedCount = Parse<int>(DmgStats[3]);
            report.WeaponSwapped = Parse<int>(DmgStats[4]);
            report.AvgDistanceToSquad = Parse<double>(DmgStats[5]);
            report.AvgDistanceToTag = Parse<double>(DmgStats[6]);

            report.CriticalHits = Parse<int>(OffStats[1]);
            report.CritableHits = Parse<int>(OffStats[2]);
            report.Flanking = Parse<int>(OffStats[4]);
            report.Glancing = Parse<int>(OffStats[5]);
            report.ConnectedHits = Parse<int>(OffStats[11]);
            report.Blined = Parse<int>(OffStats[6]);
            report.Interrupted = Parse<int>(OffStats[7]);
            report.Invulnerable = Parse<int>(OffStats[8]);
            report.Evaded = Parse<int>(OffStats[9]);
            report.Blocked = Parse<int>(OffStats[10]);
            report.Killed = Parse<int>(OffStats[12]);
            report.Downed = Parse<int>(OffStats[13]);
            return report;
        }
    }
}
