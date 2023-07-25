using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GW2EIGW2API.GW2API
{
    public class GW2APIFact
    {
        [JsonProperty]
        public string Text { get; set; }
        [JsonProperty]
        public string Icon { get; set; }
        [JsonProperty]
        public string Type { get; set; }
        [JsonProperty]
        public string Target { get; set; }
        [JsonProperty]
        public object Value { get; set; }
        [JsonProperty]
        public string Status { get; set; }
        [JsonProperty]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "apply_count")]
        public int ApplyCount { get; set; }
        [JsonProperty]
        public int Duration { get; set; }
        [JsonProperty(PropertyName = "field_type")]
        public string FieldType { get; set; }
        [JsonProperty(PropertyName = "finisher_type")]
        public string FinisherType { get; set; }
        [JsonProperty]
        public float Percent { get; set; }
        [JsonProperty(PropertyName = "hit_count")]
        public int HitCount { get; set; }
        [JsonProperty(PropertyName = "dmg_multiplier")]
        public float DmgMultiplier { get; set; }
        [JsonProperty]
        public int Distance { get; set; }
        [JsonProperty]
        public GW2APIFact Prefix { get; set; }

        
    }
}
