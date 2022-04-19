using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Statistics;
using System.Collections.Generic;

namespace Gw2LogParser.ExportModels
{
    public class Food
    {
        public double Time { get; set; }
        public double Duration { get; set; }
        public long Id { get; set; }
        public int Stack { get; set; }
        public bool Dimished { get; set; }

        public Food(Consumable consume)
        {
            Time = consume.Time / 1000.0;
            Duration = consume.Duration / 1000.0;
            Stack = consume.Stack;
            Id = consume.Buff.ID;
            Dimished = (consume.Buff.ID == 46587 || consume.Buff.ID == 46668);
        }

        internal static List<Food> BuildFoodData(ParsedLog log, AbstractSingleActor actor, Dictionary<long, Buff> usedBuffs)
        {
            var list = new List<Food>();
            IReadOnlyList<Consumable> consume = actor.GetConsumablesList(log, 0, log.FightData.FightEnd);

            foreach (Consumable entry in consume)
            {
                usedBuffs[entry.Buff.ID] = entry.Buff;
                list.Add(new Food(entry));
            }

            return list;
        }
    }
}
