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
    public class LogBuilder
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

        internal LogDataDto BuildLogData()
        {
            GeneralStatistics statistics = log.Statistics;
            log.UpdateProgressWithCancellationCheck("HTML: building Log Data");
            var logData = new LogDataDto
            {
                EncounterStart = log.LogData.LogStartStd,
                EncounterEnd = log.LogData.LogEndStd,
                ArcVersion = log.LogData.ArcVersion,
                Gw2Build = log.LogData.GW2Build,
                FightID = log.FightData.TriggerID,
                Parser = "Elite Insights " + log.ParserVersion.ToString(),
                RecordedBy = log.LogData.PoVName,
                UploadLinks = uploadLinks.ToList()
            };
            if (cr)
            {
                logData.CrData = new CombatReplayDto(log);
            }
            log.UpdateProgressWithCancellationCheck("HTML: building Players");
            foreach (Player player in log.PlayerList)
            {
                logData.HasCommander = logData.HasCommander || player.HasCommanderTag;
                logData.Players.Add(new PlayerDto(player, log, cr, ActorDetailsDto.BuildPlayerData(log, player, usedSkills, usedBuffs)));
            }

            log.UpdateProgressWithCancellationCheck("HTML: building Enemies");
            foreach (AbstractActor enemy in log.MechanicData.GetEnemyList(log, 0))
            {
                logData.Enemies.Add(new EnemyDto() { Name = enemy.Character });
            }

            log.UpdateProgressWithCancellationCheck("HTML: building Targets");
            foreach (NPC target in log.FightData.Logic.Targets)
            {
                var targetDto = new TargetDto(target, log, cr, ActorDetailsDto.BuildTargetData(log, target, usedSkills, usedBuffs, cr));
                logData.Targets.Add(targetDto);
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Skill/Buff dictionaries");
            Dictionary<string, List<Buff>> persBuffDict = BuildPersonalBoonData(log, logData.PersBuffs, usedBuffs);
            Dictionary<string, List<DamageModifier>> persDamageModDict = BuildPersonalDamageModData(log, logData.DmgModifiersPers, usedDamageMods);
            var allDamageMods = new HashSet<string>();
            foreach (Player p in log.PlayerList)
            {
                allDamageMods.UnionWith(p.GetPresentDamageModifier(log));
            }
            var commonDamageModifiers = new List<DamageModifier>();
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(ParserHelper.Source.Common, out List<DamageModifier> list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        commonDamageModifiers.Add(dMod);
                        logData.DmgModifiersCommon.Add(dMod.ID);
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(ParserHelper.Source.FightSpecific, out list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        commonDamageModifiers.Add(dMod);
                        logData.DmgModifiersCommon.Add(dMod.ID);
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            var itemDamageModifiers = new List<DamageModifier>();
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(ParserHelper.Source.Item, out list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        itemDamageModifiers.Add(dMod);
                        logData.DmgModifiersItem.Add(dMod.ID);
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            foreach (Buff boon in statistics.PresentBoons)
            {
                logData.Boons.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentConditions)
            {
                logData.Conditions.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentOffbuffs)
            {
                logData.OffBuffs.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentSupbuffs)
            {
                logData.SupBuffs.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentDefbuffs)
            {
                logData.DefBuffs.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentFractalInstabilities)
            {
                logData.FractalInstabilities.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Phases");
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phaseData = phases[i];
                var phaseDto = new PhaseDto(phaseData, phases, log)
                {
                    DpsStats = PhaseDto.BuildDPSData(log, i),
                    DpsStatsTargets = PhaseDto.BuildDPSTargetsData(log, i),
                    DmgStatsTargets = PhaseDto.BuildDMGStatsTargetsData(log, i),
                    DmgStats = PhaseDto.BuildDMGStatsData(log, i),
                    DefStats = PhaseDto.BuildDefenseData(log, i),
                    SupportStats = PhaseDto.BuildSupportData(log, i),
                    //
                    BoonStats = GW2EIBuilders.BuffData.BuildBuffUptimeData(log, statistics.PresentBoons, i),
                    OffBuffStats = GW2EIBuilders.BuffData.BuildBuffUptimeData(log, statistics.PresentOffbuffs, i),
                    SupBuffStats = GW2EIBuilders.BuffData.BuildBuffUptimeData(log, statistics.PresentSupbuffs, i),
                    DefBuffStats = GW2EIBuilders.BuffData.BuildBuffUptimeData(log, statistics.PresentDefbuffs, i),
                    PersBuffStats = GW2EIBuilders.BuffData.BuildPersonalBuffUptimeData(log, persBuffDict, i),
                    BoonGenSelfStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Self),
                    BoonGenGroupStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Group),
                    BoonGenOGroupStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.OffGroup),
                    BoonGenSquadStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Squad),
                    OffBuffGenSelfStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Self),
                    OffBuffGenGroupStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Group),
                    OffBuffGenOGroupStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.OffGroup),
                    OffBuffGenSquadStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Squad),
                    SupBuffGenSelfStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, i, BuffEnum.Self),
                    SupBuffGenGroupStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, i, BuffEnum.Group),
                    SupBuffGenOGroupStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, i, BuffEnum.OffGroup),
                    SupBuffGenSquadStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, i, BuffEnum.Squad),
                    DefBuffGenSelfStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Self),
                    DefBuffGenGroupStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Group),
                    DefBuffGenOGroupStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.OffGroup),
                    DefBuffGenSquadStats = GW2EIBuilders.BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Squad),
                    //
                    BoonActiveStats = GW2EIBuilders.BuffData.BuildActiveBuffUptimeData(log, statistics.PresentBoons, i),
                    OffBuffActiveStats = GW2EIBuilders.BuffData.BuildActiveBuffUptimeData(log, statistics.PresentOffbuffs, i),
                    SupBuffActiveStats = GW2EIBuilders.BuffData.BuildActiveBuffUptimeData(log, statistics.PresentSupbuffs, i),
                    DefBuffActiveStats = GW2EIBuilders.BuffData.BuildActiveBuffUptimeData(log, statistics.PresentDefbuffs, i),
                    PersBuffActiveStats = GW2EIBuilders.BuffData.BuildActivePersonalBuffUptimeData(log, persBuffDict, i),
                    BoonGenActiveSelfStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Self),
                    BoonGenActiveGroupStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Group),
                    BoonGenActiveOGroupStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.OffGroup),
                    BoonGenActiveSquadStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Squad),
                    OffBuffGenActiveSelfStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Self),
                    OffBuffGenActiveGroupStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Group),
                    OffBuffGenActiveOGroupStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.OffGroup),
                    OffBuffGenActiveSquadStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Squad),
                    SupBuffGenActiveSelfStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, i, BuffEnum.Self),
                    SupBuffGenActiveGroupStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, i, BuffEnum.Group),
                    SupBuffGenActiveOGroupStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, i, BuffEnum.OffGroup),
                    SupBuffGenActiveSquadStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, i, BuffEnum.Squad),
                    DefBuffGenActiveSelfStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Self),
                    DefBuffGenActiveGroupStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Group),
                    DefBuffGenActiveOGroupStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.OffGroup),
                    DefBuffGenActiveSquadStats = GW2EIBuilders.BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Squad),
                    //
                    DmgModifiersCommon = DamageModData.BuildDmgModifiersData(log, i, commonDamageModifiers),
                    DmgModifiersItem = DamageModData.BuildDmgModifiersData(log, i, itemDamageModifiers),
                    DmgModifiersPers = DamageModData.BuildPersonalDmgModifiersData(log, i, persDamageModDict),
                    TargetsCondiStats = new List<List<Gw2LogParser.GW2EIBuilders.BuffData>>(),
                    TargetsCondiTotals = new List<Gw2LogParser.GW2EIBuilders.BuffData>(),
                    TargetsBoonTotals = new List<Gw2LogParser.GW2EIBuilders.BuffData>(),
                    MechanicStats = MechanicDto.BuildPlayerMechanicData(log, i),
                    EnemyMechanicStats = MechanicDto.BuildEnemyMechanicData(log, i)
                };
                foreach (NPC target in phaseData.Targets)
                {
                    phaseDto.TargetsCondiStats.Add(GW2EIBuilders.BuffData.BuildTargetCondiData(log, i, target));
                    phaseDto.TargetsCondiTotals.Add(GW2EIBuilders.BuffData.BuildTargetCondiUptimeData(log, i, target));
                    phaseDto.TargetsBoonTotals.Add(HasBoons(log, i, target) ? GW2EIBuilders.BuffData.BuildTargetBoonData(log, i, target) : null);
                }
                logData.Phases.Add(phaseDto);
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Meta Data");
            logData.EncounterDuration = log.FightData.DurationString;
            logData.Success = log.FightData.Success;
            logData.Wvw = log.FightData.Logic.Mode == FightLogic.ParseMode.WvW;
            logData.Targetless = log.FightData.Logic.Targetless;
            logData.FightName = log.FightData.GetFightName(log);
            logData.FightIcon = log.FightData.Logic.Icon;
            logData.LightTheme = light;
            logData.SingleGroup = log.PlayerList.Where(x => !x.IsFakeActor).Select(x => x.Group).Distinct().Count() == 1;
            logData.NoMechanics = log.FightData.Logic.HasNoFightSpecificMechanics;
            if (log.LogData.LogErrors.Count > 0)
            {
                logData.LogErrors = new List<string>(log.LogData.LogErrors);
            }
            //
            SkillDto.AssembleSkills(usedSkills.Values, logData.SkillMap, log.SkillData);
            DamageModDto.AssembleDamageModifiers(usedDamageMods, logData.DamageModMap);
            BuffDto.AssembleBoons(usedBuffs.Values, logData.BuffMap, log);
            MechanicDto.BuildMechanics(log.MechanicData.GetPresentMechanics(log, 0), logData.MechanicMap);
            return logData;
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
            summaryItem.Max = (int)Convert.ChangeType(item[4], typeof(int));
            summaryItem.Casts = (int)Convert.ChangeType(item[5], typeof(int));
            summaryItem.Hits = (int)Convert.ChangeType(item[6], typeof(int));
            summaryItem.Wasted = (float)Convert.ChangeType(item[10], typeof(float));
            summaryItem.Saved = (float)Convert.ChangeType(item[11], typeof(float));
            summaryItem.HitsPerCast = (summaryItem.Casts == 0) ? 0 : summaryItem.Hits / summaryItem.Casts;
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
            original.HitsPerCast += (original.Casts == 0) ? 0 : original.Hits / original.Casts;
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
            original.Damage.TargetCondi = report.Damage.TargetCondi;

            original.Support.BoonStrips += report.Support.BoonStrips;
            original.Support.CleanseOnOther += report.Support.CleanseOnOther;
            original.Support.CleanseOnSelf += report.Support.CleanseOnSelf;
            original.Support.BoonStrips += report.Support.BoonStrips;
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
            original.Gameplay.Saved += report.Gameplay.Saved;
            original.Gameplay.Wasted += report.Gameplay.Wasted;
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

                for (int phaseIndex = 0; phaseIndex < data.Phases.Count; phaseIndex++)
                {
                    var phase = data.Phases[phaseIndex];
                    playerReport.TimeInCombat = phase.PlayerActiveTimes[playerIndex];


                    playerReport.Damage.AllDamage = Parse<long>(phase.DpsStats[playerIndex][0]);
                    playerReport.Damage.Power = Parse<long>(phase.DpsStats[playerIndex][1]);
                    playerReport.Damage.Condi = Parse<long>(phase.DpsStats[playerIndex][2]);
                    playerReport.Damage.TargetDamage = Parse<long>(phase.DpsStatsTargets[playerIndex][0][0]);
                    playerReport.Damage.TargetPower = Parse<long>(phase.DpsStatsTargets[playerIndex][0][1]);
                    playerReport.Damage.TargetCondi = Parse<long>(phase.DpsStatsTargets[playerIndex][0][2]);

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
                }
                report.players[playerReport.Name] = playerReport;
            }
        }

        private static Dictionary<string, List<Buff>> BuildPersonalBoonData(ParsedLog log, Dictionary<string, List<long>> dict, Dictionary<long, Buff> usedBuffs)
        {
            var boonsBySpec = new Dictionary<string, List<Buff>>();
            // Collect all personal buffs by spec
            foreach (KeyValuePair<string, List<Player>> pair in log.PlayerListBySpec)
            {
                List<Player> players = pair.Value;
                var specBoonIds = new HashSet<long>(log.Buffs.GetRemainingBuffsList(pair.Key).Select(x => x.ID));
                var boonToUse = new HashSet<Buff>();
                foreach (Player player in players)
                {
                    for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
                    {
                        Dictionary<long, FinalPlayerBuffs> boons = player.GetBuffs(log, i, BuffEnum.Self);
                        foreach (Buff boon in log.Statistics.PresentPersonalBuffs[player])
                        {
                            if (boons.TryGetValue(boon.ID, out FinalPlayerBuffs uptime))
                            {
                                if (uptime.Uptime > 0 && specBoonIds.Contains(boon.ID))
                                {
                                    boonToUse.Add(boon);
                                }
                            }
                        }
                    }
                }
                boonsBySpec[pair.Key] = boonToUse.ToList();
            }
            foreach (KeyValuePair<string, List<Buff>> pair in boonsBySpec)
            {
                dict[pair.Key] = new List<long>();
                foreach (Buff boon in pair.Value)
                {
                    dict[pair.Key].Add(boon.ID);
                    usedBuffs[boon.ID] = boon;
                }
            }
            return boonsBySpec;
        }

        private static Dictionary<string, List<DamageModifier>> BuildPersonalDamageModData(ParsedLog log, Dictionary<string, List<long>> dict, HashSet<DamageModifier> usedDamageMods)
        {
            var damageModBySpecs = new Dictionary<string, List<DamageModifier>>();
            // Collect all personal damage mods by spec
            foreach (KeyValuePair<string, List<Player>> pair in log.PlayerListBySpec)
            {
                var specDamageModsName = new HashSet<string>(log.DamageModifiers.GetModifiersPerProf(pair.Key).Select(x => x.Name));
                var damageModsToUse = new HashSet<DamageModifier>();
                foreach (Player player in pair.Value)
                {
                    var presentDamageMods = new HashSet<string>(player.GetPresentDamageModifier(log).Intersect(specDamageModsName));
                    foreach (string name in presentDamageMods)
                    {
                        damageModsToUse.Add(log.DamageModifiers.DamageModifiersByName[name]);
                    }
                }
                damageModBySpecs[pair.Key] = damageModsToUse.ToList();
            }
            foreach (KeyValuePair<string, List<DamageModifier>> pair in damageModBySpecs)
            {
                dict[pair.Key] = new List<long>();
                foreach (DamageModifier mod in pair.Value)
                {
                    dict[pair.Key].Add(mod.ID);
                    usedDamageMods.Add(mod);
                }
            }
            return damageModBySpecs;
        }

        private static bool HasBoons(ParsedLog log, int phaseIndex, NPC target)
        {
            Dictionary<long, FinalBuffs> conditions = target.GetBuffs(log, phaseIndex);
            foreach (Buff boon in log.Statistics.PresentBoons)
            {
                if (conditions.TryGetValue(boon.ID, out FinalBuffs uptime))
                {
                    if (uptime.Uptime > 0.0)
                    {
                        return true;
                    }
                }
            }
            return false;
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
            report.Wasted = Parse<float>(DmgStats[12]);
            report.Saved = Parse<float>(DmgStats[14]);
            report.WeaponSwapped = Parse<int>(DmgStats[16]);
            report.AvgDistanceToSquad = Parse<double>(DmgStats[17]);
            report.AvgDistanceToTag = Parse<double>(DmgStats[18]);
            return report;
        }
    }
}
