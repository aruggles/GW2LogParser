﻿using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Data.El.DamageModifiers.DamageModifier;
using static Gw2LogParser.Parser.Helper.ArcDPSEnums;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class HeraldHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(28085, 27732, InstantCastFinders.InstantCastFinder.DefaultICD), // Legendary Dragon Stance
            new BuffGainCastFinder(29371, 29275, InstantCastFinders.InstantCastFinder.DefaultICD), // Facet of Nature
            new BuffGainCastFinder(28379, 28036, InstantCastFinders.InstantCastFinder.DefaultICD), // Facet of Darkness
            new BuffGainCastFinder(27014, 28243, InstantCastFinders.InstantCastFinder.DefaultICD), // Facet of Elements
            new BuffGainCastFinder(26644, 27376, InstantCastFinders.InstantCastFinder.DefaultICD), // Facet of Strength
            new BuffGainCastFinder(27760, 27983, InstantCastFinders.InstantCastFinder.DefaultICD), // Facet of Chaos
            new DamageCastFinder(46857, 46857, InstantCastFinders.InstantCastFinder.DefaultICD), // Call of the Dragon
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(NumberOfBoonsID, "Reinforced Potency", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Herald, ByStack, "https://wiki.guildwars2.com/images/0/0a/Envoy_of_Sustenance.png", DamageModifierMode.All),
            new BuffDamageModifier(51653 , "Burst of Strength", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, "https://wiki.guildwars2.com/images/7/7b/Burst_of_Strength.png", 92069, 97950, DamageModifierMode.All),
            new BuffDamageModifier(51653 , "Burst of Strength", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, "https://wiki.guildwars2.com/images/7/7b/Burst_of_Strength.png", 97950, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(51653 , "Burst of Strength", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, "https://wiki.guildwars2.com/images/7/7b/Burst_of_Strength.png", 97950, 102321, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(51653 , "Burst of Strength", "7%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, "https://wiki.guildwars2.com/images/7/7b/Burst_of_Strength.png", 102321, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            // 
            new BuffDamageModifier(new long[] { 27336, 29275, 28036, 28243, 27376, 27983}, "Forceful Persistence (Facets)", "4% per active Facet", DamageSource.NoPets, 4.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, "https://wiki.guildwars2.com/images/5/5f/Forceful_Persistence.png", 92069, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(new long[] { 27336, 29275, 28036, 28243, 27376, 27983}, "Forceful Persistence (Facets)", "3% per active Facet", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, "https://wiki.guildwars2.com/images/5/5f/Forceful_Persistence.png", 92069, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            //new BuffDamageModifier(new long[] { 27273, 27581, 28001}, "Forceful Persistence", "13% if active upkeep", DamageSource.NoPets, 13.0, DamageType.Power, DamageType.All, Source.Herald, ByPresence, "https://wiki.guildwars2.com/images/5/5f/Forceful_Persistence.png", 92069, ulong.MaxValue, DamageModifierMode.All), // Hammers, Embrace, Impossible Odds but how to track Protective Solace?
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {         
                //skills
                new Buff("Crystal Hibernation", 29303, Source.Herald, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/4a/Crystal_Hibernation.png"),
                //facets
                new Buff("Facet of Light",27336, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"),
                new Buff("Facet of Light (Traited)",51690, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"), //Lingering buff with Draconic Echo trait
                new Buff("Infuse Light",27737, Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/6/60/Infuse_Light.png"),
                new Buff("Facet of Darkness",28036, Source.Herald, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),
                new Buff("Facet of Darkness (Traited)",51695, Source.Herald, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Elements",28243, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),
                new Buff("Facet of Elements (Traited)",51706, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Strength",27376, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),
                new Buff("Facet of Strength (Traited)",51700, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Chaos",27983, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Facet_of_Chaos.png"),
                new Buff("Facet of Chaos (Traited)",51685, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Facet_of_Chaos.png"),
                new Buff("Facet of Nature",29275, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                new Buff("Facet of Nature (Traited)",51681, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Nature-Assassin",51692, Source.Herald, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/c/cd/Facet_of_Nature%E2%80%95Assassin.png"),
                new Buff("Facet of Nature-Dragon",51674, Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/4/47/Facet_of_Nature%E2%80%95Dragon.png"),
                new Buff("Facet of Nature-Demon",51704, Source.Herald, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/f/ff/Facet_of_Nature%E2%80%95Demon.png"),
                new Buff("Facet of Nature-Dwarf",51677, Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/4/4c/Facet_of_Nature%E2%80%95Dwarf.png"),
                new Buff("Facet of Nature-Centaur",51699, Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/74/Facet_of_Nature%E2%80%95Centaur.png"),
                new Buff("Naturalistic Resonance", 29379, Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                new Buff("Legendary Dragon Stance",27732, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d5/Legendary_Dragon_Stance.png"),
                new Buff("Hardening Persistence",28957, Source.Herald, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/0f/Hardening_Persistence.png"),
                new Buff("Soothing Bastion",34136, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/04/Soothing_Bastion.png"),
                new Buff("Burst of Strength",51653, Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7b/Burst_of_Strength.png"),
                new Buff("Rising Momentum",51683, Source.Herald, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8c/Rising_Momentum.png"),
        };
    }
}
