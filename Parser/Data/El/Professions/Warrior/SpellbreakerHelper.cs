﻿using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Data.El.DamageModifiers.DamageModifier;
using static Gw2LogParser.Parser.Helper.ArcDPSEnums;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class SpellbreakerHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(43745, 40616, InstantCastFinders.InstantCastFinder.DefaultICD), // Sight beyond Sight
            new DamageCastFinder(45534, 45534, InstantCastFinders.InstantCastFinder.DefaultICD), // Loss Aversion

        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(NumberOfBoonsID, "Pure Strike (boons)", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, "https://wiki.guildwars2.com/images/7/76/Pure_Strike_%28trait%29.png", DamageModifierMode.All, (x, log) => x.HasCrit),
            new BuffDamageModifierTarget(NumberOfBoonsID, "Pure Strike (no boons)", "14% crit damage", DamageSource.NoPets, 14.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, "https://wiki.guildwars2.com/images/7/76/Pure_Strike_%28trait%29.png", DamageModifierMode.All, (x, log) => x.HasCrit),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Sight beyond Sight",40616, Source.Spellbreaker, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/d/d7/Sight_beyond_Sight.png"),
                new Buff("Full Counter",43949, Source.Spellbreaker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fb/Full_Counter.png"),
                new Buff("Disenchantment",44633, Source.Spellbreaker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e1/Winds_of_Disenchantment.png"),
                new Buff("Attacker's Insight",41963, Source.Spellbreaker, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/89/Attacker%27s_Insight.png"),
        };
    }
}
