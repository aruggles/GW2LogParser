using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Mechanic;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity; 
using static GW2EIEvtcParser.MechanicIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class WhisperOfJormag : Bjora
{
    public WhisperOfJormag(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([ 
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(ChainsOfFrostHit, Mech_ChainsOfFrost, new (Symbols.DiamondTall, Colors.Red), new ("H.Chains", "Hit by Chains of Frost", "Chains of Frost"), Sev0),
                new PlayerDstBuffApplyMechanic(ChainsOfFrostApplication, Mech_ChainsOfFrostApply, new (Symbols.DiamondTall, Colors.LightRed), new ("F.Chains", "Selected for Chains of Frost", "Chains of Frost"), Sev1, 500),
                new EnemyCastStartMechanic(ChainsOfFrostHit, Mech_ChainsOfFrostCast, new (Symbols.Hexagram, Colors.LightRed), new ("F.Chains.C", "Cast Chains of Frost", "Cast Chains of Frost"), Sev3),
            ]),
            new PlayerDstHealthDamageHitMechanic(SlitheringRime, Mech_SlitheringRime, new (Symbols.CircleX, Colors.Red), new ("SlitRime.H", "Hit by Slithering Rime (Orbs)", "Slithering Rime"), Sev2),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(LethalCoalescenceSoaked, Mech_LethalCoalescenceSoaked, new (Symbols.Circle, Colors.Green), new ("S.Lethal.Coal.", "Soaked Lethal Coalescence Damage", "Soaked Lethal Coalescence"), Sev0, 50),
                new EnemyCastStartMechanic(LethalCoalescenceSoaked, Mech_LethalCoalescenceSoakedStart, new (Symbols.Circle, Colors.DarkGreen), new ("Lethal Coalescence", "Cast Lethal Coalescence", "Cast Lethal Coalescence"), Sev3, 50),
                new PlayerDstBuffApplyMechanic(LethalCoalescenceBuff, Mech_LethalCoalescenceBuff, new (Symbols.CircleOpenDot, Colors.Green), new ("LethalCoa.A", "Selected for Lethal Coalescence (Green)", "Lethal Coalescence"), Sev1, 500),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(SpreadingIceOwn, Mech_SpreadingIceOwn, new (Symbols.Circle, Colors.Orange), new ("S.Ice", "Hit by own Spreading Ice", "Spreading Ice (Own)"), Sev3, 50),
                new EnemyCastStartMechanic(SpreadingIceOwn, Mech_SpreadingIceOwnCast, new (Symbols.Hexagram, Colors.DarkRed), new ("S.Ice.C", "Cast Spreading Ice", "Cast Spreading Ice"), Sev3),
                new PlayerDstHealthDamageHitMechanic(SpreadingIceOthers, Mech_SpreadingIceOthers, new (Symbols.TriangleUp, Colors.LightOrange), new ("S.Ice.O", "Hit by other's Spreading Ice", "Spreading Ice (Others)"), Sev0, 50),
            ]),
            new PlayerDstHealthDamageHitMechanic(IcySlice, Mech_IcySlice, new (Symbols.Hexagram, Colors.Orange), new ("I.Slice", "Hit by Icy Slice", "Icy Slice"), Sev1, 50),
            new PlayerDstHealthDamageHitMechanic(IceTempest, Mech_IceTempest, new (Symbols.Square, Colors.Orange), new ("I.Tornado", "Hit by Ice Tempest (Tornadoes)", "Ice Tempest"), Sev1, 50),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(FrigidVortexDamage, Mech_FrigidVortex, new (Symbols.Star, Colors.Pink), new ("FrigVor.H", "Hit by Frigid Vortex", "Frigid Vortex Hit"), Sev0, 50),
                new EnemyCastStartMechanic(FrigidVortexSkill, Mech_FrigidVortexCast, new (Symbols.Star, Colors.Magenta), new ("Frigid Vortex", "Cast Frigid Vortex", "Cast Frigid Vortex"), Sev3, 50),
                new PlayerDstBuffApplyMechanic(FrigidVortexBuff, Mech_FrigidVortexApply, new (Symbols.Star, Colors.LightBlue), new ("FrigVor.A", "Frigid Vortex Applied", "Frigid Vortex Buff"), Sev1),
            ]),
            new PlayerDstHealthDamageHitMechanic([IceShatterWhisper4, IceShatterWhisper2, IceShatterWhisper1, IceShatterWhisper3], Mech_IceShatterWhisper, new (Symbols.Circle, Colors.Teal), new ("IceShatt.H", "Hit by Ice Shatter (Large AoEs)", "Ice Shatter"), Sev1, 150),
            new MechanicGroup([
                new PlayerDstBuffRemoveMechanic(WhisperTeleportBack, Mech_WhisperTPBack, new (Symbols.Circle, Colors.LightBlue), new ("TP In", "Teleported back to the arena", "Teleport Back"), Sev2, 500),
                new PlayerDstBuffRemoveMechanic(WhisperTeleportOut, Mech_WhisperTPOut, new (Symbols.CircleOpen, Colors.LightBlue), new ("TP Out", "Teleported outside of the arena", "Teleport Out"), Sev2, 500),
            ]),
            new EnemyCastStartMechanic([ViciousSlam1, ViciousSlam2], Mech_ViciousSlam, new (Symbols.TriangleUp, Colors.White),  new ("Vicious Slam", "Cast Vicious Slam (Launch)", "Vicious Slam (Launch)"), Sev1, 150),
        ])
        );
        Extension = "woj";
        Icon = EncounterIconWhisperOfJormag;
        LogCategoryInformation.InSubCategoryOrder = 3;
        LogID |= 0x000005;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        var crMap = new CombatReplayMap(
                        (1682, 1682),
                        (-3287, -1772, 3313, 4828));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayWhisperOfJormag, crMap, parentMap);
        return crMap;
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(FrostbiteAuraWhisperOfJormag, FrostbiteAuraWhisperOfJormag),
        ];
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor woj = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.WhisperOfJormag)) ?? throw new MissingKeyActorsException("Whisper of Jormag not found");
        phases[0].AddTarget(woj, log);
        if (!requirePhases)
        {
            return phases;
        }
        long start, end;
        var tpOutEvents = log.CombatData.GetBuffRemoveAllData(WhisperTeleportOut).ToList();
        var tpBackEvents = log.CombatData.GetBuffRemoveAllData(WhisperTeleportBack).ToList();
        // 75% tp happened
        if (tpOutEvents.Count > 0)
        {
            end = tpOutEvents.Min(x => x.Time);
            phases.Add(new SubPhasePhaseData(0, end, "Pre Doppelganger 1"));
            // remove everything related to 75% tp out
            tpOutEvents.RemoveAll(x => x.Time <= end + 1000);
        }
        // 75% tp finished
        if (tpBackEvents.Count > 0)
        {
            start = tpBackEvents.Min(x => x.Time);
            // 25% tp happened
            if (tpOutEvents.Count > 0)
            {
                end = tpOutEvents.Min(x => x.Time);
                tpOutEvents.Clear();
                tpBackEvents.RemoveAll(x => x.Time <= end);
            }
            // 25% tp did not happen
            else
            {
                end = log.LogData.LogEnd;
                tpBackEvents.Clear();
            }
            phases.Add(new SubPhasePhaseData(start, end, "Pre Doppelganger 2"));
            // 25% tp finished
            if (tpBackEvents.Count > 0)
            {
                start = tpBackEvents.Min(x => x.Time);
                phases.Add(new SubPhasePhaseData(start, log.LogData.LogEnd, "Final"));
            }
        }
        for (int i = 1; i < phases.Count; i++)
        {
            phases[i].AddTarget(woj, log);
            phases[i].AddParentPhase(phases[0]);
        }
        return phases;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.WhisperEcho,
            TargetID.DoppelgangerElementalist,
            TargetID.DoppelgangerElementalist2,
            TargetID.DoppelgangerEngineer,
            TargetID.DoppelgangerEngineer2,
            TargetID.DoppelgangerGuardian,
            TargetID.DoppelgangerGuardian2,
            TargetID.DoppelgangerMesmer,
            TargetID.DoppelgangerMesmer2,
            TargetID.DoppelgangerNecromancer,
            TargetID.DoppelgangerNecromancer2,
            TargetID.DoppelgangerRanger,
            TargetID.DoppelgangerRanger2,
            TargetID.DoppelgangerRevenant,
            TargetID.DoppelgangerRevenant2,
            TargetID.DoppelgangerThief,
            TargetID.DoppelgangerThief2,
            TargetID.DoppelgangerWarrior,
            TargetID.DoppelgangerWarrior2,
        ];
    }
    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
    }
    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }
}
