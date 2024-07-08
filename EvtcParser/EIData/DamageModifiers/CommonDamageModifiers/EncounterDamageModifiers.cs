﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class EncounterDamageModifiers
    {
        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnFoeDamageModifier(UnnaturalSignet, "Unnatural Signet", "200%, stacks additively with Vulnerability", DamageSource.All, 200.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.UnnaturalSignet, DamageModifierMode.PvE).UsingGainAdjuster(VulnerabilityAdjuster),
            new BuffOnFoeDamageModifier(Compromised, "Compromised", "75% per stack", DamageSource.All, 75.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Compromised, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(ErraticEnergy, "Erratic Energy", "5% per stack, stacks additively with Vulnerability", DamageSource.All, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Unstable, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster),
            new BuffOnFoeDamageModifier(FracturedEnemy, "Fractured - Enemy", "50% per stack", DamageSource.All, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.ExposedWeakness, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(DiaphanousShielding, "Diaphanous Shielding", "-10% per stack, stacks additively with Vulnerability", DamageSource.All, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.DiaphanousShielding, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster),
            new BuffOnFoeDamageModifier(CacophonousMind, "Cacophonous Mind", "-5% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.TwistedEarth, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster)
                .UsingChecker((ahde, log) =>
                {
                    return VulnerabilityAdditiveChecker(ahde, log, CacophonousMind, 5);
                }),
            new CounterOnFoeDamageModifier(CacophonousMind, "Cacophonous Mind (Invul)", "-5% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.TwistedEarth, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !VulnerabilityAdditiveChecker(ahde, log, CacophonousMind, 5);
                }),
            new BuffOnFoeDamageModifier(DagdaDemonicAura, "Demonic Aura", "-10% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.ChampionOfTheCrown, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster)
                .UsingChecker((ahde, log) =>
                {
                    return VulnerabilityAdditiveChecker(ahde, log, DagdaDemonicAura, 10);
                }),
            new CounterOnFoeDamageModifier(DagdaDemonicAura, "Demonic Aura (Invul)", "-10% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.ChampionOfTheCrown, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !VulnerabilityAdditiveChecker(ahde, log, DagdaDemonicAura, 10);
                }),
            new BuffOnFoeDamageModifier(PowerOfTheVoid, "Power of the Void", "-25% per stack, multiplicative with itself", DamageSource.All, -25.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByMultipliyingStack, BuffImages.PowerOfTheVoid, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(PillarPandemonium, "Pillar Pandemonium", "-20% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -20.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.CaptainsInspiration, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster)
                .UsingChecker((ahde, log) =>
                {
                    return VulnerabilityAdditiveChecker(ahde, log, PillarPandemonium, 20);
                }),
            new CounterOnFoeDamageModifier(PillarPandemonium, "Pillar Pandemonium (Invul)", "-20% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.CaptainsInspiration, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !VulnerabilityAdditiveChecker(ahde, log, PillarPandemonium, 20);
                }),
            new BuffOnFoeDamageModifier(ShieldedCA, "Shielded CA", "-100% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -100.0, DamageType.Condition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.CaptainsInspiration, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster)
                .UsingChecker((ahde, log) =>
                {
                    return VulnerabilityAdditiveChecker(ahde, log, ShieldedCA, 100);
                }),
            new CounterOnFoeDamageModifier(ShieldedCA, "Shielded CA (Invul)", "-100% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.Condition, DamageType.All, Source.FightSpecific, BuffImages.PoweredShielding, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !VulnerabilityAdditiveChecker(ahde, log, ShieldedCA, 100);
                }),
            new BuffOnFoeDamageModifier(IonShield, "Ion Shield", "-5% per stack, while still capable of doing damage", DamageSource.All, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.IonShield, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    AbstractSingleActor target = log.FindActor(ahde.To);
                    Segment segment = target.GetBuffStatus(log, IonShield, ahde.Time);
                    return segment.Value < 20;
                }),
            new CounterOnFoeDamageModifier(IonShield, "Ion Shield (Invul)", "-5% per stack, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.IonShield, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    AbstractSingleActor target = log.FindActor(ahde.To);
                    Segment segment = target.GetBuffStatus(log, IonShield, ahde.Time);
                    return segment.Value >= 20;
                }),
            new BuffOnFoeDamageModifier(IcyBarrier, "Icy Barrier", "-10% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.ShieldOfIce, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster)
                .UsingChecker((ahde, log) =>
                {
                    return VulnerabilityAdditiveChecker(ahde, log, IcyBarrier, 10);
                }),
            new CounterOnFoeDamageModifier(IcyBarrier, "Icy Barrier (Invul)", "-10% per stack, stacks additively with Vulnerability, while doing 0 damage", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.ShieldOfIce, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !VulnerabilityAdditiveChecker(ahde, log, IcyBarrier, 10);
                }),
            new BuffOnActorDamageModifier(EmpoweredStatueOfDeath, "Empowered (Statue of Death)", "50%", DamageSource.NoPets, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.EmpoweredEater, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(ViolentCurrents, "Violent Currents", "5% per stack", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.ViolentCurrents, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(new long[] {BloodShield, BloodShieldAbo}, "Blood Shield", "-90% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.NoPets, -90.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.BloodShield, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster)
                .UsingChecker((ahde, log) =>
                {
                    return VulnerabilityAdditiveChecker(ahde, log, BloodShield, 90) || VulnerabilityAdditiveChecker(ahde, log, BloodShieldAbo, 90);
                }),
            new CounterOnFoeDamageModifier(new long[] {BloodShield, BloodShieldAbo}, "Blood Shield (invul)", "-90% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.NoPets, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.BloodShield, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !(VulnerabilityAdditiveChecker(ahde, log, BloodShield, 90) || VulnerabilityAdditiveChecker(ahde, log, BloodShieldAbo, 90));
                }),
            new BuffOnFoeDamageModifier(LethalInspiration, "Lethal Inspiration", "-90%, stacks additively with Vulnerability", DamageSource.NoPets, -90.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.PowerOfTheVoid, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster),
            new BuffOnActorDamageModifier(BloodFueledPlayer, "Blood Fueled", "10% per stack", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.BloodFueled, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(FractalSavant, "Fractal Savant", "1%", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Malign9Infusion, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(FractalProdigy, "Fractal Prodigy", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Mighty9Infusion, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(FractalChampion, "Fractal Champion", "4%", DamageSource.NoPets, 4.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Precise9Infusion, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(FractalGod, "Fractal God", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Healing9Infusion, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(SoulReunited, "Soul Reunited", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.AllysAidPoweredUp, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(Phantasmagoria, "Phantasmagoria", "50%", DamageSource.NoPets, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.VoidAffliction, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(StickingTogetherBuff, "Sticking Together", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.ActivateGreen, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(CrushingGuilt, "Crushing Guilt", "-5% per stack", DamageSource.NoPets, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.GuiltExploitation, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(Debilitated, "Debilitated", "-25% per stack", DamageSource.NoPets, -25.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Debilitated, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(SappingSurge, "Sapping Surge", "-25%", DamageSource.NoPets, -25.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.GuiltExploitation, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DebilitatedToxicSickness, "Debilitated (Toxic Sickness)", "-10% per stack", DamageSource.NoPets, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Debilitated, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(SpectralDarkness, "Spectral Darkness", "-5% per stack", DamageSource.NoPets, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.SpectralDarkness, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor1, "Dragon's End Contributor 1", "1%", DamageSource.NoPets, 1.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale01, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor2, "Dragon's End Contributor 2", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale02, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor3, "Dragon's End Contributor 3", "3%", DamageSource.NoPets, 3.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale03, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor4, "Dragon's End Contributor 4", "4%", DamageSource.NoPets, 4.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale04, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor5, "Dragon's End Contributor 5", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale05, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor6, "Dragon's End Contributor 6", "6%", DamageSource.NoPets, 6.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale06, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor7, "Dragon's End Contributor 7", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale07, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor8, "Dragon's End Contributor 8", "8%", DamageSource.NoPets, 8.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale08, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor9, "Dragon's End Contributor 9", "9%", DamageSource.NoPets, 9.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale09, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor10, "Dragon's End Contributor 10", "20%", DamageSource.NoPets, 20.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale10, DamageModifierMode.PvE),
            // Enrages
            new BuffOnFoeDamageModifier(Enraged_100_strike_25_reduc, "Enraged (-25%)", "-25%, stacks additively with Vulnerability", DamageSource.All, -25, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE).UsingGainAdjuster(VulnerabilityAdjuster),
            new BuffOnFoeDamageModifier(Enraged_200_strike_50_reduc, "Enraged (-50%)", "-50%, stacks additively with Vulnerability", DamageSource.All, -50, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE).UsingGainAdjuster(VulnerabilityAdjuster),
            new BuffOnFoeDamageModifier(Enraged_300_strike_75_reduc, "Enraged (-75%)", "-75%, stacks additively with Vulnerability", DamageSource.All, -75, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE).UsingGainAdjuster(VulnerabilityAdjuster),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {

            new BuffOnActorDamageModifier(ConjuredProtection, "Conjured Protection", "-10% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -10.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Fractured, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster)
                .UsingChecker((ahde, log) =>
                {
                    return VulnerabilityAdditiveChecker(ahde, log, ConjuredProtection, 10);
                }),
            new CounterOnActorDamageModifier(ConjuredProtection, "Conjured Protection (Invul)", "-10% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.Strike, DamageType.All, Source.FightSpecific, BuffImages.Fractured, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !VulnerabilityAdditiveChecker(ahde, log, ConjuredProtection, 10);
                }),
            //
            new BuffOnFoeDamageModifier(DiaphanousShielding, "Diaphanous Shielding", "25% per stack", DamageSource.All, 25.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.DiaphanousShielding, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(BloodFueledMatthias, "Blood Fueled Abo", "10% per stack", DamageSource.All, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.BloodFueled, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(EmpoweredMO, "Empowered (MO)", "25% per stack", DamageSource.All, 25.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Empowered, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(StrengthenedBondGuldhem, "Strengthened_Bond:_Guldhem", "10% per stack", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.StrengthenedBondGuldhem, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(Devour, "Devour", "2% per stack", DamageSource.All, 2.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Devour, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(new long[] {AquaticAuraKenut, AquaticAuraNikare}, "Aquatic Aura", "2% per stack", DamageSource.All, 2.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.ExposeWeakness, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(FracturedAllied, "Fractured (Ally)", "50% per stack", DamageSource.All, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Fractured, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(FierySurge, "Fiery Surge", "20% per stack", DamageSource.All, 20.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.FierySurge, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(AugmentedPower, "Augmented Power", "10% per stack", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.FierySurge, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(IonShield, "Ion Shield", "3% per stack", DamageSource.All, 3.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.IonShield, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(PillarPandemonium, "Pillar Pandemonium", "20% per stack", DamageSource.All, 20.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.CaptainsInspiration, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(CacophonousMind, "Cacophonous Mind", "5% per stack", DamageSource.All, 5, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.TwistedEarth, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(PowerOfTheVoid, "Power of theVoid", "25% per stack", DamageSource.All, 25, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.PowerOfTheVoid, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(EmpoweredWatchknightTriumverate, "Empowered (Watchknight Triumverate)", "5% per stack", DamageSource.All, 5, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Empowered, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(EmpoweredCerus, "Empowered (Cerus)", "5% per stack", DamageSource.All, 5, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.EmpoweredMursaarOverseer, DamageModifierMode.PvE),
            // Enrages
            new BuffOnFoeDamageModifier(new long[]{Enraged_100_strike, Enraged_100_strike_25_reduc }, "Enraged (100% strike)", "100%", DamageSource.All, 100.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(Enraged_200, "Enraged (200%)", "200%", DamageSource.All, 200.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(new long[]{Enraged_200_strike, Enraged_200_strike_50_reduc }, "Enraged (200% strike)", "200%", DamageSource.All, 200.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(Enraged_300_strike_75_reduc, "Enraged (300% strike)", "300%", DamageSource.All, 300.0, DamageType.Strike, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(Enraged_500, "Enraged (500%)", "500%", DamageSource.All, 500.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(EnragedFractal, "Enraged (Fractal)", "110%", DamageSource.All, 110, DamageType.Strike, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(EnragedCairn2, "Enraged (Cairn)", "200% per stack", DamageSource.All, 200, DamageType.Strike, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Enraged, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(EnragedTwinLargos, "Enraged (Twin Largos)", "100%", DamageSource.All, 100, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(EnragedWyvern, "Enraged (Wyvern)", "100%", DamageSource.All, 100, DamageType.Strike, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
        };
    }
}
