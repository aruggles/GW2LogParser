using Newtonsoft.Json;

namespace Gw2LogParser.GW2Api
{
    public class GW2APITraitedFact : GW2APIFact
    {
        [JsonProperty(PropertyName = "requires_trait")]
        public int RequiresTrait { get; internal set; }
        [JsonProperty]
        public int Overrides { get; internal set; }
    }
}
