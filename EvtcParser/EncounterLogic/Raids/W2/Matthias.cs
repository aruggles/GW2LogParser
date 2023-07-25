﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Matthias : SalvationPass
    {
        public Matthias(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new PlayerDstHitMechanic(new long[]{ OppressiveGazeHuman, OppressiveGazeAbomination }, "Oppressive Gaze", new MechanicPlotlySetting(Symbols.Hexagram,Colors.Red), "Hadouken","Oppressive Gaze (Hadouken projectile)", "Hadouken",0),
            new PlayerDstHitMechanic(new long[]{ BloodShardsHuman, BloodShardsAbomination }, "Blood Shards", new MechanicPlotlySetting(Symbols.DiamondWideOpen,Colors.Magenta), "Shoot Shards","Blood Shard projectiles during bubble", "Rapid Fire",0),
            new PlayerSrcHitMechanic(new long[]{ BloodShardsHuman, BloodShardsAbomination }, "Blood Shards", new MechanicPlotlySetting(Symbols.DiamondWideOpen,Colors.Green), "Refl.Shards","Blood Shard projectiles reflected during bubble", "Reflected Rapid Fire",0),
            new PlayerDstHitMechanic(new long[]{ ShardsOfRageHuman, ShardsOfRageAbomination }, "Shards of Rage", new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Red), "Jump Shards","Shards of Rage (Jump)", "Jump Shards",1000),
            new PlayerSrcHitMechanic(new long[]{ ShardsOfRageHuman, ShardsOfRageAbomination }, "Shards of Rage", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Red), "Refl.Jump Shards","Rflected Shards of Rage (Jump)", "Reflected Jump Shards",1000),
            new PlayerDstHitMechanic(FieryVortex, "Fiery Vortex", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Tornado","Fiery Vortex (Tornado)", "Tornado",250),
            new PlayerDstHitMechanic(Thunder, "Thunder", new MechanicPlotlySetting(Symbols.TriangleUpOpen,Colors.Teal), "Storm","Thunder Storm hit (air phase)", "Storm cloud",0),
            new PlayerDstBuffApplyMechanic(UnstableBloodMagic, "Unstable Blood Magic", new MechanicPlotlySetting(Symbols.Diamond,Colors.Red), "Well","Unstable Blood Magic application", "Well",0),
            new PlayerDstHitMechanic(WellOfTheProfane, "Well of the Profane", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Red), "Well dmg","Unstable Blood Magic AoE hit", "Stood in Well",0),
            new PlayerDstBuffApplyMechanic(Corruption1, "Corruption", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Corruption","Corruption Application", "Corruption",0),
            new PlayerDstHitMechanic(Corruption2, "Corruption", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Corr. dmg","Hit by Corruption AoE", "Corruption dmg",0),
            new PlayerDstBuffApplyMechanic(MatthiasSacrifice, "Sacrifice", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "Sacrifice","Sacrifice (Breakbar)", "Sacrifice",0),
            new PlayerDstBuffRemoveMechanic(MatthiasSacrifice, "Sacrifice", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CC.End","Sacrifice (Breakbar) ended", "Sacrifice End",0).UsingChecker((br,log) => br.RemovedDuration > 25 && !log.CombatData.GetDeadEvents(br.To).Any(x => Math.Abs(br.Time - x.Time) < ParserHelper.ServerDelayConstant)),
            new PlayerDstBuffRemoveMechanic(MatthiasSacrifice, "Sacrificed", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC.Fail","Sacrifice time ran out", "Sacrificed",0).UsingChecker( (br,log) => br.RemovedDuration <= 25 || log.CombatData.GetDeadEvents(br.To).Any(x => Math.Abs(br.Time - x.Time) < ParserHelper.ServerDelayConstant)),
            new PlayerDstBuffRemoveMechanic(Unbalanced, "Unbalanced", new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "KD","Unbalanced (triggered Storm phase Debuff)", "Knockdown",0).UsingChecker( (br,log) => br.RemovedDuration > 0 && !br.To.HasBuff(log, Stability, br.Time - ParserHelper.ServerDelayConstant)),
            //new Mechanic(Unbalanced, "Unbalanced", Mechanic.MechType.PlayerOnPlayer, ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting(Symbols.Square,"rgb(0,140,0)"), "KD","Unbalanced (triggered Storm phase Debuff) only on successful interrupt", "Knockdown (interrupt)",0,(condition => condition.getCombatItem().Result == ParseEnum.Result.Interrupt)),
            //new Mechanic(Unbalanced, "Unbalanced", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting(Symbols.Square,"rgb(0,140,0)"), "KD","Unbalanced (triggered Storm phase Debuff) only on successful interrupt", "Knockdown (interrupt)",0,(condition => condition.getDLog().GetResult() == ParseEnum.Result.Interrupt)),
            //new Mechanic(BloodFueled, "Blood Fueled", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting(Symbols.Square,Color.Red), "Ate Reflects(good)",0),//human //Applied at the same time as Backflip Shards since it is the buff applied by them, can be omitted imho
            //new Mechanic(BloodFueledAbo, "Blood Fueled", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting(Symbols.Square,Color.Red), "Ate Reflects(good)",0),//abom
            new EnemyDstBuffApplyMechanic(new long[]{ BloodShield, BloodShieldAbo }, "Blood Shield", new MechanicPlotlySetting(Symbols.Octagon,Colors.Red), "Bubble","Blood Shield (protective bubble)", "Bubble",100).UsingChecker((ba, log) => !ba.To.HasBuff(log, BloodShield, ba.Time - 100) && !ba.To.HasBuff(log, BloodShieldAbo, ba.Time - 100)),
            new EnemyDstBuffRemoveMechanic(new long[]{ BloodShield, BloodShieldAbo }, "Lost Blood Shield", new MechanicPlotlySetting(Symbols.Octagon,Colors.Green), "Lost Bubble","Lost Blood Shield (protective bubble)", "Lost Bubble",100),
            new PlayerDstBuffApplyMechanic(ZealousBenediction, "Zealous Benediction", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Bombs","Zealous Benediction (Expanding bombs)","Bomb",0),
            new PlayerDstBuffApplyMechanic(Slow, "Icy Patch", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Blue), "Icy KD","Knockdown by Icy Patch", "Icy Patch KD",0).UsingChecker((br,log) => br.AppliedDuration == 10000 && !br.To.HasBuff(log, Stability, br.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstHitMechanic(Surrender, "Surrender", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Black), "Spirit","Surrender (hit by walking Spirit)", "Spirit hit",0)
            });
            Extension = "matt";
            Icon = EncounterIconMatthias;
            EncounterCategoryInformation.InSubCategoryOrder = 2;
            EncounterID |= 0x000003;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayMatthias,
                            (880, 880),
                            (-7248, 4585, -4625, 7207)/*,
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210)*/);
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            IReadOnlyList<AbstractBuffEvent> bloodstoneBisque = log.CombatData.GetBuffData(BloodstoneBisque);
            if (bloodstoneBisque.Any() && log.FightData.Success)
            {
                int playersWithBisque = 0;
                int expectedPlayersForSuccess = 0;
                long fightEnd = log.FightData.FightEnd - ParserHelper.ServerDelayConstant;
                long fightStart = log.FightData.FightStart + ParserHelper.ServerDelayConstant;
                foreach (Player p in log.PlayerList)
                {
                    IReadOnlyDictionary<long, BuffsGraphModel> graphs = p.GetBuffGraphs(log);
                    if (graphs.TryGetValue(BloodstoneBisque, out BuffsGraphModel graph))
                    {
                        if (!graph.BuffChart.Any(x => x.Value == 0 && x.IntersectSegment(fightStart, fightEnd)))
                        {
                            playersWithBisque++;
                        }
                    }
                    (_, _, IReadOnlyList<Segment> dcs) = p.GetStatus(log);
                    if (!dcs.Any(x => x.ContainsPoint(fightEnd)))
                    {
                        expectedPlayersForSuccess++;
                    }
                }
                if (expectedPlayersForSuccess <= playersWithBisque)
                {
                    InstanceBuffs.Add((log.Buffs.BuffsByIds[BloodstoneBisque], 1));
                }
            }
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(SpontaneousCombustion, SpontaneousCombustion),
                new DamageCastFinder(SnowstormSkill, SnowstormSkill),
                new DamageCastFinder(DownpourSkill, DownpourSkill),
            };
        }
        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long fightEnd = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Matthias));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Matthias not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Special buff cast check
            AbstractBuffEvent heatWave = log.CombatData.GetBuffData(HeatWaveMatthias).FirstOrDefault();
            if (heatWave != null)
            {
                phases.Add(new PhaseData(0, heatWave.Time));
                AbstractBuffEvent downPour = log.CombatData.GetBuffData(DownpourMatthias).FirstOrDefault();
                if (downPour != null)
                {
                    phases.Add(new PhaseData(heatWave.Time, downPour.Time));
                    AbstractBuffEvent abo = log.CombatData.GetBuffData(Unstable).FirstOrDefault();
                    if (abo != null)
                    {
                        phases.Add(new PhaseData(downPour.Time, abo.Time));
                        AbstractBuffEvent invulRemove = log.CombatData.GetBuffData(mainTarget.AgentItem).FirstOrDefault(x => x.Time >= abo.Time && x.Time <= abo.Time + 10000 && x.BuffID == Invulnerability757 && !(x is BuffApplyEvent));
                        if (invulRemove != null)
                        {
                            phases.Add(new PhaseData(invulRemove.Time, fightEnd));
                        }
                    }
                    else
                    {
                        phases.Add(new PhaseData(downPour.Time, fightEnd));
                    }
                }
                else
                {
                    phases.Add(new PhaseData(heatWave.Time, fightEnd));
                }
            }
            else
            {
                phases.Add(new PhaseData(log.FightData.FightStart, fightEnd));
            }
            string[] namesMat = new[] { "Ice Phase", "Fire Phase", "Storm Phase", "Abomination Phase" };
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = namesMat[i - 1];
                phases[i].DrawStart = i > 1;
                phases[i].AddTarget(mainTarget);
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // has breakbar state into
            if (combatData.Any(x => x.IsStateChange == ArcDPSEnums.StateChange.BreakbarState))
            {
                var sacrificeList = combatData.Where(x => x.SkillID == MatthiasSacrifice && !x.IsExtension && (x.IsBuffRemove == ArcDPSEnums.BuffRemove.All || x.IsBuffApply())).ToList();
                var sacrificeStartList = sacrificeList.Where(x => x.IsBuffRemove == ArcDPSEnums.BuffRemove.None).ToList();
                var sacrificeEndList = sacrificeList.Where(x => x.IsBuffRemove == ArcDPSEnums.BuffRemove.All).ToList();
                var copies = new List<CombatItem>();
                for (int i = 0; i < sacrificeStartList.Count; i++)
                {
                    //
                    long sacrificeStartTime = sacrificeStartList[i].Time;
                    long sacrificeEndTime = i < sacrificeEndList.Count ? sacrificeEndList[i].Time : fightData.FightEnd;
                    //
                    AgentItem sacrifice = agentData.GetAgentByType(AgentItem.AgentType.Player).FirstOrDefault(x => x == agentData.GetAgent(sacrificeStartList[i].DstAgent, sacrificeStartList[i].Time));
                    if (sacrifice == null)
                    {
                        continue;
                    }
                    AgentItem sacrificeCrystal = agentData.AddCustomNPCAgent(sacrificeStartTime, sacrificeEndTime + 100, "Sacrificed " + (i + 1) + " " + sacrifice.Name.Split('\0')[0], sacrifice.Spec, ArcDPSEnums.TrashID.MatthiasSacrificeCrystal, false);
                    foreach (CombatItem cbt in combatData)
                    {
                        if (!sacrificeCrystal.InAwareTimes(cbt.Time))
                        {
                            continue;
                        }
                        bool skip = !(cbt.DstMatchesAgent(sacrifice, extensions) || cbt.SrcMatchesAgent(sacrifice, extensions));
                        if (skip)
                        {
                            continue;
                        }
                        // redirect damage events
                        if (cbt.IsDamage(extensions))
                        {
                            // only redirect incoming damage
                            if (cbt.DstMatchesAgent(sacrifice, extensions))
                            {
                                cbt.OverrideDstAgent(sacrificeCrystal.Agent);
                            }
                        }
                        // copy the rest
                        else
                        {
                            var copy = new CombatItem(cbt);
                            if (copy.DstMatchesAgent(sacrifice, extensions))
                            {
                                copy.OverrideDstAgent(sacrificeCrystal.Agent);
                            }
                            if (copy.SrcMatchesAgent(sacrifice, extensions))
                            {
                                copy.OverrideSrcAgent(sacrificeCrystal.Agent);
                            }
                            copies.Add(copy);
                        }
                    }
                }
                if (copies.Any())
                {
                    combatData.AddRange(copies);
                }
            }
            ComputeFightTargets(agentData, combatData, extensions);
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.IsSpecies(ArcDPSEnums.TrashID.MatthiasSacrificeCrystal))
                {
                    target.SetManualHealth(100000);
                }
            }
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Matthias,
                (int)ArcDPSEnums.TrashID.MatthiasSacrificeCrystal
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Storm,
                ArcDPSEnums.TrashID.Spirit,
                ArcDPSEnums.TrashID.Spirit2,
                ArcDPSEnums.TrashID.IcePatch,
                ArcDPSEnums.TrashID.Tornado
            };
        }

        private static void AddMatthiasBubbles(long buffID, NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            List<AbstractBuffEvent> shields = GetFilteredList(log.CombatData, buffID, target, true, true);
            int start = 0;
            for (int i = 0; i < shields.Count; i++)
            {
                AbstractBuffEvent buffEvent = shields[i];
                if (buffEvent is BuffApplyEvent)
                {
                    start = (int)buffEvent.Time;
                    if (i == shields.Count - 1)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 250, ((int)start, (int)log.FightData.FightEnd), "rgba(255, 0, 255, 0.5)", new AgentConnector(target)));
                    }
                } 
                else
                {
                    replay.Decorations.Add(new CircleDecoration(true, 0, 250, ((int)start, (int)buffEvent.Time), "rgba(255, 0, 255, 0.5)", new AgentConnector(target)));
                }
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Matthias:
                    AddMatthiasBubbles(BloodShield, target, log, replay);
                    AddMatthiasBubbles(BloodShieldAbo, target, log, replay);
                    var rageShards = cls.Where(x => x.SkillId == ShardsOfRageHuman || x.SkillId == ShardsOfRageAbomination).ToList();
                    foreach (AbstractCastEvent c in rageShards)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(false, 0, 300, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, end, 300, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    }
                    var hadouken = cls.Where(x => x.SkillId == OppressiveGazeAbomination || x.SkillId == OppressiveGazeHuman).ToList();
                    foreach (AbstractCastEvent c in hadouken)
                    {
                        start = (int)c.Time;
                        int preCastTime = 1000;
                        int duration = 750;
                        int width = 4000; int height = 130;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            float direction = ParserHelper.RadianToDegreeF(Math.Atan2(facing.Y, facing.X));
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, direction, width / 2, (start, start + preCastTime), "rgba(255, 0, 0, 0.1)", new AgentConnector(target)));
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, direction, width / 2, (start + preCastTime, start + preCastTime + duration), "rgba(255, 0, 0, 0.7)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Storm:
                    replay.Decorations.Add(new CircleDecoration(false, 0, 260, (start, end), "rgba(0, 80, 255, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Spirit:
                case (int)ArcDPSEnums.TrashID.Spirit2:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.IcePatch:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, end), "rgba(0, 0, 255, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Tornado:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 90, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                default:
                    break;
            }

        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // Corruption
            List<AbstractBuffEvent> corruptedMatthias = GetFilteredList(log.CombatData, Corruption1, p, true, true);
            corruptedMatthias.AddRange(GetFilteredList(log.CombatData, Corruption2, p, true, true));
            int corruptedMatthiasStart = 0;
            foreach (AbstractBuffEvent c in corruptedMatthias)
            {
                if (c is BuffApplyEvent)
                {
                    corruptedMatthiasStart = (int)c.Time;
                }
                else
                {
                    int corruptedMatthiasEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 180, (corruptedMatthiasStart, corruptedMatthiasEnd), "rgba(255, 150, 0, 0.5)", new AgentConnector(p)));
                    ParametricPoint3D wellNextPosition = replay.PolledPositions.FirstOrDefault(x => x.Time >= corruptedMatthiasEnd);
                    ParametricPoint3D wellPrevPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= corruptedMatthiasEnd);
                    if (wellNextPosition != null || wellPrevPosition != null)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, (corruptedMatthiasEnd, corruptedMatthiasEnd + 100000), "rgba(0, 0, 0, 0.3)", new InterpolatedPositionConnector(wellPrevPosition, wellNextPosition, corruptedMatthiasEnd)));
                        replay.Decorations.Add(new CircleDecoration(true, corruptedMatthiasEnd + 100000, 180, (corruptedMatthiasEnd, corruptedMatthiasEnd + 100000), "rgba(0, 0, 0, 0.3)", new InterpolatedPositionConnector(wellPrevPosition, wellNextPosition, corruptedMatthiasEnd)));
                    }
                }
            }
            // Well of profane
            List<AbstractBuffEvent> wellMatthias = GetFilteredList(log.CombatData, UnstableBloodMagic, p, true, true);
            int wellMatthiasStart = 0;
            foreach (AbstractBuffEvent c in wellMatthias)
            {
                if (c is BuffApplyEvent)
                {
                    wellMatthiasStart = (int)c.Time;
                }
                else
                {
                    int wellMatthiasEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(false, 0, 120, (wellMatthiasStart, wellMatthiasEnd), "rgba(150, 255, 80, 0.5)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, wellMatthiasStart + 9000, 120, (wellMatthiasStart, wellMatthiasEnd), "rgba(150, 255, 80, 0.5)", new AgentConnector(p)));
                    ParametricPoint3D wellNextPosition = replay.PolledPositions.FirstOrDefault(x => x.Time >= wellMatthiasEnd);
                    ParametricPoint3D wellPrevPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= wellMatthiasEnd);
                    if (wellNextPosition != null || wellPrevPosition != null)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 300, (wellMatthiasEnd, wellMatthiasEnd + 90000), "rgba(255, 0, 50, 0.5)", new InterpolatedPositionConnector(wellPrevPosition, wellNextPosition, wellMatthiasEnd)));
                    }
                }
            }
            // Sacrifice
            List<AbstractBuffEvent> sacrificeMatthias = GetFilteredList(log.CombatData, MatthiasSacrifice, p, true, true);
            int sacrificeMatthiasStart = 0;
            foreach (AbstractBuffEvent c in sacrificeMatthias)
            {
                if (c is BuffApplyEvent)
                {
                    sacrificeMatthiasStart = (int)c.Time;
                }
                else
                {
                    int sacrificeMatthiasEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 120, (sacrificeMatthiasStart, sacrificeMatthiasEnd), "rgba(0, 150, 250, 0.2)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, sacrificeMatthiasStart + 10000, 120, (sacrificeMatthiasStart, sacrificeMatthiasEnd), "rgba(0, 150, 250, 0.35)", new AgentConnector(p)));
                }
            }
            // Bombs
            var zealousBenediction = log.CombatData.GetBuffData(ZealousBenediction).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent c in zealousBenediction)
            {
                int zealousStart = (int)c.Time;
                int zealousEnd = zealousStart + 5000;
                replay.Decorations.Add(new CircleDecoration(true, 0, 180, (zealousStart, zealousEnd), "rgba(200, 150, 0, 0.2)", new AgentConnector(p)));
                replay.Decorations.Add(new CircleDecoration(true, zealousEnd, 180, (zealousStart, zealousEnd), "rgba(200, 150, 0, 0.4)", new AgentConnector(p)));
            }
        }

    }
}
