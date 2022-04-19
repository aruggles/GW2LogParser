using Gw2LogParser.Parser.Data.El.Statistics;
using static Gw2LogParser.GW2EIBuilders.JsonPlayerBuffsGeneration;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonPlayerBuffsGenerationBuilder
    {
        public static JsonBuffsGenerationData BuildJsonBuffsGenerationData(FinalActorBuffs stats)
        {
            var jsonBuffsGenerationData = new JsonBuffsGenerationData();
            jsonBuffsGenerationData.Generation = stats.Generation;
            jsonBuffsGenerationData.Overstack = stats.Overstack;
            jsonBuffsGenerationData.Wasted = stats.Wasted;
            jsonBuffsGenerationData.UnknownExtended = stats.UnknownExtended;
            jsonBuffsGenerationData.Extended = stats.Extended;
            jsonBuffsGenerationData.ByExtension = stats.ByExtension;
            return jsonBuffsGenerationData;
        }
    }
}
