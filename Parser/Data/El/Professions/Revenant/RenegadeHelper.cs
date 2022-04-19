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
    internal static class RenegadeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(41858, 44272, InstantCastFinders.InstantCastFinder.DefaultICD), // Legendary Renegade Stance
            new DamageCastFinder(46849, 46849, InstantCastFinders.InstantCastFinder.DefaultICD), // Call of the Renegade
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(42883, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", 0, 115190, DamageModifierMode.All),
            new BuffDamageModifier(45614, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", 0, 115190, DamageModifierMode.All),
            new BuffDamageModifier(42883, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", 115190, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(45614, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", 115190, ulong.MaxValue, DamageModifierMode.PvE),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Legendary Renegade Stance",44272, Source.Renegade, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/19/Legendary_Renegade_Stance.png"),
                new Buff("Breakrazor's Bastion",44682, Source.Renegade, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/a/a7/Breakrazor%27s_Bastion.png"),
                new Buff("Razorclaw's Rage",41016, Source.Renegade, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/7/73/Razorclaw%27s_Rage.png"),
                new Buff("Soulcleave's Summit",45026, Source.Renegade, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/7/78/Soulcleave%27s_Summit.png"),
                new Buff("Kalla's Fervor",42883, Source.Renegade, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
                new Buff("Improved Kalla's Fervor",45614, Source.Renegade, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
        };
    }
}
