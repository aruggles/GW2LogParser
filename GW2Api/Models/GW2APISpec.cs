using Newtonsoft.Json;

namespace GW2EIGW2API.GW2API
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
