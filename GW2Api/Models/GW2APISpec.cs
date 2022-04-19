using Newtonsoft.Json;

namespace Gw2LogParser.GW2Api
{
    public class GW2APISpec : GW2APIBaseItem
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Profession { get; set; }
        [JsonProperty]
        public bool Elite { get; set; }
        //minor_traits
        //major_traits
        [JsonProperty]
        public string Icon { get; set; }
        [JsonProperty]
        public string Background { get; set; }
    }
}
