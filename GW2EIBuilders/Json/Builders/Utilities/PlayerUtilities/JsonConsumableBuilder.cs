using GW2EIEvtcParser.EIData;

namespace Gw2LogParser.GW2EIBuilders
{
    /// <summary>
    /// Class representing consumables
    /// </summary>
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
