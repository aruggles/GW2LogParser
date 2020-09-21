using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gw2LogParser.GW2Api
{
    public class GW2APISkill
    {
        [JsonProperty]
        public long Id { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Description { get; set; }
        [JsonProperty]
        public string Icon { get; set; }
        [JsonProperty(PropertyName = "chat_link")]
        public string ChatLink { get; set; }
        public string Type { get; set; }
        [JsonProperty(PropertyName = "weapon_type")]
        public string WeaponType { get; set; }
        [JsonProperty]
        public string[] Professions { get; set; }
        [JsonProperty]
        public string Slot { get; set; }
        [JsonProperty]
        public string[] Categories { get; set; }
        [JsonProperty]
        public List<GW2APIFact> Facts { get; set; }
        [JsonProperty(PropertyName = "traited_facts")]
        public List<GW2APITraitedFact> TraitedFacts { get; set; }
        [JsonProperty]
        public string Attunement { get; internal set; }
        [JsonProperty]
        public int Cost { get; internal set; }
        [JsonProperty(PropertyName = "dual_wield")]
        public string DualWield { get; internal set; }
        [JsonProperty(PropertyName = "flip_skill")]
        public int FlipSkill { get; internal set; }
        [JsonProperty]
        public int Initiative { get; internal set; }
        [JsonProperty(PropertyName = "next_chain")]
        public int NextChain { get; internal set; }
        [JsonProperty(PropertyName = "prev_chain")]
        public int PrevChain { get; internal set; }
        [JsonProperty(PropertyName = "transform_skills")]
        public List<int> TransformSkills { get; internal set; }
        [JsonProperty(PropertyName = "bundle_skills")]
        public List<int> BundleSkills { get; internal set; }
        [JsonProperty(PropertyName = "toolbelt_skill")]
        public int ToolbeltSkill { get; internal set; }
    }
}
