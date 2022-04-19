﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.GW2EIBuilders.JsonBuffsUptime;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonNPCBuilder
    {
        public static JsonNPC BuildJsonNPC(AbstractSingleActor npc, ParsedLog log, RawFormatSettings settings, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var jsonNPC = new JsonNPC();
            JsonActorBuilder.FillJsonActor(jsonNPC, npc, log, settings, skillDesc, buffDesc);
            IReadOnlyList<PhaseData> phases = log.FightData.GetNonDummyPhases(log);
            //
            jsonNPC.Id = npc.ID;
            IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(npc.AgentItem);
            jsonNPC.FirstAware = (int)npc.FirstAware;
            jsonNPC.LastAware = (int)npc.LastAware;
            jsonNPC.EnemyPlayer = npc is PlayerNonSquad;
            double hpLeft = 100.0;
            if (log.FightData.Success)
            {
                hpLeft = 0;
            }
            else
            {
                if (hpUpdates.Count > 0)
                {
                    hpLeft = hpUpdates.Last().HPPercent;
                }
            }
            jsonNPC.HealthPercentBurned = 100.0 - hpLeft;
            jsonNPC.FinalHealth = (int)Math.Round(jsonNPC.TotalHealth * hpLeft / 100.0);
            //
            jsonNPC.Buffs = GetNPCJsonBuffsUptime(npc, log, settings, buffDesc);
            // Breakbar
            if (settings.RawFormatTimelineArrays)
            {
                jsonNPC.BreakbarPercents = npc.GetBreakbarPercentUpdates(log).Select(x => new double[2] { x.Start, x.Value }).ToList();
            }
            return jsonNPC;
        }

        private static List<JsonBuffsUptime> GetNPCJsonBuffsUptime(AbstractSingleActor npc, ParsedLog log, RawFormatSettings settings, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<JsonBuffsUptime>();
            IReadOnlyList<PhaseData> phases = log.FightData.GetNonDummyPhases(log);
            var buffs = phases.Select(x => npc.GetBuffs(ParserHelper.BuffEnum.Self, log, x.Start, x.End)).ToList();
            foreach (KeyValuePair<long, FinalActorBuffs> pair in buffs[0])
            {
                var data = new List<JsonBuffsUptimeData>();
                for (int i = 0; i < phases.Count; i++)
                {
                    PhaseData phase = phases[i];
                    Dictionary<long, FinalBuffsDictionary> buffsDictionary = npc.GetBuffsDictionary(log, phase.Start, phase.End);
                    if (buffs[i].TryGetValue(pair.Key, out FinalActorBuffs val))
                    {
                        JsonBuffsUptimeData value = JsonBuffsUptimeBuilder.BuildJsonBuffsUptimeData(val, buffsDictionary[pair.Key]);
                        data.Add(value);
                    }
                    else
                    {
                        var value = new JsonBuffsUptimeData();
                        data.Add(value);
                    }
                }
                res.Add(JsonBuffsUptimeBuilder.BuildJsonBuffsUptime(npc, pair.Key, log, settings, data, buffDesc));
            }
            return res;
        }
    }
}
