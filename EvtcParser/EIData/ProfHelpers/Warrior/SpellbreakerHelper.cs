﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class SpellbreakerHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(SightBeyondSightSkill, SightBeyondSightBuff),
            new DamageCastFinder(LossAversion, LossAversion).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),

        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(NumberOfBoons, "Pure Strike (boons)", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, BuffImages.PureStrike, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffDamageModifierTarget(NumberOfBoons, "Pure Strike (boons)", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, BuffImages.PureStrike, DamageModifierMode.sPvPWvW).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022Balance),
            new BuffDamageModifierTarget(NumberOfBoons, "Pure Strike (boons)", "7.5% crit damage", DamageSource.NoPets, 7.5, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, BuffImages.PureStrike, DamageModifierMode.PvE).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022Balance),
            new BuffDamageModifierTarget(NumberOfBoons, "Pure Strike (no boons)", "14% crit damage", DamageSource.NoPets, 14.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, BuffImages.PureStrike, DamageModifierMode.All).UsingChecker( (x, log) => x.HasCrit).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffDamageModifierTarget(NumberOfBoons, "Pure Strike (no boons)", "14% crit damage", DamageSource.NoPets, 14.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, BuffImages.PureStrike, DamageModifierMode.sPvPWvW).UsingChecker( (x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022Balance),
            new BuffDamageModifierTarget(NumberOfBoons, "Pure Strike (no boons)", "15% crit damage", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, BuffImages.PureStrike, DamageModifierMode.PvE).UsingChecker( (x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022Balance),
            new BuffDamageModifierTarget(MagebaneTether, "Magebane Tether", "10% to tethered target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, BuffImages.MagebaneTether, DamageModifierMode.PvEInstanceOnly).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(MagebaneTether).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 8000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.Time - effectApply.Time < 8000;
                }
                return false;
            }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffDamageModifierTarget(MagebaneTether, "Magebane Tether", "15% to tethered target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, BuffImages.MagebaneTether, DamageModifierMode.PvEInstanceOnly).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(MagebaneTether).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 8000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.Time - effectApply.Time < 8000;
                }
                return false;
            }).WithBuilds(GW2Builds.August2022Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Sight beyond Sight", SightBeyondSightBuff, Source.Spellbreaker, BuffClassification.Other, BuffImages.SightBeyondSight),
            new Buff("Full Counter", FullCounterBuff, Source.Spellbreaker, BuffClassification.Other, BuffImages.FullCounter),
            new Buff("Disenchantment", Disenchantment, Source.Spellbreaker, BuffClassification.Other, BuffImages.WindsOfDisenchantment),
            new Buff("Attacker's Insight", AttackersInsight, Source.Spellbreaker, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.AttackersInsight),
            new Buff("Magebane Tether", MagebaneTether, Source.Spellbreaker, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.MagebaneTether),
        };


    }
}
