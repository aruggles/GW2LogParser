﻿using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.GW2EIBuilders.JsonBuffsUptime;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonBuffsUptimeBuilder
    {
        private static Dictionary<string, double> ConvertKeys(Dictionary<AbstractSingleActor, double> toConvert)
        {
            var res = new Dictionary<string, double>();
            foreach (KeyValuePair<AbstractSingleActor, double> pair in toConvert)
            {
                res[pair.Key.Character] = pair.Value;
            }
            return res;
        }

        public static JsonBuffsUptimeData BuildJsonBuffsUptimeData(FinalActorBuffs buffs, FinalBuffsDictionary buffsDictionary)
        {
            var jsonBuffsUptimeData = new JsonBuffsUptimeData
            {
                Uptime = buffs.Uptime,
                Presence = buffs.Presence,
                Generated = ConvertKeys(buffsDictionary.Generated),
                Overstacked = ConvertKeys(buffsDictionary.Overstacked),
                Wasted = ConvertKeys(buffsDictionary.Wasted),
                UnknownExtended = ConvertKeys(buffsDictionary.UnknownExtension),
                ByExtension = ConvertKeys(buffsDictionary.Extension),
                Extended = ConvertKeys(buffsDictionary.Extended)
            };
            return jsonBuffsUptimeData;
        }


        public static JsonBuffsUptime BuildJsonBuffsUptime(AbstractSingleActor actor, long buffID, ParsedLog log, RawFormatSettings settings, List<JsonBuffsUptimeData> buffData, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var jsonBuffsUptime = new JsonBuffsUptime
            {
                Id = buffID,
                BuffData = buffData
            };
            if (!buffDesc.ContainsKey("b" + buffID))
            {
                buffDesc["b" + buffID] = JsonLogBuilder.BuildBuffDesc(log.Buffs.BuffsByIds[buffID], log);
            }
            if (settings.RawFormatTimelineArrays)
            {
                jsonBuffsUptime.States = GetBuffStates(actor.GetBuffGraphs(log)[buffID]);
            }
            return jsonBuffsUptime;
        }
        public static List<int[]> GetBuffStates(BuffsGraphModel bgm)
        {
            if (bgm == null || bgm.BuffChart.Count == 0)
            {
                return null;
            }
            var res = bgm.BuffChart.Select(x => new int[2] { (int)x.Start, (int)x.Value }).ToList();
            return res.Count > 0 ? res : null;
        }
    }
}
