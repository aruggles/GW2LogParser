﻿
using GW2EIEvtcParser.EIData;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class DamageModDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Tooltip { get; set; }
        public bool NonMultiplier { get; set; }
        public bool SkillBased { get; set; }
        public bool Approximate { get; set; }

        public static void AssembleDamageModifiers(ICollection<DamageModifier> damageMods, Dictionary<string, DamageModDto> dict)
        {
            foreach (DamageModifier mod in damageMods)
            {
                int id = mod.ID;
                dict["d" + id] = new DamageModDto()
                {
                    Id = id,
                    Name = mod.Name,
                    Icon = mod.Icon,
                    Tooltip = mod.Tooltip,
                    NonMultiplier = !mod.Multiplier,
                    SkillBased = mod.SkillBased,
                    Approximate = mod.Approximate
                };
            }
        }
    }
}
