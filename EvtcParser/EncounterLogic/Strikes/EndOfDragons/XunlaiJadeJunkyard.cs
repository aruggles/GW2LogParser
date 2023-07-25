﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class XunlaiJadeJunkyard : EndOfDragonsStrike
    {
        public XunlaiJadeJunkyard(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(GraspingHorror, "GraspingHorror", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.LightOrange), "Hands.H", "Hit by Hands AoE", "Hands Hit", 150),
                new PlayerDstHitMechanic(DeathsEmbraceSkill, "Death's Embrace", new MechanicPlotlySetting(Symbols.CircleCross, Colors.DarkRed), "AnkkaPull.H", "Hit by Death's Embrace (Ankka's Pull)", "Death's Embrace Hit", 150),
                new PlayerDstHitMechanic(DeathsHandInBetween, "Death's Hand", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Yellow), "Sctn.AoE.H", "Hit by in-between sections AoE", "Death's Hand Hit (transitions)", 150),
                new PlayerDstHitMechanic(DeathsHandDropped, "Death's Hand", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Sprd.AoE.H", "Hit by placeable Death's Hand AoE", "Death's Hand Hit (placeable)", 150),
                new PlayerDstHitMechanic(WallOfFear, "Wall of Fear", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkRed), "Krait.H", "Hit by Krait AoE", "Krait Hit", 150),
                new PlayerDstHitMechanic(new long[] { WaveOfTormentNM, WaveOfTormentCM }, "Wave of Torment", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Quaggan.H", "Hit by Quaggan Explosion", "Quaggan Hit", 150),
                new PlayerDstHitMechanic(TerrifyingApparition, "Terrifying Apparition", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkRed), "Lich.H", "Hit by Lich AoE", "Lich Hit", 150),
                new PlayerDstHitMechanic(new long[] { WallOfFear, WaveOfTormentNM, WaveOfTormentCM, TerrifyingApparition }, "Clarity", new MechanicPlotlySetting(Symbols.DiamondTall, Colors.Blue), "Clarity.Achiv", "Achievement Eligibility: Clarity", "Achiv Clarity", 150).UsingAchievementEligibility(true),
                new PlayerDstBuffApplyMechanic(AnkkaLichHallucinationFixation, "Lich Fixation", new MechanicPlotlySetting(Symbols.Diamond, Colors.LightBlue), "Lich.H.F", "Fixated by Lich Hallucination", "Lich Fixation", 150),
                new PlayerDstHitMechanic(new long[] { ZhaitansReachThrashXJJ1, ZhaitansReachThrashXJJ2 }, "Thrash", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.DarkGreen), "ZhtRch.Pull", "Pulled by Zhaitan's Reach", "Zhaitan's Reach Pull", 150),
                new PlayerDstHitMechanic(new long[] { ZhaitansReachGroundSlam, ZhaitansReachGroundSlamXJJ }, "Ground Slam", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.DarkGreen), "ZhtRch.Knck", "Knocked by Zhaitan's Reach", "Zhaitan's Reach Knock", 150),
                new PlayerDstHitMechanic(ImminentDeathSkill, "Imminent Death", new MechanicPlotlySetting(Symbols.DiamondTall, Colors.Green), "Imm.Death.H", "Hit by Imminent Death", "Imminent Death Hit", 0),
                new EnemyCastStartMechanic(InevitabilityOfDeath, "Inevitability of Death", new MechanicPlotlySetting(Symbols.Octagon, Colors.LightRed), "Inev.Death.C", "Casted Inevitability of Death (Enrage)", "Inevitability of Death (Enrage)", 150),
                new EnemyCastStartMechanic(DeathsEmbraceSkill, "Death's Embrace", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Blue), "AnkkaPull.C", "Casted Death's Embrace", "Death's Embrace Cast", 150),
                new EnemyDstBuffApplyMechanic(PowerOfTheVoid, "Power of the Void", new MechanicPlotlySetting(Symbols.Star, Colors.Yellow), "Pwrd.Up", "Ankka has powered up", "Ankka powered up", 150),
                new PlayerDstBuffApplyMechanic(ImminentDeathBuff, "Imminent Death", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Green), "Imm.Death.B", "Placed Death's Hand AoE and gained Imminent Death Buff", "Imminent Death Buff", 150),
                new PlayerDstBuffApplyMechanic(FixatedAnkkaKainengOverlook, "Fixated", new MechanicPlotlySetting(Symbols.Diamond, Colors.Purple), "Fxt.Hatred", "Fixated by Reanimated Hatred", "Fixated Hatred", 150),
                new PlayerDstBuffApplyMechanic(Hallucinations, "Hallucinations", new MechanicPlotlySetting(Symbols.Square, Colors.LightBlue), "Hallu", "Received Hallucinations Debuff", "Hallucinations Debuff", 150),
                new PlayerDstBuffApplyMechanic(DeathsHandSpreadBuff, "Death's Hand Spread", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Green), "Sprd.AoE.B", "Received Death's Hand Spread", "Death's Hand Spread", 150),
                new PlayerDstBuffApplyMechanic(DevouringVoid, "Devouring Void", new MechanicPlotlySetting(Symbols.DiamondWide, Colors.LightBlue), "DevVoid.B", "Received Devouring Void", "Devouring Void Applied", 150),
                new PlayerDstBuffApplyMechanic(DevouringVoid, "Undevoured", new MechanicPlotlySetting(Symbols.DiamondWide, Colors.Blue), "Undev.Achiv", "Achievement Eligibility: Undevoured", "Achiv Undevoured", 150).UsingAchievementEligibility(true).UsingKeeper(x => x.FightData.IsCM),
            }
            );
            Icon = EncounterIconXunlaiJadeJunkyard;
            Extension = "xunjadejunk";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayXunlaiJadeJunkyard,
                            (1485, 1292),
                            (-7090, -2785, 3647, 6556)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
            if (ankka == null)
            {
                throw new MissingKeyActorsException("Ankka not found");
            }
            phases[0].AddTarget(ankka);
            if (!requirePhases)
            {
                return phases;
            }

            // DPS Phases
            List<PhaseData> dpsPhase = GetPhasesByInvul(log, Determined895, ankka, false, true);
            for (int i = 0; i < dpsPhase.Count; i++)
            {
                dpsPhase[i].Name = $"DPS Phase {i + 1}";
                dpsPhase[i].AddTarget(ankka);
            }
            phases.AddRange(dpsPhase);

            // Necrotic Rituals
            List<PhaseData> rituals = GetPhasesByInvul(log, NecroticRitual, ankka, true, true);
            for (int i = 0; i < rituals.Count; i++)
            {
                if (i % 2 != 0)
                {
                    rituals[i].Name = $"Necrotic Ritual {(i + 1) / 2}";
                    rituals[i].AddTarget(ankka);
                }
            }
            phases.AddRange(rituals);

            // Health and Transition Phases
            List<PhaseData> subPhases = GetPhasesByInvul(log, AnkkaPlateformChanging, ankka, true, true);
            for (int i = 0; i < subPhases.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        subPhases[i].Name = "Phase 100-75%";
                        break;
                    case 1:
                        subPhases[i].Name = "Transition 1";
                        break;
                    case 2:
                        subPhases[i].Name = "Phase 75-40%";
                        break;
                    case 3:
                        subPhases[i].Name = "Transition 2";
                        break;
                    case 4:
                        subPhases[i].Name = "Phase 40-0%";
                        break;
                    default:
                        break;
                }
                subPhases[i].AddTarget(ankka);
            }
            phases.AddRange(subPhases);
            //
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
                if (ankka == null)
                {
                    throw new MissingKeyActorsException("Ankka not found");
                }
                var buffApplies = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == ankka.AgentItem && !x.Initial && x.AppliedDuration > int.MaxValue / 2 && x.Time >= fightData.FightStart + 5000).ToList();
                if (buffApplies.Count == 3)
                {
                    fightData.SetSuccess(true, buffApplies.LastOrDefault().Time);
                }
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Ankka,
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Ankka,
                (int)ArcDPSEnums.TrashID.ReanimatedAntipathy,
                (int)ArcDPSEnums.TrashID.ReanimatedSpite,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Ankka,
                ArcDPSEnums.TrashID.ReanimatedMalice1,
                ArcDPSEnums.TrashID.ReanimatedMalice2,
                ArcDPSEnums.TrashID.ReanimatedHatred,
                ArcDPSEnums.TrashID.ZhaitansReach,
                ArcDPSEnums.TrashID.KraitsHallucination,
                ArcDPSEnums.TrashID.LichHallucination,
                ArcDPSEnums.TrashID.QuaggansHallucinationNM,
                ArcDPSEnums.TrashID.QuaggansHallucinationCM,
                ArcDPSEnums.TrashID.SanctuaryPrism,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
            if (ankka == null)
            {
                throw new MissingKeyActorsException("Ankka not found");
            }
            MapIDEvent map = combatData.GetMapIDEvents().FirstOrDefault();
            if (map != null && map.MapID == 1434)
            {
                return FightData.EncounterMode.Story;
            }
            return ankka.GetHealth(combatData) > 50e6 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var sanctuaryPrism = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget).ToList();
            foreach (AgentItem sanctuary in sanctuaryPrism)
            {
                IEnumerable<CombatItem> items = combatData.Where(x => x.SrcMatchesAgent(sanctuary) && x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && x.DstAgent == 0);
                sanctuary.OverrideType(AgentItem.AgentType.NPC);
                sanctuary.OverrideID(ArcDPSEnums.TrashID.SanctuaryPrism);
                sanctuary.OverrideAwareTimes(fightData.LogStart, items.Any() ? items.FirstOrDefault().Time : fightData.LogEnd);
            }
            agentData.Refresh();
            ComputeFightTargets(agentData, combatData, extensions);
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);

            if (log.FightData.Success && log.FightData.IsCM && CustomCheckGazeIntoTheVoidEligibility(log))
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityGazeIntoTheVoid], 1));
            }
        }

        private static bool CustomCheckGazeIntoTheVoidEligibility(ParsedEvtcLog log)
        {
            IReadOnlyList<AgentItem> agents = log.AgentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Ankka);

            foreach (AgentItem agent in agents)
            {
                IReadOnlyDictionary<long, BuffsGraphModel> bgms = log.FindActor(agent).GetBuffGraphs(log);
                if (bgms != null && bgms.TryGetValue(PowerOfTheVoid, out BuffsGraphModel bgm))
                {
                    if (bgm.BuffChart.Any(x => x.Value == 6)) { return true; }
                }
            }
            return false;
        }

        private static (EffectGUIDEvent guid, int radius, int duration) GetDeathsHandOnPlayerData(ParsedEvtcLog log)
        {
            return log.FightData.IsCM ? (log.CombatData.GetEffectGUIDEvent(EffectGUIDs.DeathsHandByAnkkaCM), 380, 36000): (log.CombatData.GetEffectGUIDEvent(EffectGUIDs.DeathsHandByAnkkaNM), 300, 16000);
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Ankka:
                    var deathsEmbraces = casts.Where(x => x.SkillId == DeathsEmbraceSkill).ToList();
                    int deathsEmbraceCastDuration = 10143;
                    foreach (AbstractCastEvent deathEmbrace in deathsEmbraces)
                    {
                        int endTime = (int)deathEmbrace.Time + deathsEmbraceCastDuration;

                        Point3D ankkaPosition = target.GetCurrentPosition(log, deathEmbrace.Time);
                        if (ankkaPosition == null) { continue; }

                        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DeathsEmbrace, out IReadOnlyList<EffectEvent> deathsEmbraceEffects))
                        {
                            int radius = 500; // Zone 1
                            // Zone 2
                            if (ankkaPosition.X > 0 && ankkaPosition.X < 4000)
                            {
                                radius = 340;
                            }
                            // Zone 3
                            if (ankkaPosition.Y > 4000 && ankkaPosition.Y < 6000)
                            {
                                radius = 380;
                            }
                            var effects = deathsEmbraceEffects.Where(x => x.Time >= deathEmbrace.Time && x.Time <= deathEmbrace.EndTime).ToList();
                            foreach (EffectEvent effectEvt in effects)
                            {
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, radius, (int)(effectEvt.Time - deathEmbrace.Time), effectEvt.Position);
                            }
                        }
                        else
                        {
                            // logs without effects
                            int delay = 1833 * 2;
                            // Zone 1
                            if (ankkaPosition.X > -6000 && ankkaPosition.X < -2500 && ankkaPosition.Y < 1000 && ankkaPosition.Y > -1000)
                            {
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 500, delay, new Point3D(-3941.78f, 66.76819f, -3611.2f)); // CENTER
                            }
                            // Zone 2
                            if (ankkaPosition.X > 0 && ankkaPosition.X < 4000)
                            {
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 340, delay, new Point3D(1663.69f, 1739.87f, -4639.695f)); // NW
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 340, delay, new Point3D(2563.689f, 1739.87f, -4664.611f)); // NE
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 340, delay, new Point3D(1663.69f, 839.8699f, -4640.633f)); // SW
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 340, delay, new Point3D(2563.689f, 839.8699f, -4636.368f)); // SE
                            }
                            // Zone 3
                            if (ankkaPosition.Y > 4000 && ankkaPosition.Y < 6000)
                            {
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 380, delay, new Point3D(-2547.61f, 5466.439f, -6257.504f)); // NW
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 380, delay, new Point3D(-1647.61f, 5466.439f, -6256.795f)); // NE
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 380, delay, new Point3D(-2547.61f, 4566.439f, -6256.799f)); // SW
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 380, delay, new Point3D(-1647.61f, 4566.439f, -6257.402f)); // SE
                            }
                        }
                    }
                    (EffectGUIDEvent deathsHandGUID, int deathsHandOnPlayerRadius, int deathsHandOnPlayerDuration) = GetDeathsHandOnPlayerData(log);
                    if (deathsHandGUID != null)
                    {
                        var deathsHandEffects = log.CombatData.GetEffectEventsByEffectID(deathsHandGUID.ContentID).Where(x => x.Src == target.AgentItem).ToList();
                        foreach (EffectEvent deathsHandEffect in deathsHandEffects)
                        {
                            if (!log.CombatData.GetBuffRemoveAllData(DeathsHandSpreadBuff).Any(x => Math.Abs(x.Time - deathsHandEffect.Time) < ServerDelayConstant))
                            {
                                // One also happens during death's embrace so we filter that one out
                                if (!deathsEmbraces.Any(x => x.Time <= deathsHandEffect.Time && x.Time + deathsEmbraceCastDuration >= deathsHandEffect.Time))
                                {

                                    AddDeathsHandDecoration(replay, deathsHandEffect.Position, (int)deathsHandEffect.Time, 3000, 380, 1000);
                                }
                            } 
                            else
                            {
                                AddDeathsHandDecoration(replay, deathsHandEffect.Position, (int)deathsHandEffect.Time, 3000, deathsHandOnPlayerRadius, deathsHandOnPlayerDuration);
                            }
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.KraitsHallucination:
                    // Wall of Fear
                    int firstMovementTime = 2550;
                    int kraitsRadius = 420;

                    replay.Decorations.Add(new CircleDecoration(true, (int)target.FirstAware + firstMovementTime, kraitsRadius, ((int)target.FirstAware, (int)target.FirstAware + firstMovementTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    replay.Decorations.Add(new CircleDecoration(true, 0, kraitsRadius, ((int)target.FirstAware + firstMovementTime, (int)target.LastAware), "rgba(250, 0, 0, 0.2)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.LichHallucination:
                    // Terrifying Apparition
                    int awareTime = 1000;
                    int lichRadius = 280;

                    replay.Decorations.Add(new CircleDecoration(true, (int)target.FirstAware + awareTime, lichRadius, ((int)target.FirstAware, (int)target.FirstAware + awareTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    replay.Decorations.Add(new CircleDecoration(true, 0, lichRadius, ((int)target.FirstAware + awareTime, (int)target.LastAware), "rgba(250, 0, 0, 0.2)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.QuaggansHallucinationNM:
                    var waveOfTormentNM = casts.Where(x => x.SkillId == WaveOfTormentNM).ToList();
                    foreach (AbstractCastEvent c in waveOfTormentNM)
                    {
                        int castTime = 2800;
                        int radius = 300;
                        int endTime = (int)c.Time + castTime;

                        replay.Decorations.Add(new CircleDecoration(true, endTime, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.QuaggansHallucinationCM:
                    var waveOfTormentCM = casts.Where(x => x.SkillId == WaveOfTormentCM).ToList();
                    foreach (AbstractCastEvent c in waveOfTormentCM)
                    {
                        int castTime = 5600;
                        int radius = 450;
                        int endTime = (int)c.Time + castTime;

                        replay.Decorations.Add(new CircleDecoration(true, endTime, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.ZhaitansReach:
                    // Thrash - Circle that pulls in
                    var thrash = casts.Where(x => x.SkillId == ZhaitansReachThrashXJJ1 || x.SkillId == ZhaitansReachThrashXJJ2).ToList();
                    foreach (AbstractCastEvent c in thrash)
                    {
                        int castTime = 1900;
                        int endTime = (int)c.Time + castTime;

                        replay.Decorations.Add(new DoughnutDecoration(true, endTime, 300, 500, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new DoughnutDecoration(true, 0, 300, 500, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    // Ground Slam - AoE that knocks out
                    var groundSlam = casts.Where(x => x.SkillId == ZhaitansReachGroundSlam || x.SkillId == ZhaitansReachGroundSlamXJJ).ToList();
                    foreach (AbstractCastEvent c in groundSlam)
                    {
                        int castTime = 0;
                        int radius = 400;
                        int endTime = 0;
                        // 66534 -> Fast AoE -- 66397 -> Slow AoE
                        if (c.SkillId == ZhaitansReachGroundSlam) { castTime = 800; } else if (c.SkillId == ZhaitansReachGroundSlamXJJ) { castTime = 2500; }
                        endTime = (int)c.Time + castTime;

                        replay.Decorations.Add(new CircleDecoration(true, endTime, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.ReanimatedSpite:
                    break;
                case (int)ArcDPSEnums.TrashID.SanctuaryPrism:
                    if (!log.FightData.IsCM)
                    {
                        replay.Trim(log.FightData.LogStart, log.FightData.LogStart);
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            (EffectGUIDEvent deathsHandOnPlayerGUID, int deathsHandRadius, int deathsHandDuration) = GetDeathsHandOnPlayerData(log);
            if (p.GetBuffGraphs(log).TryGetValue(DeathsHandSpreadBuff, out BuffsGraphModel value))
            {
                foreach (Segment segment in value.BuffChart)
                {
                    if (segment != null && segment.Start > 0 && segment.Value == 1)
                    {
                        // AoE on player
                        replay.Decorations.Add(new CircleDecoration(true, (int)segment.End, deathsHandRadius, ((int)segment.Start, (int)segment.End), "rgba(250, 120, 0, 0.2)", new AgentConnector(p)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, deathsHandRadius, ((int)segment.Start, (int)segment.End), "rgba(250, 120, 0, 0.2)", new AgentConnector(p)));
                        // Logs without effects
                        if (deathsHandOnPlayerGUID == null)
                        {
                            ParametricPoint3D playerPosition = p.GetCombatReplayPolledPositions(log).Where(x => x.Time <= (int)segment.End).LastOrDefault();
                            if (playerPosition != null)
                            {
                                AddDeathsHandDecoration(replay, playerPosition, (int)segment.End, 3000, deathsHandRadius, deathsHandDuration);
                            }
                        }
                    }
                }
            }
            //
            List<AbstractBuffEvent> lichTethers = GetFilteredList(log.CombatData, AnkkaLichHallucinationFixation, p, true, true);
            int lichTetherStart = 0;
            AbstractSingleActor lichTetherSource = null;
            foreach (AbstractBuffEvent lichTether in lichTethers)
            {
                if (lichTether is BuffApplyEvent)
                {
                    lichTetherStart = (int)lichTether.Time;
                    lichTetherSource = TrashMobs.FirstOrDefault(x => x.AgentItem == lichTether.CreditedBy);
                }
                else
                {
                    int tetherEnd = (int)lichTether.Time;
                    if (lichTetherSource != null)
                    {
                        replay.Decorations.Add(new LineDecoration(0, (lichTetherStart, tetherEnd), "rgba(0, 255, 255, 0.5)", new AgentConnector(p), new AgentConnector(lichTetherSource)));
                    }
                }
            }
        }

        private static void AddDeathsHandDecoration(CombatReplay replay, Point3D position, int start, int delay, int radius, int duration)
        {
            // Growing AoE
            replay.Decorations.Add(new CircleDecoration(true, start + delay, radius, (start, start + delay), "rgba(250, 120, 0, 0.2)", new PositionConnector(position)));
            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, start + delay), "rgba(250, 120, 0, 0.2)", new PositionConnector(position)));
            // Damaging AoE
            replay.Decorations.Add(new DoughnutDecoration(true, 0, radius - 10, radius, (start + delay, start + duration), "rgba(255, 0, 0, 0.4)", new PositionConnector(position)));
            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start + delay, start + duration), "rgba(0, 100, 0, 0.2)", new PositionConnector(position)));
        }

        private static void AddDeathEmbraceDecoration(CombatReplay replay, int startCast, int durationCast, int radius, int delay, Point3D position)
        {
            int endTime = startCast + durationCast;
            var connector = new PositionConnector(position);
            replay.Decorations.Add(new CircleDecoration(true, startCast + delay, radius, (startCast, startCast + delay), "rgba(250, 120, 0, 0.2)", connector));
            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (startCast + delay, endTime), "rgba(250, 0, 0, 0.2)", connector));
        }
    }
}
