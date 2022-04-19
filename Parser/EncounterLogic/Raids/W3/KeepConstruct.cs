﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class KeepConstruct : StrongholdOfTheFaithful
    {
        public KeepConstruct(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(34912, "Fixate", new MechanicPlotlySetting("star",Colors.Magenta), "Fixate","Fixated by Statue", "Fixated",0),
            new PlayerBuffApplyMechanic(34925, "Fixate", new MechanicPlotlySetting("star",Colors.Magenta), "Fixate","Fixated by Statue", "Fixated",0),
            new HitOnPlayerMechanic(35077, "Hail of Fury", new MechanicPlotlySetting("circle-open",Colors.Red), "Debris","Hail of Fury (Falling Debris)", "Debris",0),
            new EnemyBuffApplyMechanic(35096, "Compromised", new MechanicPlotlySetting("hexagon",Colors.Blue), "Rift#","Compromised (Pushed Orb through Rifts)", "Compromised",0),
            new EnemyBuffApplyMechanic(35119, "Magic Blast", new MechanicPlotlySetting("star",Colors.Teal), "M.B.# 33%","Magic Blast (Orbs eaten by KC) at 33%", "Magic Blast 33%",0, (de, log) => {
                var phases = log.FightData.GetPhases(log).Where(x => x.Name.Contains("%")).ToList();
                if (phases.Count < 2)
                {
                    // no 33% magic blast
                    return false;
                }
                return de.Time >= phases[1].End;
            }),
            new EnemyBuffApplyMechanic(35119, "Magic Blast", new MechanicPlotlySetting("star",Colors.Teal), "M.B.# 66%","Magic Blast (Orbs eaten by KC) at 66%", "Magic Blast 66%",0, (de, log) => {
                var phases = log.FightData.GetPhases(log).Where(x => x.Name.Contains("%")).ToList();
                if (phases.Count < 1)
                {
                    // no 66% magic blast
                    return false;
                }
                bool condition = de.Time >= phases[0].End;
                if (phases.Count > 1)
                {
                    // must be before 66%-33% phase if it exists
                    condition = condition && de.Time <= phases[1].Start;
                }
                return condition;
            }),
            new SpawnMechanic(16227, "Insidious Projection", new MechanicPlotlySetting("bowtie",Colors.Red), "Merge","Insidious Projection spawn (2 Statue merge)", "Merged Statues",0),
            new HitOnPlayerMechanic(35137, "Phantasmal Blades", new MechanicPlotlySetting("hexagram-open",Colors.Magenta), "Pizza","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new HitOnPlayerMechanic(34971, "Phantasmal Blades", new MechanicPlotlySetting("hexagram-open",Colors.Magenta), "Pizza","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new HitOnPlayerMechanic(35064, "Phantasmal Blades", new MechanicPlotlySetting("hexagram-open",Colors.Magenta), "Pizza","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new HitOnPlayerMechanic(35086, "Tower Drop", new MechanicPlotlySetting("circle",Colors.LightOrange), "Jump","Tower Drop (KC Jump)", "Tower Drop",0),
            new PlayerBuffApplyMechanic(35103, "Xera's Fury", new MechanicPlotlySetting("circle",Colors.Orange), "Bomb","Xera's Fury (Large Bombs) application", "Bombs",0),
            new HitOnPlayerMechanic(34914, "Good White Orb", new MechanicPlotlySetting("circle",Colors.White), "GW.Orb","Good White Orb", "Good White Orb",0, (de,log) => de.To.HasBuff(log, 35109, de.Time)),
            new HitOnPlayerMechanic(34972, "Good Red Orb", new MechanicPlotlySetting("circle",Colors.DarkRed), "GR.Orb","Good Red Orb", "Good Red Orb",0, (de,log) => de.To.HasBuff(log, 35091, de.Time)),
            new HitOnPlayerMechanic(34914, "Bad White Orb", new MechanicPlotlySetting("circle",Colors.Grey), "BW.Orb","Bad White Orb", "Bad White Orb",0, (de,log) => !de.To.HasBuff(log, 35109, de.Time)),
            new HitOnPlayerMechanic(34972, "Bad Red Orb", new MechanicPlotlySetting("circle",Colors.Red), "BR.Orb","Bad Red Orb", "Bad Red Orb",0, (de,log) => !de.To.HasBuff(log, 35091, de.Time)),
            new HitOnEnemyMechanic(16261, "Core Hit", new MechanicPlotlySetting("star-open",Colors.LightOrange), "Core Hit","Core was Hit by Player", "Core Hit",1000)
            });
            Extension = "kc";
            Icon = "https://wiki.guildwars2.com/images/e/ea/Mini_Keep_Construct.png";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/dEwDsOJ.png",
                            (987, 1000),
                            (-5467, 8069, -2282, 11297)/*,
                            (-12288, -27648, 12288, 27648),
                            (1920, 12160, 2944, 14464)*/);
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(34946, 34946, InstantCastFinder.DefaultICD), // Construct Aura
            };
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.KeepConstruct);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Keep Construct not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Main phases 35025
            List<AbstractBuffEvent> kcPhaseInvuls = GetFilteredList(log.CombatData, 35025, mainTarget, true);
            foreach (AbstractBuffEvent c in kcPhaseInvuls)
            {
                if (c is BuffApplyEvent)
                {
                    end = c.Time;
                    phases.Add(new PhaseData(start, end));
                }
                else
                {
                    start = c.Time;
                }
            }
            if (fightDuration - start > ParserHelper.PhaseTimeLimit && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
                start = fightDuration;
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].AddTarget(mainTarget);
            }
            // add burn phases
            int offset = phases.Count;
            var orbItems = log.CombatData.GetBuffData(35096).Where(x => x.To == mainTarget.AgentItem).ToList();
            // Get number of orbs and filter the list
            start = 0;
            int orbCount = 0;
            var segments = new List<Segment>();
            foreach (AbstractBuffEvent c in orbItems)
            {
                if (c is BuffApplyEvent)
                {
                    if (start == 0)
                    {
                        start = c.Time;
                    }
                    orbCount++;
                }
                else if (start != 0)
                {
                    segments.Add(new Segment(start, Math.Min(c.Time, fightDuration), orbCount));
                    orbCount = 0;
                    start = 0;
                }
            }
            int burnCount = 1;
            foreach (Segment seg in segments)
            {
                var phase = new PhaseData(seg.Start, seg.End, "Burn " + burnCount++ + " (" + seg.Value + " orbs)");
                phase.AddTarget(mainTarget);
                phases.Add(phase);
            }
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            // pre burn phases
            int preBurnCount = 1;
            var preBurnPhase = new List<PhaseData>();
            List<AbstractBuffEvent> kcInvuls = GetFilteredList(log.CombatData, 762, mainTarget, true);
            foreach (AbstractBuffEvent invul in kcInvuls)
            {
                if (invul is BuffApplyEvent)
                {
                    end = invul.Time;
                    PhaseData prevPhase = phases.LastOrDefault(x => x.Start <= end || x.End <= end);
                    if (prevPhase != null)
                    {
                        start = (prevPhase.End >= end ? prevPhase.Start : prevPhase.End) + 1;
                        if (end - start > 1000)
                        {
                            var phase = new PhaseData(start, end, "Pre-Burn " + preBurnCount++);
                            phase.AddTarget(mainTarget);
                            preBurnPhase.Add(phase);
                        }
                    }
                }
            }
            phases.AddRange(preBurnPhase);
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            // add leftover phases
            PhaseData cur = null;
            int leftOverCount = 1;
            var leftOverPhases = new List<PhaseData>();
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (phase.Name.Contains("%"))
                {
                    cur = phase;
                }
                else if (phase.Name.Contains("orbs"))
                {
                    if (cur != null)
                    {
                        if (cur.End >= phase.End + 5000 && (i == phases.Count - 1 || phases[i + 1].Name.Contains("%")))
                        {
                            var leftOverPhase = new PhaseData(phase.End + 1, cur.End, "Leftover " + leftOverCount++);
                            leftOverPhase.AddTarget(mainTarget);
                            leftOverPhases.Add(leftOverPhase);
                        }
                    }
                }
            }
            phases.AddRange(leftOverPhases);
            return phases;
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.KeepConstruct,
                (int)ArcDPSEnums.TrashID.Jessica,
                (int)ArcDPSEnums.TrashID.Olson,
                (int)ArcDPSEnums.TrashID.Engul,
                (int)ArcDPSEnums.TrashID.Faerla,
                (int)ArcDPSEnums.TrashID.Caulle,
                (int)ArcDPSEnums.TrashID.Henley,
                (int)ArcDPSEnums.TrashID.Galletta,
                (int)ArcDPSEnums.TrashID.Ianim,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Core,
                ArcDPSEnums.TrashID.GreenPhantasm,
                ArcDPSEnums.TrashID.InsidiousProjection,
                ArcDPSEnums.TrashID.UnstableLeyRift,
                ArcDPSEnums.TrashID.RadiantPhantasm,
                ArcDPSEnums.TrashID.CrimsonPhantasm,
                ArcDPSEnums.TrashID.RetrieverProjection
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, friendlies, extensions);
            var countDict = new Dictionary<int, int>();
            var bigPhantasmIDs = new HashSet<int>
            {
                (int)ArcDPSEnums.TrashID.Jessica,
                (int)ArcDPSEnums.TrashID.Olson,
                (int)ArcDPSEnums.TrashID.Engul,
                (int)ArcDPSEnums.TrashID.Faerla,
                (int)ArcDPSEnums.TrashID.Caulle,
                (int)ArcDPSEnums.TrashID.Henley,
                (int)ArcDPSEnums.TrashID.Galletta,
                (int)ArcDPSEnums.TrashID.Ianim,
            };
            foreach (AbstractSingleActor target in Targets)
            {
                if (bigPhantasmIDs.Contains(target.ID))
                {
                    if (countDict.TryGetValue(target.ID, out int count))
                    {
                        target.OverrideName(target.Character + " " + (++count));
                    }
                    else
                    {
                        count = 1;
                        target.OverrideName(target.Character + " " + count);
                    }
                    countDict[target.ID] = count;
                }
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.KeepConstruct:

                    List<AbstractBuffEvent> kcOrbCollect = GetFilteredList(log.CombatData, 35025, target, true);
                    int kcOrbStart = 0, kcOrbEnd = 0;
                    foreach (AbstractBuffEvent c in kcOrbCollect)
                    {
                        if (c is BuffApplyEvent)
                        {
                            kcOrbStart = (int)c.Time;
                        }
                        else
                        {
                            kcOrbEnd = (int)c.Time;
                            replay.Decorations.Add(new CircleDecoration(false, 0, 300, (kcOrbStart, kcOrbEnd), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                            replay.Decorations.Add(new CircleDecoration(true, kcOrbEnd, 300, (kcOrbStart, kcOrbEnd), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        }
                    }
                    var towerDrop = cls.Where(x => x.SkillId == 35086).ToList();
                    foreach (AbstractCastEvent c in towerDrop)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        int skillCast = end - 1000;
                        Point3D next = replay.PolledPositions.FirstOrDefault(x => x.Time >= end);
                        Point3D prev = replay.PolledPositions.LastOrDefault(x => x.Time <= end);
                        if (prev != null || next != null)
                        {
                            replay.Decorations.Add(new CircleDecoration(false, 0, 400, (start, skillCast), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, end)));
                            replay.Decorations.Add(new CircleDecoration(true, skillCast, 400, (start, skillCast), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, end)));
                        }
                    }
                    var blades1 = cls.Where(x => x.SkillId == 35064).ToList();
                    var blades2 = cls.Where(x => x.SkillId == 35137).ToList();
                    var blades3 = cls.Where(x => x.SkillId == 34971).ToList();
                    int bladeDelay = 150;
                    int duration = 1000;
                    foreach (AbstractCastEvent c in blades1)
                    {
                        int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                        start = (int)c.Time + bladeDelay;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)", new AgentConnector(target)));
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, facing, 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts longer
                        }
                    }
                    foreach (AbstractCastEvent c in blades2)
                    {
                        int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                        start = (int)c.Time + bladeDelay;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)", new AgentConnector(target)));
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, facing, 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI), 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts longer
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts longer
                        }
                    }
                    foreach (AbstractCastEvent c in blades3)
                    {
                        int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                        start = (int)c.Time + bladeDelay;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)", new AgentConnector(target)));
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI), 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + 120), 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI - 120), 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts longer
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8 + 120), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts longer
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8 - 120), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,0,255,0.5)", new AgentConnector(target))); // First blade lasts longer
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Core:
                    break;
                case (int)ArcDPSEnums.TrashID.Jessica:
                case (int)ArcDPSEnums.TrashID.Olson:
                case (int)ArcDPSEnums.TrashID.Engul:
                case (int)ArcDPSEnums.TrashID.Faerla:
                case (int)ArcDPSEnums.TrashID.Caulle:
                case (int)ArcDPSEnums.TrashID.Henley:
                case (int)ArcDPSEnums.TrashID.Galletta:
                case (int)ArcDPSEnums.TrashID.Ianim:
                    replay.Decorations.Add(new CircleDecoration(false, 0, 600, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    replay.Decorations.Add(new CircleDecoration(true, 0, 400, (start, end), "rgba(0, 125, 255, 0.5)", new AgentConnector(target)));
                    Point3D firstPhantasmPosition = replay.PolledPositions.FirstOrDefault();
                    if (firstPhantasmPosition != null)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 300, (start - 5000, start), "rgba(220, 50, 0, 0.5)", new PositionConnector(firstPhantasmPosition)));
                        replay.Decorations.Add(new CircleDecoration(true, start, 300, (start - 5000, start), "rgba(220, 50, 0, 0.5)", new PositionConnector(firstPhantasmPosition)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.GreenPhantasm:
                    int lifetime = 8000;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 210, (start, start + lifetime), "rgba(0,255,0,0.2)", new AgentConnector(target)));
                    replay.Decorations.Add(new CircleDecoration(true, start + lifetime, 210, (start, start + lifetime), "rgba(0,255,0,0.3)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.RetrieverProjection:
                case (int)ArcDPSEnums.TrashID.InsidiousProjection:
                case (int)ArcDPSEnums.TrashID.UnstableLeyRift:
                case (int)ArcDPSEnums.TrashID.RadiantPhantasm:
                case (int)ArcDPSEnums.TrashID.CrimsonPhantasm:
                    break;
                default:
                    break;
            }

        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return combatData.GetSkills().Contains(34958) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedLog log, CombatReplay replay)
        {
            // Bombs
            List<AbstractBuffEvent> xeraFury = GetFilteredList(log.CombatData, 35103, p, true);
            int xeraFuryStart = 0;
            foreach (AbstractBuffEvent c in xeraFury)
            {
                if (c is BuffApplyEvent)
                {
                    xeraFuryStart = (int)c.Time;
                }
                else
                {
                    int xeraFuryEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 550, (xeraFuryStart, xeraFuryEnd), "rgba(200, 150, 0, 0.2)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, xeraFuryEnd, 550, (xeraFuryStart, xeraFuryEnd), "rgba(200, 150, 0, 0.4)", new AgentConnector(p)));
                }

            }
            //fixated Statue
            var fixatedStatue = GetFilteredList(log.CombatData, 34912, p, true).Concat(GetFilteredList(log.CombatData, 34925, p, true)).ToList();
            int fixationStatueStart = 0;
            NPC statue = null;
            foreach (AbstractBuffEvent c in fixatedStatue)
            {
                if (c is BuffApplyEvent)
                {
                    fixationStatueStart = (int)c.Time;
                    statue = TrashMobs.FirstOrDefault(x => x.AgentItem == c.CreditedBy);
                }
                else
                {
                    int fixationStatueEnd = (int)c.Time;
                    if (statue != null)
                    {
                        replay.Decorations.Add(new LineDecoration(0, (fixationStatueStart, fixationStatueEnd), "rgba(255, 0, 255, 0.5)", new AgentConnector(p), new AgentConnector(statue)));
                    }
                }
            }
        }
    }
}
