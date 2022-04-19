﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.Statistics;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.GW2EIBuilders.JsonDamageModifierData;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonDamageModifierDataBuilder
    {
        private static JsonDamageModifierItem BuildJsonDamageModifierItem(DamageModifierStat extraData)
        {
            var jsonDamageModifierItem = new JsonDamageModifierItem();
            jsonDamageModifierItem.HitCount = extraData.HitCount;
            jsonDamageModifierItem.TotalHitCount = extraData.TotalHitCount;
            jsonDamageModifierItem.DamageGain = extraData.DamageGain;
            jsonDamageModifierItem.TotalDamage = extraData.TotalDamage;
            return jsonDamageModifierItem;
        }

        private static JsonDamageModifierData BuildJsonDamageModifierData(int ID, List<JsonDamageModifierItem> data)
        {
            var jsonDamageModifierData = new JsonDamageModifierData();
            jsonDamageModifierData.Id = ID;
            jsonDamageModifierData.DamageModifiers = data;
            return jsonDamageModifierData;
        }


        public static List<JsonDamageModifierData> GetDamageModifiers(List<IReadOnlyDictionary<string, DamageModifierStat>> damageModDicts, ParsedLog log, Dictionary<string, JsonLog.DamageModDesc> damageModDesc)
        {
            var dict = new Dictionary<int, List<JsonDamageModifierItem>>();
            foreach (IReadOnlyDictionary<string, DamageModifierStat> damageModDict in damageModDicts)
            {
                foreach (string key in damageModDict.Keys)
                {
                    DamageModifier dMod = log.DamageModifiers.DamageModifiersByName[key];
                    int iKey = dMod.ID;
                    string nKey = "d" + iKey;
                    if (!damageModDesc.ContainsKey(nKey))
                    {
                        damageModDesc[nKey] = JsonLogBuilder.BuildDamageModDesc(dMod);
                    }
                    if (dict.TryGetValue(iKey, out List<JsonDamageModifierItem> list))
                    {
                        list.Add(BuildJsonDamageModifierItem(damageModDict[key]));
                    }
                    else
                    {
                        dict[iKey] = new List<JsonDamageModifierItem>
                        {
                            BuildJsonDamageModifierItem(damageModDict[key])
                        };
                    }
                }
            }

            var res = new List<JsonDamageModifierData>();
            foreach (KeyValuePair<int, List<JsonDamageModifierItem>> pair in dict)
            {
                res.Add(BuildJsonDamageModifierData(pair.Key, pair.Value));
            }
            return res;
        }

        public static List<JsonDamageModifierData>[] GetDamageModifiersTarget(AbstractSingleActor player, ParsedLog log, Dictionary<string, JsonLog.DamageModDesc> damageModDesc, IReadOnlyList<PhaseData> phases)
        {
            var res = new List<JsonDamageModifierData>[log.FightData.Logic.Targets.Count];
            for (int i = 0; i < log.FightData.Logic.Targets.Count; i++)
            {
                AbstractSingleActor tar = log.FightData.Logic.Targets[i];
                res[i] = GetDamageModifiers(phases.Select(x => player.GetDamageModifierStats(tar, log, x.Start, x.End)).ToList(), log, damageModDesc); ;
            }
            return res;
        }
    }
}
