﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Slothasor : SalvationPass
    {
        public Slothasor(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(TantrumDamage, "Tantrum", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Tantrum","Tantrum (Triple Circles after Ground slamming)", "Tantrum",5000),
            new PlayerDstBuffApplyMechanic(VolatilePoisonBuff, "Volatile Poison", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Poison","Volatile Poison Application (Special Action Key)", "Poison (Action Key)",0),
            new PlayerDstHitMechanic(VolatilePoisonSkill, "Volatile Poison", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Poison dmg","Stood in Volatile Poison", "Poison dmg",0),
            new PlayerDstHitMechanic(Halitosis, "Halitosis", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.LightOrange), "Breath","Halitosis (Flame Breath)", "Flame Breath",0),
            new PlayerDstHitMechanic(SporeRelease, "Spore Release", new MechanicPlotlySetting(Symbols.Pentagon,Colors.Red), "Shake","Spore Release (Coconut Shake)", "Shake",0),
            new PlayerDstBuffApplyMechanic(MagicTransformation, "Magic Transformation", new MechanicPlotlySetting(Symbols.Hexagram,Colors.Teal), "Slub","Magic Transformation (Ate Magic Mushroom)", "Slub Transform",0), 
            //new Mechanic(Nauseated, "Nauseated", ParseEnum.BossIDS.Slothasor, new MechanicPlotlySetting("diamond-tall-open",Colors.LightPurple), "Slub CD",0), //can be skipped imho, identical person and timestamp as Slub Transform
            new PlayerDstBuffApplyMechanic(FixatedSlothasor, "Fixated", new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixate","Fixated by Slothasor", "Fixated",0),
            new PlayerDstHitMechanic(new long[] { ToxicCloud1, ToxicCloud2 }, "Toxic Cloud", new MechanicPlotlySetting(Symbols.PentagonOpen,Colors.DarkGreen), "Floor","Toxic Cloud (stood in green floor poison)", "Toxic Floor",0),
            new PlayerDstBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Fear","Hit by fear after breakbar", "Feared",0).UsingChecker((ba,log) => ba.AppliedDuration == 8000),
            new EnemyDstBuffApplyMechanic(NarcolepsyBuff, "Narcolepsy", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Narcolepsy (Breakbar)", "Breakbar",0),
            new EnemyDstBuffRemoveMechanic(NarcolepsyBuff, "Narcolepsy", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","Narcolepsy (Failed CC)", "CC Fail",0).UsingChecker((br,log) => br.RemovedDuration > 120000),
            new EnemyDstBuffRemoveMechanic(NarcolepsyBuff, "Narcolepsy", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Narcolepsy (Breakbar broken)", "CCed",0).UsingChecker( (br,log) => br.RemovedDuration <= 120000),
            new PlayerDstBuffApplyMechanic(SlipperySlubling, "Slippery Slubling", new MechanicPlotlySetting(Symbols.Star,Colors.Yellow), "Slppr.Slb","Slippery Slubling", "Slippery Slubling",0),
            });
            Extension = "sloth";
            Icon = EncounterIconSlothasor;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplaySlothasor,
                            (654, 1000),
                            (5822, -3491, 9549, 2205)/*,
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210)*/);
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            IReadOnlyList<AbstractBuffEvent> slipperySlublings = log.CombatData.GetBuffData(SlipperySlubling);
            if (slipperySlublings.Any() && log.FightData.Success)
            {
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, SlipperySlubling, log.FightData.FightEnd - ServerDelayConstant))
                    {
                        InstanceBuffs.Add((log.Buffs.BuffsByIds[SlipperySlubling], 1));
                        break;
                    }
                }
            }
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Slubling1,
                ArcDPSEnums.TrashID.Slubling2,
                ArcDPSEnums.TrashID.Slubling3,
                ArcDPSEnums.TrashID.Slubling4,
                ArcDPSEnums.TrashID.PoisonMushroom
            };
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(VolatileAura, VolatileAura), // Volatile Aura
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long fightEnd = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Slothasor));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Slothasor not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            var sleepy = mainTarget.GetCastEvents(log, log.FightData.FightStart, fightEnd).Where(x => x.SkillId == NarcolepsySkill).ToList();
            long start = 0;
            int i = 1;
            foreach (AbstractCastEvent c in sleepy)
            {
                var phase = new PhaseData(start, Math.Min(c.Time, fightEnd), "Phase " + i++);
                phase.AddTarget(mainTarget);
                start = c.EndTime;
                phases.Add(phase);
            }
            var lastPhase = new PhaseData(start, fightEnd, "Phase " + i++);
            lastPhase.AddTarget(mainTarget);
            phases.Add(lastPhase);
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // Mushrooms
            var mushroomAgents = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && (x.HitboxWidth == 146 || x.HitboxWidth == 210) && x.HitboxHeight == 300).ToList();
            if (mushroomAgents.Any())
            {
                int idToKeep = mushroomAgents.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.Count()).MaxBy(x => x.Value).Key;
                foreach (AgentItem mushroom in mushroomAgents)
                {
                    if (!mushroom.IsSpecies(idToKeep))
                    {
                        continue;
                    }
                    var copyEventsFrom = new List<AgentItem>() { mushroom };
                    var hpUpdates = combatData.Where(x => x.SrcMatchesAgent(mushroom) && x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate).ToList();
                    var aliveUpdates = hpUpdates.Where(x => x.DstAgent == 10000).ToList();
                    var deadUpdates = hpUpdates.Where(x => x.DstAgent == 0).ToList();
                    long lastDeadTime = long.MinValue;
                    var posFacingHP = combatData.Where(x => x.SrcMatchesAgent(mushroom) && (x.IsStateChange == ArcDPSEnums.StateChange.Position || x.IsStateChange == ArcDPSEnums.StateChange.Rotation || x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)).ToList();
                    foreach (CombatItem aliveEvent in aliveUpdates)
                    {
                        CombatItem deadEvent = deadUpdates.FirstOrDefault(x => x.Time > lastDeadTime && x.Time > aliveEvent.Time);
                        if (deadEvent == null)
                        {
                            lastDeadTime = Math.Min(fightData.LogEnd, mushroom.LastAware);
                        } 
                        else
                        {
                            lastDeadTime = deadEvent.Time;
                        }
                        AgentItem aliveMushroom = agentData.AddCustomNPCAgent(aliveEvent.Time, lastDeadTime, mushroom.Name, mushroom.Spec, ArcDPSEnums.TrashID.PoisonMushroom, false, mushroom.Toughness, mushroom.Healing, mushroom.Condition, mushroom.Concentration, mushroom.HitboxWidth, mushroom.HitboxHeight);
                        RedirectEventsAndCopyPreviousStates(combatData, extensions, agentData, mushroom, copyEventsFrom, aliveMushroom);
                        copyEventsFrom.Add(aliveMushroom);
                    }
                }
                agentData.Refresh();
            }
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Slothasor:
                    var sleepy = cls.Where(x => x.SkillId == NarcolepsySkill).ToList();
                    foreach (AbstractCastEvent c in sleepy)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    var breath = cls.Where(x => x.SkillId == Halitosis).ToList();
                    foreach (AbstractCastEvent c in breath)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 1000;
                        int duration = 2000;
                        int range = 600;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            float direction = Point3D.GetRotationFromFacing(facing);
                            int angle = 60;
                            replay.Decorations.Add(new PieDecoration(true, 0, range, direction, angle, (start, start + preCastTime), "rgba(255,200,0,0.1)", new AgentConnector(target)));
                            replay.Decorations.Add(new PieDecoration(true, 0, range, direction, angle, (start + preCastTime, start + preCastTime + duration), "rgba(255,200,0,0.4)", new AgentConnector(target)));
                        }
                    }
                    var tantrum = cls.Where(x => x.SkillId == TantrumSkill).ToList();
                    foreach (AbstractCastEvent c in tantrum)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(false, 0, 300, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, end, 300, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                    }
                    var shakes = cls.Where(x => x.SkillId == SporeRelease).ToList();
                    foreach (AbstractCastEvent c in shakes)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(false, 0, 700, (start, end), "rgba(255, 0, 0, 0.4)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, end, 700, (start, end), "rgba(255, 0, 0, 0.4)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.PoisonMushroom:
                    break;
                default:
                    break;
            }

        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // Poison
            List<AbstractBuffEvent> poisonToDrop = GetFilteredList(log.CombatData, VolatilePoisonBuff, p, true, true);
            int toDropStart = 0;
            foreach (AbstractBuffEvent c in poisonToDrop)
            {
                if (c is BuffApplyEvent)
                {
                    toDropStart = (int)c.Time;
                }
                else
                {
                    int toDropEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(false, 0, 180, (toDropStart, toDropEnd), "rgba(255, 255, 100, 0.5)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, toDropStart + 8000, 180, (toDropStart, toDropEnd), "rgba(255, 255, 100, 0.5)", new AgentConnector(p)));
                    ParametricPoint3D poisonNextPos = replay.PolledPositions.FirstOrDefault(x => x.Time >= toDropEnd);
                    ParametricPoint3D poisonPrevPos = replay.PolledPositions.LastOrDefault(x => x.Time <= toDropEnd);
                    if (poisonNextPos != null || poisonPrevPos != null)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, toDropStart + 90000, 900, (toDropEnd, toDropEnd + 90000), "rgba(255, 0, 0, 0.3)", new InterpolatedPositionConnector(poisonPrevPos, poisonNextPos, toDropEnd), 180));
                    }
                }
            }
            // Transformation
            List<AbstractBuffEvent> slubTrans = GetFilteredList(log.CombatData, MagicTransformation, p, true, true);
            int transfoStart = 0;
            foreach (AbstractBuffEvent c in slubTrans)
            {
                if (c is BuffApplyEvent)
                {
                    transfoStart = (int)c.Time;
                }
                else
                {
                    int transfoEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 180, (transfoStart, transfoEnd), "rgba(0, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
            // fixated
            List<AbstractBuffEvent> fixatedSloth = GetFilteredList(log.CombatData, FixatedSlothasor, p, true, true);
            int fixatedSlothStart = 0;
            foreach (AbstractBuffEvent c in fixatedSloth)
            {
                if (c is BuffApplyEvent)
                {
                    fixatedSlothStart = (int)c.Time;
                }
                else
                {
                    int fixatedSlothEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 120, (fixatedSlothStart, fixatedSlothEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
        }
    }
}
