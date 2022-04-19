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
    internal static class VindicatorHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(62749, 62919, InstantCastFinders.InstantCastFinder.DefaultICD), // Legendary Alliance Stance
            new BuffGainCastFinder(62891, 62919, InstantCastFinders.InstantCastFinder.DefaultICD), // Legendary Alliance Stance (UW)
            new DamageCastFinder(62705, 62705, InstantCastFinders.InstantCastFinder.DefaultICD), // Call of the Alliance
            new BuffGainCastFinder(62687, 62864, InstantCastFinders.InstantCastFinder.DefaultICD), // Urn of Saint Viktor
            new BuffGainCastFinder(62693, 62811, InstantCastFinders.InstantCastFinder.DefaultICD), // Forerunner of Death (Death Drop) 
            new BuffGainCastFinder(62689, 62994, InstantCastFinders.InstantCastFinder.DefaultICD), // Saint of zu Heltzer (Saint's Shield)
            new DamageCastFinder(62859, 62859, InstantCastFinders.InstantCastFinder.DefaultICD), // Vassals of the Empire (Imperial Impact)
            //new EXTHealingCastFinder(-1, -1, InstantCastFinders.InstantCastFinder.DefaultICD), // Redemptor's Sermon
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(62811, "Forerunner of Death", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Vindicator, ByPresence, "https://wiki.guildwars2.com/images/9/95/Forerunner_of_Death.png", 119939, ulong.MaxValue, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Legendary Alliance Stance",62919, Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d6/Legendary_Alliance_Stance.png", 119939, ulong.MaxValue),
            new Buff("Urn of Saint Viktor", 62864, Source.Vindicator, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/ff/Urn_of_Saint_Viktor.png", 119939, ulong.MaxValue),
            new Buff("Saint of zu Heltzer", 62994, Source.Vindicator, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/3/36/Saint_of_zu_Heltzer.png", 119939, ulong.MaxValue),
            new Buff("Forerunner of Death", 62811, Source.Vindicator, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/9/95/Forerunner_of_Death.png", 119939, ulong.MaxValue),
            new Buff("Imperial Guard", 62819, Source.Vindicator, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7f/Imperial_Guard.png", 119939, ulong.MaxValue),
        };
    }
}
