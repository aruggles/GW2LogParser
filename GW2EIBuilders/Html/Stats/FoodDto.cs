using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    public class FoodDto
    {
        public double Time { get; set; }
        public double Duration { get; set; }
        public long Id { get; set; }
        public int Stack { get; set; }
        public bool Dimished { get; set; }

        private FoodDto(Consumable consume)
        {
            Time = consume.Time / 1000.0;
            Duration = consume.Duration / 1000.0;
            Stack = consume.Stack;
            Id = consume.Buff.ID;
            Dimished = (consume.Buff.ID == 46587 || consume.Buff.ID == 46668);
        }

        public static List<FoodDto> BuildFoodData(ParsedLog log, AbstractSingleActor actor, Dictionary<long, Buff> usedBuffs)
        {
            var list = new List<FoodDto>();
            IReadOnlyList<Consumable> consume = actor.GetConsumablesList(log, 0, log.FightData.FightEnd);

            foreach (Consumable entry in consume)
            {
                usedBuffs[entry.Buff.ID] = entry.Buff;
                list.Add(new FoodDto(entry));
            }

            return list;
        }
    }
}
