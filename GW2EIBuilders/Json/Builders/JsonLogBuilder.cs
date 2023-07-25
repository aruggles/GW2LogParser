﻿using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using Gw2LogParser.EvtcParserExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.GW2EIBuilders.JsonLog;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class JsonLogBuilder
    {
        internal static SkillDesc BuildSkillDesc(SkillItem skill, ParsedLog log)
        {
            var skillDesc = new SkillDesc
            {
                Name = skill.Name,
                AutoAttack = skill.AA,
                Icon = skill.Icon,
                CanCrit = SkillItem.CanCrit(skill.ID, log.LogData.GW2Build),
                IsSwap = skill.IsSwap,
                IsInstantCast = log.CombatData.GetInstantCastData(skill.ID).Any(),
                IsNotAccurate = log.SkillData.IsNotAccurate(skill.ID),
                IsGearProc = log.SkillData.IsGearProc(skill.ID),
                IsTraitProc = log.SkillData.IsTraitProc(skill.ID),
                ConversionBasedHealing = log.CombatData.HasEXTHealing && log.CombatData.EXTHealingCombatData.GetHealingType(skill, log) == HealingStatsExtensionHandler.EXTHealingType.ConversionBased,
                HybridHealing = log.CombatData.HasEXTHealing && log.CombatData.EXTHealingCombatData.GetHealingType(skill, log) == HealingStatsExtensionHandler.EXTHealingType.Hybrid
            };
            return skillDesc;
        }

        internal static BuffDesc BuildBuffDesc(Buff buff, ParsedLog log)
        {
            var buffDesc = new BuffDesc
            {
                Name = buff.Name,
                Icon = buff.Link,
                Stacking = buff.Type == Buff.BuffType.Intensity,
                ConversionBasedHealing = log.CombatData.HasEXTHealing ? log.CombatData.EXTHealingCombatData.GetHealingType(buff, log) == HealingStatsExtensionHandler.EXTHealingType.ConversionBased : false,
                HybridHealing = log.CombatData.HasEXTHealing ? log.CombatData.EXTHealingCombatData.GetHealingType(buff, log) == HealingStatsExtensionHandler.EXTHealingType.Hybrid : false
            };
            BuffInfoEvent buffInfoEvent = log.CombatData.GetBuffInfoEvent(buff.ID);
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
                    string desc = formula.GetDescription(false, log.Buffs.BuffsByIds, buff);
                    if (desc.Length > 0)
                    {
                        descriptions.Add(desc);
                    }
                }
                buffDesc.Descriptions = descriptions;
            }
            return buffDesc;
        }

        internal static DamageModDesc BuildDamageModDesc(DamageModifier damageModifier)
        {
            var damageModDesc = new DamageModDesc
            {
                Name = damageModifier.Name,
                Icon = damageModifier.Icon,
                Description = damageModifier.Tooltip,
                NonMultiplier = !damageModifier.Multiplier,
                SkillBased = damageModifier.SkillBased,
                Approximate = damageModifier.Approximate
            };
            return damageModDesc;
        }

        public static JsonLog BuildJsonLog(ParsedLog log, RawFormatSettings settings, Version parserVersion, string[] uploadLinks)
        {
            var jsonLog = new JsonLog();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Meta Data");
            jsonLog.TriggerID = log.FightData.TriggerID;
            jsonLog.EIEncounterID = log.FightData.Logic.EncounterID;
            jsonLog.FightName = log.FightData.FightName;
            jsonLog.FightIcon = log.FightData.Logic.Icon;
            jsonLog.EliteInsightsVersion = parserVersion.ToString();
            jsonLog.ArcVersion = log.LogData.ArcVersion;
            jsonLog.RecordedBy = log.LogData.PoVName;
            jsonLog.TimeStart = log.LogData.LogStart;
            jsonLog.TimeEnd = log.LogData.LogEnd;
            jsonLog.TimeStartStd = log.LogData.LogStartStd;
            jsonLog.TimeEndStd = log.LogData.LogEndStd;
            jsonLog.Duration = log.FightData.DurationString;
            jsonLog.DurationMS = log.FightData.FightDuration;
            jsonLog.LogStartOffset = log.FightData.FightStartOffset;
            jsonLog.Success = log.FightData.Success;
            jsonLog.GW2Build = log.LogData.GW2Build;
            jsonLog.UploadLinks = uploadLinks;
            jsonLog.Language = log.LogData.Language;
            jsonLog.LanguageID = (byte)log.LogData.LanguageID;
            jsonLog.FractalScale = log.CombatData.GetFractalScaleEvent() != null ? log.CombatData.GetFractalScaleEvent().Scale : 0;
            jsonLog.IsCM = log.FightData.IsCM;
            var personalBuffs = new Dictionary<string, HashSet<long>>();
            var skillMap = new Dictionary<string, SkillDesc>();
            var buffMap = new Dictionary<string, BuffDesc>();
            var damageModMap = new Dictionary<string, DamageModDesc>();

            if (log.FightData.Logic.GetInstanceBuffs(log).Any())
            {
                var presentFractalInstabilities = new List<long>();
                var presentInstanceBuffs = new List<long[]>();
                foreach ((Buff instanceBuff, int stack) in log.FightData.Logic.GetInstanceBuffs(log))
                {
                    if (!buffMap.ContainsKey("b" + instanceBuff.ID))
                    {
                        buffMap["b" + instanceBuff.ID] = BuildBuffDesc(instanceBuff, log);
                    }
                    if (instanceBuff.Source == ParserHelper.Source.FractalInstability)
                    {
                        presentFractalInstabilities.Add(instanceBuff.ID);
                    }
                    presentInstanceBuffs.Add(new long[] { instanceBuff.ID, stack });
                }
                jsonLog.PresentFractalInstabilities = presentFractalInstabilities;
                jsonLog.PresentInstanceBuffs = presentInstanceBuffs;
            }
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Mechanics");
            MechanicData mechanicData = log.MechanicData;
            IReadOnlyCollection<Mechanic> presentMechanics = log.MechanicData.GetPresentMechanics(log, log.FightData.FightStart, log.FightData.FightEnd);
            if (presentMechanics.Any())
            {
                jsonLog.Mechanics = JsonMechanicsBuilder.GetJsonMechanicsList(log, mechanicData, presentMechanics);
            }
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Phases");
            jsonLog.Phases = log.FightData.GetPhases(log).Select(x => JsonPhaseBuilder.BuildJsonPhase(x, log)).ToList();
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
                var usedExtensions = new List<ExtensionDesc>();
                foreach (AbstractExtensionHandler extension in log.LogData.UsedExtensions)
                {
                    var set = new HashSet<string>();
                    if (log.LogData.PoV != null)
                    {
                        set.Add(log.FindActor(log.LogData.PoV).Character);
                        foreach (AgentItem agent in extension.RunningExtension)
                        {
                            set.Add(log.FindActor(agent).Character);
                        }
                    }
                    usedExtensions.Add(new ExtensionDesc()
                    {
                        Name = extension.Name,
                        Version = extension.Version,
                        Signature = extension.Signature,
                        Revision = extension.Revision,
                        RunningExtension = set.ToList()
                    });
                }
                jsonLog.UsedExtensions = usedExtensions;
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
