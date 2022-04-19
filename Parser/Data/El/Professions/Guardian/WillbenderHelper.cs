﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Data.El.DamageModifiers.DamageModifier;
using static Gw2LogParser.Parser.Helper.ArcDPSEnums;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class WillbenderHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffApproximateDamageModifier(62509, "Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", 118697, ulong.MaxValue, DamageModifierMode.All, (x, log) => {
                Agent src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(62509).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 6000;
                }
                return false;
            }),
            new BuffApproximateDamageModifier(62509, "Tyrant's Lethal Tempo", "5% per stack", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", 118697, ulong.MaxValue, DamageModifierMode.All, (x, log) => {
                Agent src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(62509).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 4000;
                }
                return false;
            }),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                //virtues
                new Buff("Rushing Justice", 62529, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/74/Rushing_Justice.png", 118697, ulong.MaxValue),
                new Buff("Flowing Resolve", 62632, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/be/Flowing_Resolve.png", 118697, ulong.MaxValue),
                new Buff("Crashing Courage", 62615, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/95/Crashing_Courage.png", 118697, ulong.MaxValue),
                //
                new Buff("Repose", 62638, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/80/Repose.png", 118697, ulong.MaxValue),
                new Buff("Lethal Tempo", 62509, Source.Willbender, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", 118697, ulong.MaxValue),
                //new Buff("Tyrant's Lethal Tempo", 62657, Source.Willbender, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", 118697, ulong.MaxValue),
        };
    }
}
