using GW2EIEvtcParser.EIData;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonConsumableBuilder
    {
        public static JsonConsumable BuildJsonConsumable(Consumable food)
        {
            var jsonConsumable = new JsonConsumable();
            jsonConsumable.Stack = food.Stack;
            jsonConsumable.Duration = food.Duration;
            jsonConsumable.Time = food.Time;
            jsonConsumable.Id = food.Buff.ID;
            return jsonConsumable;
        }
    }
}
