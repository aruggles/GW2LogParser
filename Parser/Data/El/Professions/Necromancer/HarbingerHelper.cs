﻿
using static Gw2LogParser.Parser.Helper.ParserHelper;
using System.Collections.Generic;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using static Gw2LogParser.Parser.Data.El.DamageModifiers.DamageModifier;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using Gw2LogParser.Parser.Data.El.Buffs;
using static Gw2LogParser.Parser.Helper.ArcDPSEnums;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class HarbingerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(62567, 59964, InstantCastFinders.InstantCastFinder.DefaultICD), // Harbinger shroud
            new BuffLossCastFinder(62540, 59964, InstantCastFinders.InstantCastFinder.DefaultICD), // Harbinger shroud
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(62653, "Wicked Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, "https://wiki.guildwars2.com/images/0/00/Wicked_Corruption.png", 118697, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(62653, "Septic Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, "https://wiki.guildwars2.com/images/f/f7/Septic_Corruption.png", 118697, ulong.MaxValue, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Harbinger Shroud",59964, Source.Harbinger, BuffNature.GraphOnlyBuff, "https://render.guildwars2.com/file/C9CA706909A104A509F594AADA150D680AA948BC/2479400.png", 118697, ulong.MaxValue),
                new Buff("Blight",62653, Source.Harbinger, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/ac/Blight.png", 118697, ulong.MaxValue),
        };
    }
}
