using Newtonsoft.Json;

namespace Gw2LogParser.GW2Api
{
    public abstract class GW2APIBaseItem
    {
        [JsonProperty]
        public long Id { get; internal set; }
    }
}
