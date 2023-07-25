﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class SoullessHorror : HallOfChains
    {
        public SoullessHorror(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new PlayerDstHitMechanic(InnerVortexSlash, "Vortex Slash", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Donut In","Vortex Slash (Inner Donut hit)", "Inner Donut",0),
            new PlayerDstHitMechanic(OuterVortexSlash, "Vortex Slash", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Donut Out","Vortex Slash (Outer Donut hit)", "Outer Donut", 0),
            new PlayerDstHitMechanic(new long[] { InnerVortexSlash, OuterVortexSlash }, "Necro Dancer", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightOrange), "NecDancer.Achiv", "Achievement Eligibility: Necro Dancer", "Necro Dancer", 0).UsingAchievementEligibility(true),
            new PlayerDstHitMechanic(SoulRift, "Soul Rift", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Golem","Soul Rift (stood in Golem Aoe)", "Golem Aoe",0),
            new PlayerDstHitMechanic(QuadSlashFirstSet, "Quad Slash", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.LightOrange), "Slice1","Quad Slash (4 Slices, First hit)", "4 Slices 1",0),
            new PlayerDstHitMechanic(QuadSlashSecondSet, "Quad Slash", new MechanicPlotlySetting(Symbols.StarSquareOpen,Colors.LightOrange), "Slice2","Quad Slash (4 Slices, Second hit)", "4 Slices 2",0),
            new PlayerDstHitMechanic(SpinningSlash, "Spinning Slash", new MechanicPlotlySetting(Symbols.StarTriangleUpOpen,Colors.DarkRed), "Scythe","Spinning Slash (hit by Scythe)", "Scythe",0),
            new PlayerDstHitMechanic(DeathBloom, "Death Bloom", new MechanicPlotlySetting(Symbols.Octagon,Colors.LightOrange), "8Slice","Death Bloom (8 Slices)", "8 Slices",0),
            new PlayerDstBuffApplyMechanic(FixatedSH, "Fixated", new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixate","Fixated (Special Action Key)", "Fixated",0),
            new PlayerDstBuffApplyMechanic(Necrosis, "Necrosis", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Magenta), "Necrosis","Necrosis (Tanking Debuff)", "Necrosis Debuff",50),
            new PlayerDstHitMechanic(CorruptTheLiving, "Corrupt the Living", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Spin","Corrupt the Living (Torment+Poison Spin)", "Torment+Poison Spin",0),
            new PlayerDstHitMechanic(WurmSpit, "Wurm Spit", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.DarkTeal), "Spit","Wurm Spit", "Wurm Spit",0),
            new EnemyCastStartMechanic(HowlingDeath, "Howling Death", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Howling Death (Breakbar)", "Breakbar",0),
            new EnemyCastEndMechanic(HowlingDeath, "Howling Death", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Howling Death (Breakbar) broken", "CCed",0).UsingChecker((ce, log) => ce.ActualDuration <= 6800),
            new EnemyCastEndMechanic(HowlingDeath, "Howling Death", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","Howling Death (Breakbar failed) ", "CC Fail",0).UsingChecker((ce,log) => ce.ActualDuration > 6800),

            });
            Extension = "sh";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = EncounterIconSoullessHorror;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(ChillingAura, ChillingAura), // Chilling Aura
            };
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplaySoullessHorror,
                            (1000, 1000),
                            (-12223, -771, -8932, 2420)/*,
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508)*/);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Scythe,
                ArcDPSEnums.TrashID.TormentedDead,
                ArcDPSEnums.TrashID.SurgingSoul,
                ArcDPSEnums.TrashID.FleshWurm
            };
        }

        internal override List<ErrorEvent> GetCustomWarningMessages(FightData fightData, int arcdpsVersion)
        {
            List<ErrorEvent> res = base.GetCustomWarningMessages(fightData, arcdpsVersion);
            res.AddRange(GetConfusionDamageMissingMessage(arcdpsVersion));
            return res;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.SoullessHorror));
                if (mainTarget == null)
                {
                    throw new MissingKeyActorsException("Soulless Horror not found");
                }
                AbstractBuffEvent buffOnDeath = combatData.GetBuffData(Determined895).Where(x => x.To == mainTarget.AgentItem && x is BuffApplyEvent).LastOrDefault();
                if (buffOnDeath != null)
                {
                    fightData.SetSuccess(true, buffOnDeath.Time);
                }
            }
        }

        /*internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            // Necrosis adjust
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 47414);
            return res;
        }*/

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long fightEnd = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.SoullessHorror));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Soulless Horror not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            var howling = mainTarget.GetCastEvents(log, log.FightData.FightStart, fightEnd).Where(x => x.SkillId == HowlingDeath).ToList();
            long start = 0;
            int i = 1;
            foreach (AbstractCastEvent c in howling)
            {
                var phase = new PhaseData(start, Math.Min(c.Time, fightEnd), "Pre-Breakbar " + i++);
                phase.AddTarget(mainTarget);
                start = c.EndTime;
                phases.Add(phase);
            }
            if (fightEnd - start > ParserHelper.PhaseTimeLimit)
            {
                var lastPhase = new PhaseData(start, fightEnd, "Final");
                lastPhase.AddTarget(mainTarget);
                phases.Add(lastPhase);
            }
            return phases;
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.SoullessHorror:
                    // arena reduction
                    var center = new Point3D(-10581, 825, -817);
                    string destroyedRingColor = "rgba(255, 120, 30, 0.3)";
                    List<(double, int, int)> destroyedRings;
                    if (log.FightData.IsCM)
                    {
                        destroyedRings = new List<(double, int, int)>()
                            {
                                (100, 1330, 1550),
                                (90, 1120, 1330),
                                (66, 910, 1120),
                                (33, 720, 910)
                            };
                    }
                    else
                    {
                        destroyedRings = new List<(double, int, int)>()
                            {
                                (90, 1330, 1550),
                                (66, 1120, 1330),
                                (33, 910, 1120),
                            };
                    }
                    foreach ((double hpVal, int innerRadius, int outerRadius) in destroyedRings)
                    {
                        Segment hpUpdate = target.GetHealthUpdates(log).FirstOrDefault(x => x.Value <= hpVal);
                        if (hpUpdate != null)
                        {
                            replay.Decorations.Add(new DoughnutDecoration(true, (int)hpUpdate.Start + 3000, innerRadius, outerRadius, ((int)hpUpdate.Start, (int)log.FightData.FightEnd), destroyedRingColor, new PositionConnector(center)));
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, innerRadius, outerRadius, ((int)hpUpdate.Start, (int)log.FightData.FightEnd), destroyedRingColor, new PositionConnector(center)));
                        }
                        else
                        {
                            break;
                        }
                    }

                    //
                    var howling = cls.Where(x => x.SkillId == HowlingDeath).ToList();
                    foreach (AbstractCastEvent c in howling)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(true, start + c.ExpectedDuration, 180, (start, end), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    var vortex = cls.Where(x => x.SkillId == InnerVortexSlash).ToList();
                    foreach (AbstractCastEvent c in vortex)
                    {
                        start = (int)c.Time;
                        end = start + 4000;
                        ParametricPoint3D next = replay.PolledPositions.FirstOrDefault(x => x.Time >= start);
                        ParametricPoint3D prev = replay.PolledPositions.LastOrDefault(x => x.Time <= start);
                        if (next != null || prev != null)
                        {
                            replay.Decorations.Add(new CircleDecoration(false, 0, 380, (start, end), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, start)));
                            replay.Decorations.Add(new CircleDecoration(true, end, 380, (start, end), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, start)));
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 380, 760, (end, end + 1000), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, start)));
                        }
                    }
                    var deathBloom = cls.Where(x => x.SkillId == DeathBloom).ToList();
                    foreach (AbstractCastEvent c in deathBloom)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 3500, Point3D.GetRotationFromFacing(facing) + (i * 360 / 8), 360 / 12, (start, end), "rgba(255,200,0,0.5)", new AgentConnector(target)));
                        }

                    }
                    var quad1 = cls.Where(x => x.SkillId == QuadSlashFirstSet).ToList();
                    var quad2 = cls.Where(x => x.SkillId == QuadSlashSecondSet).ToList();
                    foreach (AbstractCastEvent c in quad1)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 3500, Point3D.GetRotationFromFacing(facing) + (i * 360 / 4), 360 / 12, (start, end), "rgba(255,200,0,0.5)", new AgentConnector(target)));
                        }

                    }
                    foreach (AbstractCastEvent c in quad2)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 3500, Point3D.GetRotationFromFacing(facing) + 45 + (i * 360 / 4), 360 / 12, (start, end), "rgba(255,200,0,0.5)", new AgentConnector(target)));
                        }

                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Scythe:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 80, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.TormentedDead:
                    if (replay.Positions.Count == 0)
                    {
                        break;
                    }
                    replay.Decorations.Add(new CircleDecoration(true, 0, 400, (end, end + 60000), "rgba(255, 0, 0, 0.5)", new PositionConnector(replay.Positions.Last())));
                    break;
                case (int)ArcDPSEnums.TrashID.SurgingSoul:
                    List<ParametricPoint3D> positions = replay.Positions;
                    if (positions.Count < 2)
                    {
                        break;
                    }
                    if (positions[0].X < -12000 || positions[0].X > -9250)
                    {
                        replay.Decorations.Add(new RectangleDecoration(true, 0, 240, 660, (start, end), "rgba(255,100,0,0.5)", new AgentConnector(target)));
                        break;
                    }
                    else if (positions[0].Y < -525 || positions[0].Y > 2275)
                    {
                        replay.Decorations.Add(new RectangleDecoration(true, 0, 645, 238, (start, end), "rgba(255,100,0,0.5)", new AgentConnector(target)));
                        break;
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.FleshWurm:
                    break;
                default:
                    break;
            }

        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            var necrosis = combatData.GetBuffData(Necrosis).Where(x => x is BuffApplyEvent).ToList();
            if (necrosis.Count == 0)
            {
                return 0;
            }
            // split necrosis
            var splitNecrosis = new Dictionary<AgentItem, List<AbstractBuffEvent>>();
            foreach (AbstractBuffEvent c in necrosis)
            {
                AgentItem tank = c.To;
                if (!splitNecrosis.ContainsKey(tank))
                {
                    splitNecrosis.Add(tank, new List<AbstractBuffEvent>());
                }
                splitNecrosis[tank].Add(c);
            }
            List<AbstractBuffEvent> longestNecrosis = splitNecrosis.Values.First(l => l.Count == splitNecrosis.Values.Max(x => x.Count));
            long minDiff = long.MaxValue;
            for (int i = 0; i < longestNecrosis.Count - 1; i++)
            {
                AbstractBuffEvent cur = longestNecrosis[i];
                AbstractBuffEvent next = longestNecrosis[i + 1];
                long timeDiff = next.Time - cur.Time;
                if (timeDiff > 1000 && minDiff > timeDiff)
                {
                    minDiff = timeDiff;
                }
            }
            return (minDiff < 11000) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }
    }
}
