﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.EIData.CastFinderHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class MesmerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffLossCastFinder(SignetOfMidnightSkill, SignetOfMidnightBuff)
                .UsingChecker((brae, combatData, agentData, skillData) => {
                     return HasGainedBuff(combatData, HideInShadows, brae.To, brae.Time, 2000, brae.To);
                })
                .UsingNotAccurate(true) // HideInShadows may not be applied if the Mesmer has a full stack of HideInShadows already
                .UsingDisableWithEffectData(),
            new EffectCastFinderByDst(SignetOfMidnightSkill, EffectGUIDs.MesmerSignetOfMidnight).UsingDstBaseSpecChecker(Spec.Mesmer),
            new BuffGainCastFinder(PortalEntre, PortalWeaving),
            new BuffGainCastFinder(PortalExeunt, PortalUses),
            new DamageCastFinder(LesserPhantasmalDefender, LesserPhantasmalDefender).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            /*new BuffGainCastFinder(10192, 10243, GW2Builds.October2018Balance, GW2Builds.July2019Balance, (evt, combatData) => {
                var buffsLossToCheck = new List<long>
                {
                    10235, 30739, 21751, 10231, 10246, 10233
                }; // signets
                foreach (long buffID in buffsLossToCheck)
                {
                    if (combatData.GetBuffData(buffID).Where(x => x.Time >= evt.Time - ParserHelper.ServerDelayConstant && x.Time <= evt.Time + ParserHelper.ServerDelayConstant && x is BuffRemoveAllEvent).Any())
                    {
                        return false;
                    }
                }
                return true;

            }), // Distortion
            new BuffGainCastFinder(10192, 10243, GW2Builds.July2019Balance, 104844, (evt, combatData) => {
                if (evt.To.Prof == "Chronomancer")
                {
                    return false;
                }
                var buffsLossToCheck = new List<long>
                {
                    10235, 30739, 21751, 10231, 10246, 10233
                }; // signets
                foreach (long buffID in buffsLossToCheck)
                {
                    if (combatData.GetBuffData(buffID).Where(x => x.Time >= evt.Time - ParserHelper.ServerDelayConstant && x.Time <= evt.Time + ParserHelper.ServerDelayConstant && x is BuffRemoveAllEvent).Any())
                    {
                        return false;
                    }
                }
                return true;

            }), // Distortion
            new BuffGainCastFinder(10192, 10243, 104844, GW2Builds.EndOfLife, (evt, combatData) => {
                var buffsLossToCheck = new List<long>
                {
                    10235, 30739, 21751, 10231, 10246, 10233
                }; // signets
                foreach (long buffID in buffsLossToCheck)
                {
                    if (combatData.GetBuffData(buffID).Where(x => x.Time >= evt.Time - ParserHelper.ServerDelayConstant && x.Time <= evt.Time + ParserHelper.ServerDelayConstant && x is BuffRemoveAllEvent).Any()) 
                    {
                        return false;
                    }
                }
                return true;
                
            }), // Distortion*/
            new EffectCastFinder(Feedback, EffectGUIDs.MesmerFeedback).UsingSrcBaseSpecChecker(Spec.Mesmer),

            // identify swap by buff remove
            // identify phase retreat by spawned staff clone
            // fallback to blink or phase retreat
            new EffectCastFinderByDst(Swap, EffectGUIDs.MesmerTeleport)
                .UsingDstBaseSpecChecker(Spec.Mesmer)
                .UsingChecker((evt, combatData, agentData, skillData) => HasLostBuffStack(combatData, IllusionaryLeapBuff, evt.Dst, evt.Time))
                .UsingNotAccurate(true),
            new EffectCastFinderByDst(PhaseRetreat, EffectGUIDs.MesmerTeleport)
                .UsingDstBaseSpecChecker(Spec.Mesmer)
                .UsingChecker((evt, combatData, agentData, skillData) => !HasLostBuffStack(combatData, IllusionaryLeapBuff, evt.Dst, evt.Time))
                .UsingChecker((evt, combatData, agentData, skillData) => HasSpawnedMinion(agentData, MinionID.CloneStaff, evt.Dst, evt.Time, 30))
                .UsingNotAccurate(true),
            new EffectCastFinderByDst(BlinkOrPhaseRetreat, EffectGUIDs.MesmerTeleport)
                .UsingDstBaseSpecChecker(Spec.Mesmer)
                .UsingChecker((evt, combatData, agentData, skillData) => !HasLostBuffStack(combatData, IllusionaryLeapBuff, evt.Dst, evt.Time))
                .UsingChecker((evt, combatData, agentData, skillData) => !HasSpawnedMinion(agentData, MinionID.CloneStaff, evt.Dst, evt.Time, 30))
                .UsingNotAccurate(true),

            new EffectCastFinder(MindWrack, EffectGUIDs.MesmerDistortionOrMindWrack).UsingChecker((evt, combatData, agentData, skillData) => !combatData.GetBuffData(DistortionBuff).Any(x => x.To == evt.Src && Math.Abs(x.Time - evt.Time) < ServerDelayConstant) && (evt.Src.Spec == Spec.Mesmer || evt.Src.Spec == Spec.Mirage)),
            new EffectCastFinder(CryOfFrustration, EffectGUIDs.MesmerCryOfFrustration).UsingChecker((evt, combatData, agentData, skillData) => (evt.Src.Spec == Spec.Mesmer || evt.Src.Spec == Spec.Mirage)),
            new EffectCastFinder(Diversion, EffectGUIDs.MesmerDiversion).UsingChecker((evt, combatData, agentData, skillData) => (evt.Src.Spec == Spec.Mesmer || evt.Src.Spec == Spec.Mirage)),
            new EffectCastFinder(DistortionSkill, EffectGUIDs.MesmerDistortionOrMindWrack).UsingChecker((evt, combatData, agentData, skillData) =>{
                if (evt.Src.Spec != Spec.Mesmer || evt.Src.Spec != Spec.Mirage)
                {
                    return false;
                }
                if (!combatData.GetBuffData(DistortionBuff).Any(x => x is BuffApplyEvent && x.To == evt.Src && Math.Abs(x.Time - evt.Time) < ServerDelayConstant))
                {
                    return false;
                }
                return true;
            }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
            new EffectCastFinder(DistortionSkill, EffectGUIDs.MesmerDistortionOrMindWrack).UsingChecker((evt, combatData, agentData, skillData) => {
                if (evt.Src.BaseSpec != Spec.Mesmer || evt.Src.Spec == Spec.Virtuoso)
                {
                    return false;
                }
                if (!combatData.GetBuffData(DistortionBuff).Any(x => x is BuffApplyEvent && x.To == evt.Src && Math.Abs(x.Time - evt.Time) < ServerDelayConstant))
                {
                    return false;
                }
                return true;
            }).WithBuilds(GW2Builds.October2022Balance),
            // Mantras        
            new DamageCastFinder(PowerSpike, PowerSpike).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new DamageCastFinder(MantraOfPain, MantraOfPain).WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
            new DamageCastFinder(PowerSpike, PowerSpike).WithBuilds(GW2Builds.February2023Balance),
            new EXTHealingCastFinder(MantraOfRecovery, MantraOfRecovery).WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
            new EffectCastFinderByDst(PowerReturn, EffectGUIDs.MesmerPowerReturn).UsingDstBaseSpecChecker(Spec.Mesmer).WithBuilds(GW2Builds.February2023Balance),
            new EffectCastFinder(MantraOfResolve, EffectGUIDs.MesmerMantraOfResolveAndPowerCleanse).UsingSrcBaseSpecChecker(Spec.Mesmer).WithBuilds(GW2Builds.StartOfLife ,GW2Builds.February2023Balance),
            new EffectCastFinder(PowerCleanse, EffectGUIDs.MesmerMantraOfResolveAndPowerCleanse).UsingSrcBaseSpecChecker(Spec.Mesmer).WithBuilds(GW2Builds.February2023Balance),
            new EffectCastFinderByDst(MantraOfConcentration, EffectGUIDs.MesmerMantraOfConcentrationAndPowerBreak).UsingDstBaseSpecChecker(Spec.Mesmer).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2023Balance),
            new EffectCastFinderByDst(PowerBreak, EffectGUIDs.MesmerMantraOfConcentrationAndPowerBreak).UsingDstBaseSpecChecker(Spec.Mesmer).WithBuilds(GW2Builds.February2023Balance),
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Domination
            // Empowered illusions require knowing all illusion species ID
            // We need illusion species ID to enable Vicious Expression on All
            new BuffDamageModifierTarget(NumberOfBoons, "Vicious Expression", "25% on boonless target",  DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByAbsence, BuffImages.ConfoundingSuggestions, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance, GW2Builds.February2020Balance2),
            new BuffDamageModifierTarget(NumberOfBoons, "Vicious Expression", "15% on boonless target",  DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByAbsence, BuffImages.ConfoundingSuggestions, DamageModifierMode.All).WithBuilds(GW2Builds.February2020Balance2),
            //
            new DamageLogDamageModifier("Egotism", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Mesmer, BuffImages.TemporalEnchanter, (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, DamageModifierMode.PvE).WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2023Balance).UsingApproximate(true),
            new DamageLogDamageModifier("Egotism", "5% if target hp% lower than self hp%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Mesmer, BuffImages.TemporalEnchanter, (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2023Balance).UsingApproximate(true),
            new DamageLogDamageModifier("Egotism", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Mesmer, BuffImages.TemporalEnchanter, (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, DamageModifierMode.All).WithBuilds(GW2Builds.February2023Balance).UsingApproximate(true),
            //
            new BuffDamageModifierTarget(Vulnerability, "Fragility", "0.5% per stack vuln on target", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, BuffImages.Fragility, DamageModifierMode.All),
            // Dueling
            // Superiority Complex can all the conditions be tracked?
            // Illusions
            new BuffDamageModifier(CompoundingPower, "Compounding Power", "2% per stack (8s) after creating an illusion ", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, BuffImages.CompoundingPower, DamageModifierMode.All),
            // Phantasmal Force: the current infrastructure is not capable of checking buffs on minions, once we have that, this does not require knowing illusion species id
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            // Signets
            new Buff("Signet of the Ether", SignetOfTheEther, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfTheEther),
            new Buff("Signet of Domination", SignetOfDomination, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfDomination),
            new Buff("Signet of Illusions", SignetOfIllusions, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfIllusions),
            new Buff("Signet of Inspiration", SignetOfInspirationBuff, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfInspiration),
            new Buff("Signet of Midnight", SignetOfMidnightBuff, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfMidnight),
            new Buff("Signet of Humility", SignetOfHumility, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfHumility),
            // Skills
            new Buff("Distortion", DistortionBuff, Source.Mesmer, BuffStackType.Queue, 25, BuffClassification.Other, BuffImages.Distortion),
            new Buff("Blur", Blur, Source.Mesmer, BuffClassification.Other, BuffImages.Distortion),
            new Buff("Mirror", Mirror, Source.Mesmer, BuffClassification.Other, BuffImages.Mirror),
            new Buff("Echo", Echo, Source.Mesmer, BuffClassification.Other, BuffImages.Echo),
            new Buff("Illusionary Counter", IllusionaryCounterBuff, Source.Mesmer, BuffClassification.Other, BuffImages.IllusionaryCounter),
            new Buff("Illusionary Riposte", IllusionaryRiposteBuff, Source.Mesmer, BuffClassification.Other, BuffImages.IllusionaryRiposte),
            new Buff("Illusionary Leap", IllusionaryLeapBuff, Source.Mesmer, BuffClassification.Other, BuffImages.IllusionaryLeap),
            new Buff("Portal Weaving", PortalWeaving, Source.Mesmer, BuffClassification.Other, BuffImages.PortalEnter),
            new Buff("Portal Uses", PortalUses, Source.Mesmer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.PortalEnter),
            new Buff("Illusion of Life", IllusionOfLife, Source.Mesmer, BuffClassification.Support, BuffImages.IllusionOfLife),
            new Buff("Time Echo", TimeEcho, Source.Mesmer, BuffClassification.Other, BuffImages.DejaVu).WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            // Traits
            new Buff("Fencer's Finesse", FencersFinesse , Source.Mesmer, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.FencersFinesse),
            new Buff("Illusionary Defense", IllusionaryDefense, Source.Mesmer, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.IllusionaryDefense),
            new Buff("Compounding Power", CompoundingPower, Source.Mesmer, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.CompoundingPower),
            new Buff("Phantasmal Force", PhantasmalForce, Source.Mesmer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Mistrust),
            new Buff("Reflection", Reflection, Source.Mesmer, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.ArcaneShield),
            new Buff("Reflection 2", Reflection2, Source.Mesmer, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.ArcaneShield),
            // Transformations
            new Buff("Morphed (Polymorph Moa)", MorphedPolymorphMoa, Source.Mesmer, BuffClassification.Debuff, BuffImages.MorphedPolymorphMoa),
            new Buff("Morphed (Polymorph Tuna)", MorphedPolymorphTuna, Source.Mesmer, BuffClassification.Debuff, BuffImages.MorphedPolymorphTuna),
        };

        private static readonly HashSet<int> _cloneIDs = new HashSet<int>()
        {
            (int)MinionID.CloneSword,
            (int)MinionID.CloneScepter,
            (int)MinionID.CloneAxe,
            (int)MinionID.CloneGreatsword,
            (int)MinionID.CloneStaff,
            (int)MinionID.CloneTrident,
            (int)MinionID.CloneSpear,
            (int)MinionID.CloneDagger,
            (int)MinionID.CloneDownstate,
            (int)MinionID.CloneUnknown,
            (int)MinionID.CloneSwordTorch,
            (int)MinionID.CloneSwordFocus,
            (int)MinionID.CloneSwordSword,
            (int)MinionID.CloneSwordShield,
            (int)MinionID.CloneSwordPistol,
            (int)MinionID.CloneIllusionaryLeap,
            (int)MinionID.CloneIllusionaryLeapFocus,
            (int)MinionID.CloneIllusionaryLeapShield,
            (int)MinionID.CloneIllusionaryLeapSword,
            (int)MinionID.CloneIllusionaryLeapPistol,
            (int)MinionID.CloneIllusionaryLeapTorch,
            (int)MinionID.CloneScepterTorch,
            (int)MinionID.CloneScepterShield,
            (int)MinionID.CloneScepterPistol,
            (int)MinionID.CloneScepterFocus,
            (int)MinionID.CloneScepterSword,
            (int)MinionID.CloneAxeTorch,
            (int)MinionID.CloneAxePistol,
            (int)MinionID.CloneAxeSword,
            (int)MinionID.CloneAxeFocus,
            (int)MinionID.CloneAxeShield,
            (int)MinionID.CloneDaggerShield,
            (int)MinionID.CloneDaggerPistol,
            (int)MinionID.CloneDaggerFocus,
            (int)MinionID.CloneDaggerTorch,
            (int)MinionID.CloneDaggerSword,
        };

        internal static void AdjustMinionName(AgentItem minion)
        {
            switch (minion.ID)
            {
                case (int)MinionID.CloneSpear:
                    minion.OverrideName("Spear " + minion.Name);
                    break;
                case (int)MinionID.CloneGreatsword:
                    minion.OverrideName("Greatsword " + minion.Name);
                    break;
                case (int)MinionID.CloneStaff:
                    minion.OverrideName("Staff " + minion.Name);
                    break;
                case (int)MinionID.CloneTrident:
                    minion.OverrideName("Trident " + minion.Name);
                    break;
                case (int)MinionID.CloneDownstate:
                    minion.OverrideName("Downstate " + minion.Name);
                    break;
                case (int)MinionID.CloneSword:
                case (int)MinionID.CloneSwordPistol:
                case (int)MinionID.CloneSwordTorch:
                case (int)MinionID.CloneSwordFocus:
                case (int)MinionID.CloneSwordSword:
                case (int)MinionID.CloneSwordShield:
                case (int)MinionID.CloneIllusionaryLeap:
                case (int)MinionID.CloneIllusionaryLeapFocus:
                case (int)MinionID.CloneIllusionaryLeapShield:
                case (int)MinionID.CloneIllusionaryLeapSword:
                case (int)MinionID.CloneIllusionaryLeapPistol:
                case (int)MinionID.CloneIllusionaryLeapTorch:
                    minion.OverrideName("Sword " + minion.Name);
                    break;
                case (int)MinionID.CloneScepter:
                case (int)MinionID.CloneScepterTorch:
                case (int)MinionID.CloneScepterShield:
                case (int)MinionID.CloneScepterPistol:
                case (int)MinionID.CloneScepterFocus:
                case (int)MinionID.CloneScepterSword:
                    minion.OverrideName("Scepter " + minion.Name);
                    break;
                case (int)MinionID.CloneAxe:
                case (int)MinionID.CloneAxeTorch:
                case (int)MinionID.CloneAxePistol:
                case (int)MinionID.CloneAxeSword:
                case (int)MinionID.CloneAxeFocus:
                case (int)MinionID.CloneAxeShield:
                    minion.OverrideName("Axe " + minion.Name);
                    break;
                case (int)MinionID.CloneDagger:
                case (int)MinionID.CloneDaggerShield:
                case (int)MinionID.CloneDaggerPistol:
                case (int)MinionID.CloneDaggerFocus:
                case (int)MinionID.CloneDaggerTorch:
                case (int)MinionID.CloneDaggerSword:
                    minion.OverrideName("Dagger " + minion.Name);
                    break;
                default:
                    break;
            }
        }

        internal static bool IsClone(AgentItem agentItem)
        {
            if (agentItem.Type == AgentItem.AgentType.Gadget)
            {
                return false;
            }
            return _cloneIDs.Contains(agentItem.ID);
        }

        private static bool IsClone(int id)
        {
            return _cloneIDs.Contains(id);
        }

        private static HashSet<int> NonCloneMinions = new HashSet<int>()
        {
            (int)MinionID.IllusionaryWarlock,
            (int)MinionID.IllusionaryWarden,
            (int)MinionID.IllusionarySwordsman,
            (int)MinionID.IllusionaryMage,
            (int)MinionID.IllusionaryDuelist,
            (int)MinionID.IllusionaryBerserker,
            (int)MinionID.IllusionaryDisenchanter,
            (int)MinionID.IllusionaryRogue,
            (int)MinionID.IllusionaryDefender,
            (int)MinionID.IllusionaryMariner,
            (int)MinionID.IllusionaryWhaler,
            (int)MinionID.IllusionaryAvenger,
        };

        internal static bool IsKnownMinionID(int id)
        {
            return NonCloneMinions.Contains(id) || IsClone(id);
        }
    }
}
