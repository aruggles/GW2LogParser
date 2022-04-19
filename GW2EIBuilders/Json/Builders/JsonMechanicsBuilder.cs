﻿using Gw2LogParser.Parser.Data.Events.Mechanics;
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

        public static JsonMechanics BuildJsonMechanics(string name, string description, List<JsonMechanic> data)
        {
            var jsMechs = new JsonMechanics();
            jsMechs.Name = name;
            jsMechs.Description = description;
            jsMechs.MechanicsData = data;
            return jsMechs;
        }

        internal static List<JsonMechanics> GetJsonMechanicsList(List<MechanicEvent> mechanicLogs)
        {
            var mechanics = new List<JsonMechanics>();
            var dict = new Dictionary<string, (string desc, List<JsonMechanic> data)>();
            foreach (MechanicEvent ml in mechanicLogs)
            {
                JsonMechanic mech = BuildJsonMechanic(ml);
                if (dict.TryGetValue(ml.ShortName, out (string _, List<JsonMechanic> data) jsonMechData))
                {
                    jsonMechData.data.Add(mech);
                }
                else
                {
                    dict[ml.ShortName] = (ml.Description, new List<JsonMechanic> { mech });
                }
            }
            foreach (KeyValuePair<string, (string desc, List<JsonMechanic> data)> pair in dict)
            {
                mechanics.Add(BuildJsonMechanics(pair.Key, pair.Value.desc, pair.Value.data));
            }
            return mechanics;
        }
    }
}
