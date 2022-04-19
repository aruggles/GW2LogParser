﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using System;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Data.El.DamageModifiers.DamageModifier;
using static Gw2LogParser.Parser.Helper.ArcDPSEnums;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class DragonhunterHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(721, "Zealot's Aggression", "10% on crippled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/7e/Zealot%27s_Aggression.png", DamageModifierMode.All),
            // Pur of Sight unclear. Max is very likely to be 1200, as it is the maximum tooltip range for a DH but what is the distance at witch the minimum is reached? Is the scaling linear?
            new BuffDamageModifierTarget(30232, "Big Game Hunter", "10% to tethered target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/76/Big_Game_Hunter.png", 0, 92715, DamageModifierMode.All, (x, log) => {
                Agent src = x.From;
                Agent dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(30232).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 8000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 8000;
                }
                return false;
            }),
            new BuffDamageModifierTarget(30232, "Big Game Hunter", "20% to tethered target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/76/Big_Game_Hunter.png", 92715, 102321, DamageModifierMode.All, (x, log) => {
                Agent src = x.From;
                Agent dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(30232).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 8000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 8000;
                }
                return false;
            }),
            new BuffDamageModifierTarget(30232, "Big Game Hunter", "15% to tethered target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/76/Big_Game_Hunter.png", 102321, ulong.MaxValue, DamageModifierMode.All, (x, log) => {
                Agent src = x.From;
                Agent dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(30232).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 10000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 10000;
                }
                return false;
            }),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Justice (Dragonhunter)",30232, Source.Dragonhunter, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b0/Spear_of_Light.png"),
                new Buff("Shield of Courage (Active)", 29906, Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Shield_of_Courage.png"),
                new Buff("Spear of Justice", 29632, Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f1/Spear_of_Justice.png"),
                new Buff("Shield of Courage", 29523, Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Shield_of_Courage.png"),
                new Buff("Wings of Resolve", 30308, Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/cb/Wings_of_Resolve.png"),
        };

    }
}
