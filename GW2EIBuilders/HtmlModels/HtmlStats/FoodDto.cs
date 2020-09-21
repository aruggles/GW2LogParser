using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Statistics;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    public class FoodDto
    {
        public double Time { get; internal set; }
        public double Duration { get; internal set; }
        public long Id { get; internal set; }
        public int Stack { get; internal set; }
        public bool Dimished { get; internal set; }

        private FoodDto(Consumable consume)
        {
            Time = consume.Time / 1000.0;
            Duration = consume.Duration / 1000.0;
            Stack = consume.Stack;
            Id = consume.Buff.ID;
            Dimished = (consume.Buff.ID == 46587 || consume.Buff.ID == 46668);
        }

        internal static List<FoodDto> BuildPlayerFoodData(ParsedLog log, Player p, Dictionary<long, Buff> usedBuffs)
        {
            var list = new List<FoodDto>();
            List<Consumable> consume = p.GetConsumablesList(log, 0, log.FightData.FightEnd);

            foreach (Consumable entry in consume)
            {
                usedBuffs[entry.Buff.ID] = entry.Buff;
                list.Add(new FoodDto(entry));
            }

            return list;
        }
    }
}
