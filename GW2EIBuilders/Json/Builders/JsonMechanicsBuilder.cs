﻿using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;
using static Gw2LogParser.GW2EIBuilders.JsonMechanics;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonMechanicsBuilder
    {
        public static JsonMechanic BuildJsonMechanic(MechanicEvent ml)
        {
            var jsMech = new JsonMechanic();
            jsMech.Time = ml.Time;
            jsMech.Actor = ml.Actor.Character;
            return jsMech;
        }

        public static JsonMechanics BuildJsonMechanics(Mechanic mech, List<JsonMechanic> data)
        {
            var jsMechs = new JsonMechanics();
            jsMechs.Name = mech.ShortName;
            jsMechs.FullName = mech.FullName;
            jsMechs.Description = mech.Description;
            jsMechs.IsAchievementEligibility = mech.IsAchievementEligibility;
            jsMechs.MechanicsData = data;
            return jsMechs;
        }

        internal static List<JsonMechanics> GetJsonMechanicsList(ParsedLog log, MechanicData mechanicData, IReadOnlyCollection<Mechanic> presentMechanics)
        {
            var mechanics = new List<JsonMechanics>();
            var dict = new Dictionary<Mechanic, List<JsonMechanic>>();
            foreach (Mechanic mech in presentMechanics)
            {
                var jsonMechanics = new List<JsonMechanic>();
                foreach (MechanicEvent ml in mechanicData.GetMechanicLogs(log, mech, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    jsonMechanics.Add(BuildJsonMechanic(ml));
                }
                dict[mech] = jsonMechanics;
            }
            foreach (KeyValuePair<Mechanic, List<JsonMechanic>> pair in dict)
            {
                mechanics.Add(BuildJsonMechanics(pair.Key, pair.Value));
            }
            return mechanics;
        }
    }
}
