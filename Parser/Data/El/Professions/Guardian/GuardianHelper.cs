﻿using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Extensions;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Data.El.DamageModifiers.DamageModifier;
using static Gw2LogParser.Parser.Helper.ArcDPSEnums;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class GuardianHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(9082, 9123, InstantCastFinders.InstantCastFinder.DefaultICD), // Shield of Wrath
            new BuffGainCastFinder(9104, 9103, 0), // Zealot's Flame
            //new BuffLossCastFinder(9115,9114,InstantCastFinder.DefaultICD), // Virtue of Justice
            //new BuffLossCastFinder(9120,9119,InstantCastFinder.DefaultICD), // Virtue of Resolve
            //new BuffLossCastFinder(9118,9113,InstantCastFinder.DefaultICD), // Virtue of Courage
            new DamageCastFinder(9247,9247, InstantCastFinders.InstantCastFinder.DefaultICD), // Judge's Intervention
            new DamageCastFinder(30255,30255, InstantCastFinders.InstantCastFinder.DefaultICD), // Wrath of Justice
            new DamageCastFinder(9101,9101, InstantCastFinders.InstantCastFinder.DefaultICD), // Smiter's Boon
            new DamageCastFinder(9245,9245, InstantCastFinders.InstantCastFinder.DefaultICD), // Smite Condition
            //new DamageCastFinder(9097,9097, InstantCastFinders.InstantCastFinder.DefaultICD), // Symbol of Blades
            new DamageCastFinder(21795, 21795, InstantCastFinders.InstantCastFinder.DefaultICD), // Glacial Heart
            new DamageCastFinder(22499, 22499, InstantCastFinders.InstantCastFinder.DefaultICD), // Shattered Aegis
            new EXTHealingCastFinder(13594, 13594, InstantCastFinders.InstantCastFinder.DefaultICD), // Selfless Daring
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Zeal
            new BuffDamageModifierTarget(737, "Fiery Wrath", "7% on burning target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/7/70/Fiery_Wrath.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(738, "Symbolic Exposure", "5% on vuln target", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/c/cd/Symbolic_Exposure.png", DamageModifierMode.All),
            new BuffDamageModifier(56890, "Symbolic Avenger", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, "https://wiki.guildwars2.com/images/e/e5/Symbolic_Avenger.png", 97950, ulong.MaxValue, DamageModifierMode.All),
            // Radiance
            new BuffDamageModifier(873, "Retribution", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/d/d7/Retribution_%28trait%29.png", 0, 115190,DamageModifierMode.All),
            new BuffDamageModifier(873, "Retribution", "10% under resolution", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/d/d7/Retribution_%28trait%29.png", 115190, ulong.MaxValue, DamageModifierMode.All),
            // Virtues
            new BuffDamageModifier(743, "Unscathed Contender", "20% under aegis", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png", DamageModifierMode.All),
            new BuffDamageModifier(NumberOfBoonsID, "Power of the Virtuous", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, "https://wiki.guildwars2.com/images/e/ee/Power_of_the_Virtuous.png", 0, 115190, DamageModifierMode.All),
            new BuffDamageModifier(NumberOfBoonsID, "Inspired Virtue", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, "https://wiki.guildwars2.com/images/c/c7/Inspired_Virtue.png", 115190, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(59592, "Inspiring Virtue", "10% (6s) after activating a virtue ", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/8/8f/Virtuous_Solace.png", 102321, ulong.MaxValue, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {        
                //skills
                new Buff("Zealot's Flame", 9103, Source.Guardian, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Zealot%27s_Flame.png"),
                new Buff("Purging Flames",21672, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/28/Purging_Flames.png"),
                new Buff("Litany of Wrath",21665, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4a/Litany_of_Wrath.png"),
                new Buff("Renewed Focus",9255, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/10/Renewed_Focus.png"),
                new Buff("Shield of Wrath",9123, Source.Guardian, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/bc/Shield_of_Wrath.png"),
                new Buff("Binding Blade (Self)",9225, Source.Guardian, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/de/Binding_Blade.png"),
                new Buff("Binding Blade",9148, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/de/Binding_Blade.png"),
                //signets
                new Buff("Signet of Resolve",9220, Source.Guardian, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/75/Signet_of_Resolve.png"),
                new Buff("Signet of Resolve (Shared)", 46554, Source.Guardian, BuffStackType.Stacking, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/75/Signet_of_Resolve.png"),
                new Buff("Bane Signet",9092, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png"),
                new Buff("Bane Signet (PI)",9240, Source.Guardian, BuffStackType.Stacking, 25, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png"),
                new Buff("Signet of Judgment",9156, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png"),
                new Buff("Signet of Judgment (PI)",9239, Source.Guardian, BuffStackType.Stacking, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png"),
                new Buff("Signet of Mercy",9162, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png"),
                new Buff("Signet of Mercy (PI)",9238, Source.Guardian, BuffStackType.Stacking, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png"),
                new Buff("Signet of Wrath",9100, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png"),
                new Buff("Signet of Wrath (PI)",9237, Source.Guardian, BuffStackType.Stacking, 25, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png"),
                new Buff("Signet of Courage",29633, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5d/Signet_of_Courage.png"),
                new Buff("Signet of Courage (Shared)",43487 , Source.Guardian, BuffStackType.Stacking, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/5/5d/Signet_of_Courage.png"),
                //virtues
                new Buff("Virtue of Justice", 9114, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/10/Virtue_of_Justice.png"),
                new Buff("Virtue of Courage", 9113, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a9/Virtue_of_Courage.png"),
                new Buff("Virtue of Resolve", 9119, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b2/Virtue_of_Resolve.png"),
                new Buff("Justice", 9116, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/10/Virtue_of_Justice.png"),
                //traits
                new Buff("Strength in Numbers",13796, Source.Guardian, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/7b/Strength_in_Numbers.png"),
                new Buff("Invigorated Bulwark",30207, Source.Guardian, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/00/Invigorated_Bulwark.png"),
                new Buff("Virtue of Resolve (Battle Presence)", 17046, Source.Guardian, BuffStackType.Queue, 2, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/2/27/Battle_Presence.png"),
                new Buff("Virtue of Resolve (Battle Presence - Absolute Resolve)", 17047, Source.Guardian, BuffStackType.Queue, 2, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/b/b2/Virtue_of_Resolve.png"),
                new Buff("Symbolic Avenger",56890, Source.Guardian, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e5/Symbolic_Avenger.png", 97950, ulong.MaxValue),
                new Buff("Inspiring Virtue",59592, Source.Guardian, BuffStackType.Queue, 99, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8f/Virtuous_Solace.png", 102321, 102389),
                new Buff("Inspiring Virtue",59592, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8f/Virtuous_Solace.png", 102389, ulong.MaxValue),
                new Buff("Force of Will",29485, Source.Guardian, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d2/Force_of_Will.png"),
        };
    }
}
