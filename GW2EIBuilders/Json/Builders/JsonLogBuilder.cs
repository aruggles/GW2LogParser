﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.Mechanics;
using Gw2LogParser.Parser.Data.Events.Mechanics;
using Gw2LogParser.Parser.Data.Events.MetaData;
using Gw2LogParser.Parser.Data.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.GW2EIBuilders.JsonLog;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class JsonLogBuilder
    {
        internal static SkillDesc BuildSkillDesc(Skill item, ParsedLog log)
        {
            var skillDesc = new SkillDesc
            {
                Name = item.Name,
                AutoAttack = item.AA,
                Icon = item.Icon,
                CanCrit = Skill.CanCrit(item.ID, log.LogData.GW2Build),
                IsSwap = item.IsSwap,
                IsNotAccurate = log.SkillData.IsNotAccurate(item.ID),
                ConversionBasedHealing = log.CombatData.HasEXTHealing ? log.CombatData.EXTHealingCombatData.GetHealingType(item, log) == Parser.Extensions.HealingStatsExtensionHandler.EXTHealingType.ConversionBased : false,
                HybridHealing = log.CombatData.HasEXTHealing ? log.CombatData.EXTHealingCombatData.GetHealingType(item, log) == Parser.Extensions.HealingStatsExtensionHandler.EXTHealingType.Hybrid : false
            };
            return skillDesc;
        }

        internal static BuffDesc BuildBuffDesc(Buff item, ParsedLog log)
        {
            var buffDesc = new BuffDesc
            {
                Name = item.Name,
                Icon = item.Link,
                Stacking = item.Type == Buff.BuffType.Intensity,
                ConversionBasedHealing = log.CombatData.HasEXTHealing ? log.CombatData.EXTHealingCombatData.GetHealingType(item, log) == Parser.Extensions.HealingStatsExtensionHandler.EXTHealingType.ConversionBased : false,
                HybridHealing = log.CombatData.HasEXTHealing ? log.CombatData.EXTHealingCombatData.GetHealingType(item, log) == Parser.Extensions.HealingStatsExtensionHandler.EXTHealingType.Hybrid : false
            };
            BuffInfoEvent buffInfoEvent = log.CombatData.GetBuffInfoEvent(item.ID);
            if (buffInfoEvent != null)
            {
                var descriptions = new List<string>(){
                        "Max Stack(s) " + buffInfoEvent.MaxStacks
                    };
                if (buffInfoEvent.DurationCap > 0)
                {
                    descriptions.Add("Duration Cap: " + Math.Round(buffInfoEvent.DurationCap / 1000.0, 3) + " seconds");
                }
                foreach (BuffFormula formula in buffInfoEvent.Formulas)
                {
                    if (formula.IsConditional)
                    {
                        continue;
                    }
                    string desc = formula.GetDescription(false, log.Buffs.BuffsByIds);
                    if (desc.Length > 0)
                    {
                        descriptions.Add(desc);
                    }
                }
                buffDesc.Descriptions = descriptions;
            }
            return buffDesc;
        }

        internal static DamageModDesc BuildDamageModDesc(DamageModifier item)
        {
            var damageModDesc = new DamageModDesc
            {
                Name = item.Name,
                Icon = item.Icon,
                Description = item.Tooltip,
                NonMultiplier = !item.Multiplier,
                SkillBased = item.SkillBased,
                Approximate = item.Approximate
            };
            return damageModDesc;
        }

        public static JsonLog BuildJsonLog(ParsedLog log, RawFormatSettings settings, Version parserVersion, string[] uploadLinks)
        {
            var jsonLog = new JsonLog();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Meta Data");
            jsonLog.TriggerID = log.FightData.TriggerID;
            jsonLog.FightName = log.FightData.GetFightName(log);
            jsonLog.FightIcon = log.FightData.Logic.Icon;
            jsonLog.EliteInsightsVersion = parserVersion.ToString();
            jsonLog.ArcVersion = log.LogData.ArcVersion;
            jsonLog.RecordedBy = log.LogData.PoVName;
            jsonLog.TimeStart = log.LogData.LogStart;
            jsonLog.TimeEnd = log.LogData.LogEnd;
            jsonLog.TimeStartStd = log.LogData.LogStartStd;
            jsonLog.TimeEndStd = log.LogData.LogEndStd;
            jsonLog.Duration = log.FightData.DurationString;
            jsonLog.Success = log.FightData.Success;
            jsonLog.GW2Build = log.LogData.GW2Build;
            jsonLog.UploadLinks = uploadLinks;
            jsonLog.Language = log.LogData.Language;
            jsonLog.LanguageID = (byte)log.LogData.LanguageID;
            jsonLog.IsCM = log.FightData.IsCM;
            var personalBuffs = new Dictionary<string, HashSet<long>>();
            var skillMap = new Dictionary<string, SkillDesc>();
            var buffMap = new Dictionary<string, BuffDesc>();
            var damageModMap = new Dictionary<string, DamageModDesc>();

            if (log.StatisticsHelper.PresentFractalInstabilities.Any())
            {
                var presentFractalInstabilities = new List<long>();
                foreach (Buff fractalInstab in log.StatisticsHelper.PresentFractalInstabilities)
                {
                    presentFractalInstabilities.Add(fractalInstab.ID);
                    if (!buffMap.ContainsKey("b" + fractalInstab.ID))
                    {
                        buffMap["b" + fractalInstab.ID] = BuildBuffDesc(fractalInstab, log);
                    }
                }
                jsonLog.PresentFractalInstabilities = presentFractalInstabilities;
            }
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Mechanics");
            MechanicData mechanicData = log.MechanicData;
            var mechanicLogs = new List<MechanicEvent>();
            foreach (List<MechanicEvent> mLog in mechanicData.GetAllMechanics(log))
            {
                mechanicLogs.AddRange(mLog);
            }
            if (mechanicLogs.Any())
            {
                jsonLog.Mechanics = JsonMechanicsBuilder.GetJsonMechanicsList(mechanicLogs);
            }
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Phases");
            jsonLog.Phases = log.FightData.GetNonDummyPhases(log).Select(x => JsonPhaseBuilder.BuildJsonPhase(x, log)).ToList();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Targets");
            jsonLog.Targets = log.FightData.Logic.Targets.Select(x => JsonNPCBuilder.BuildJsonNPC(x, log, settings, skillMap, buffMap)).ToList();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Players");
            jsonLog.Players = log.Friendlies.Select(x => JsonPlayerBuilder.BuildJsonPlayer(x, log, settings, skillMap, buffMap, damageModMap, personalBuffs)).ToList();
            //
            if (log.LogData.LogErrors.Any())
            {
                jsonLog.LogErrors = new List<string>(log.LogData.LogErrors);
            }
            if (log.LogData.UsedExtensions.Any())
            {
                jsonLog.UsedExtensions = log.LogData.UsedExtensions.Select(x => {
                    return new ExtensionDesc()
                    {
                        Name = x.Name,
                        Version = x.Version,
                        Signature = x.Signature,
                        Revision = x.Revision
                    };
                }).ToList();
            }
            //
            jsonLog.PersonalBuffs = personalBuffs.ToDictionary(x => x.Key, x => (IReadOnlyCollection<long>)x.Value);
            jsonLog.SkillMap = skillMap;
            jsonLog.BuffMap = buffMap;
            jsonLog.DamageModMap = damageModMap;
            //
            if (log.CanCombatReplay)
            {
                jsonLog.CombatReplayMetaData = JsonCombatReplayMetaDataBuilder.BuildJsonCombatReplayMetaData(log, settings);
            }
            return jsonLog;
        }
    }
}
