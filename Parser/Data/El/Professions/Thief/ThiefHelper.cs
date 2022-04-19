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
    internal static class ThiefHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(13002,13135,InstantCastFinders.InstantCastFinder.DefaultICD), // Shadowstep
            new BuffLossCastFinder(13106,13135,InstantCastFinders.InstantCastFinder.DefaultICD, (evt, combatData) => evt.RemovedDuration > ServerDelayConstant), // Shadow Return
            new DamageCastFinder(13014, 13014, InstantCastFinders.InstantCastFinder.DefaultICD), // Mug
            new BuffGainCastFinder(13046,44597,InstantCastFinders.InstantCastFinder.DefaultICD), // Assassin's Signet
            new BuffGiveCastFinder(13093,13094,InstantCastFinders.InstantCastFinder.DefaultICD), // Devourer Venom
            new BuffGiveCastFinder(13096,13095,InstantCastFinders.InstantCastFinder.DefaultICD), // Ice Drake Venom
            new BuffGiveCastFinder(13055,13054,InstantCastFinders.InstantCastFinder.DefaultICD), // Skale Venom
            new BuffGiveCastFinder(49052,49083,InstantCastFinders.InstantCastFinder.DefaultICD), // Soul Stone Venom
            //new BuffGiveCastFinder(13037,13036,InstantCastFinder.DefaultICD), // Spider Venom - same id as leeching venom trait?
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Deadly arts
            new BuffDamageModifierTarget(NumberOfConditionsID, "Exposed Weakness", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Thief, ByStack, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png", 90455, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifierTarget(NumberOfConditionsID, "Exposed Weakness", "10% if condition on target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Thief, ByPresence, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png", 0, 90455, DamageModifierMode.PvE),
            new DamageLogDamageModifier("Executioner", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Thief,"https://wiki.guildwars2.com/images/9/93/Executioner.png", (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            // Critical Strikes
            new DamageLogDamageModifier("Twin Fangs","7% over 90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", (x, log) => x.IsOverNinety && x.HasCrit, ByPresence, DamageModifierMode.All),
            new DamageLogDamageModifier("Ferocious Strikes", "10% on critical strikes if target >50%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", (x, log) => !x.AgainstUnderFifty && x.HasCrit, ByPresence, DamageModifierMode.All),
            // Trickery
            new BuffDamageModifier(34659, "Lead Attacks", "1% (10s) per initiative spent", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Thief, ByStack, "https://wiki.guildwars2.com/images/0/01/Lead_Attacks.png", DamageModifierMode.All), // It's not always possible to detect the presence of pistol and the trait is additive with itself. Staff master is worse as we can't detect endurance at all
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            //signets
            new Buff("Signet of Malice",13049, Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/ae/Signet_of_Malice.png"),
            new Buff("Assassin's Signet (Passive)",13047, Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Assassin%27s_Signet.png"),
            new Buff("Assassin's Signet (Active)",44597, Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Assassin%27s_Signet.png"),
            new Buff("Infiltrator's Signet",13063, Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8e/Infiltrator%27s_Signet.png"),
            new Buff("Signet of Agility",13061, Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1d/Signet_of_Agility.png"),
            new Buff("Signet of Shadows",13059, Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/17/Signet_of_Shadows.png"),
            //venoms // src is always the user, makes generation data useless
            new Buff("Skelk Venom",21780, Source.Thief, BuffStackType.StackingConditionalLoss, 5, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/75/Skelk_Venom.png"),
            new Buff("Ice Drake Venom",13095, Source.Thief, BuffStackType.StackingConditionalLoss, 4, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/7/7b/Ice_Drake_Venom.png"),
            new Buff("Devourer Venom", 13094, Source.Thief, BuffStackType.StackingConditionalLoss, 2, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/4/4d/Devourer_Venom.png"),
            new Buff("Skale Venom", 13054, Source.Thief, BuffStackType.StackingConditionalLoss, 4, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/1/14/Skale_Venom.png"),
            new Buff("Spider Venom",13036, Source.Thief, BuffStackType.StackingConditionalLoss, 6, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/3/39/Spider_Venom.png"),
            new Buff("Soul Stone Venom",49083, Source.Thief, BuffStackType.Stacking, 25, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/d/d6/Soul_Stone_Venom.png"),
            new Buff("Basilisk Venom", 13133, Source.Thief, BuffStackType.StackingConditionalLoss, 2, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/3/3a/Basilisk_Venom.png"),
            new Buff("Infiltration",13135, Source.Thief, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/25/Shadowstep.png"),
            //transforms
            new Buff("Dagger Storm",13134, Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c0/Dagger_Storm.png"),
            //traits
            new Buff("Hidden Killer",42720, Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ec/Hidden_Killer.png"),
            new Buff("Lead Attacks",34659, Source.Thief, BuffStackType.Stacking, 15, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/01/Lead_Attacks.png"),
            new Buff("Instant Reflexes",34283, Source.Thief, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7d/Instant_Reflexes.png"),
        };
    }
}
