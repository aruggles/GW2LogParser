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
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class ValeGuardian : SpiritVale
    {
        public ValeGuardian(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(31860, "Unstable Magic Spike", new MechanicPlotlySetting("circle",Colors.Blue), "Split TP","Unstable Magic Spike (Green Guard Teleport)","Green Guard TP",500),
            new HitOnPlayerMechanic(31392, "Unstable Magic Spike", new MechanicPlotlySetting("circle",Colors.Blue), "Boss TP","Unstable Magic Spike (Boss Teleport)", "Boss TP",500),
            new HitOnPlayerMechanic(31340, "Distributed Magic", new MechanicPlotlySetting("circle",Colors.DarkGreen), "Green","Distributed Magic (Stood in Green)", "Green Team",0),
            new EnemyCastStartMechanic(31340, "Distributed Magic", new MechanicPlotlySetting("circle-open",Colors.LightBlue) , "Green Cast B","Distributed Magic (Green Field appeared in Blue Sector)", "Green in Blue",0),
            new HitOnPlayerMechanic(31391, "Distributed Magic", new MechanicPlotlySetting("circle",Colors.DarkGreen), "Green","Distributed Magic (Stood in Green)", "Green Team",0),
            new EnemyCastStartMechanic(31391, "Distributed Magic", new MechanicPlotlySetting("circle-open",Colors.Orange), "Green Cast R","Distributed Magic (Green Field appeared in Red Sector)", "Green in Red",0),
            new HitOnPlayerMechanic(31529, "Distributed Magic", new MechanicPlotlySetting("circle",Colors.DarkGreen), "Green","Distributed Magic (Stood in Green)", "Green Team", 0),
            new HitOnPlayerMechanic(31750, "Distributed Magic", new MechanicPlotlySetting("circle",Colors.DarkGreen), "Green","Distributed Magic (Stood in Green)", "Green Team",0),
            new EnemyCastStartMechanic(31750, "Distributed Magic", new MechanicPlotlySetting("circle-open",Colors.Green), "Green Cast G","Distributed Magic (Green Field appeared in Green Sector)", "Green in Green",0),
            new HitOnPlayerMechanic(31886, "Magic Pulse", new MechanicPlotlySetting("circle-open",Colors.Red), "Seeker","Magic Pulse (Hit by Seeker)", "Seeker",0),
            new PlayerBuffApplyMechanic(31695, "Pylon Attunement: Red", new MechanicPlotlySetting("square",Colors.Red), "Attune R","Pylon Attunement: Red", "Red Attuned",0),
            new PlayerBuffApplyMechanic(31317, "Pylon Attunement: Blue", new MechanicPlotlySetting("square",Colors.Blue), "Attune B","Pylon Attunement: Blue", "Blue Attuned",0),
            new PlayerBuffApplyMechanic(31852, "Pylon Attunement: Green", new MechanicPlotlySetting("square",Colors.DarkGreen), "Attune G","Pylon Attunement: Green", "Green Attuned",0),
            new EnemyBuffRemoveMechanic(31413, "Blue Pylon Power", new MechanicPlotlySetting("square-open",Colors.Blue), "Invuln Strip","Blue Guard Invuln was stripped", "Blue Invuln Strip",0),
            new HitOnPlayerMechanic(31539, "Unstable Pylon", new MechanicPlotlySetting("hexagram-open",Colors.Red), "Floor R","Unstable Pylon (Red Floor dmg)", "Floor dmg",0),
            new HitOnPlayerMechanic(31828, "Unstable Pylon", new MechanicPlotlySetting("hexagram-open",Colors.Blue), "Floor B","Unstable Pylon (Blue Floor dmg)", "Floor dmg",0),
            new HitOnPlayerMechanic(31884, "Unstable Pylon", new MechanicPlotlySetting("hexagram-open",Colors.DarkGreen), "Floor G","Unstable Pylon (Green Floor dmg)", "Floor dmg",0),
            new EnemyCastStartMechanic(31419, "Magic Storm", new MechanicPlotlySetting("diamond-tall",Colors.DarkTeal), "CC","Magic Storm (Breakbar)","Breakbar",0),
            new EnemyCastEndMechanic(31419, "Magic Storm", new MechanicPlotlySetting("diamond-tall",Colors.DarkGreen), "CCed","Magic Storm (Breakbar broken) ", "CCed",0, (c, log) => c.ActualDuration <= 8544),
            new EnemyCastEndMechanic(31419, "Magic Storm", new MechanicPlotlySetting("diamond-tall",Colors.Red), "CC Fail","Magic Storm (Breakbar failed) ", "CC Fail",0,(c, log) => c.ActualDuration > 8544),
            });
            Extension = "vg";
            Icon = "https://wiki.guildwars2.com/images/f/fb/Mini_Vale_Guardian.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/W7MocGz.png",
                            (889, 889),
                            (-6365, -22213, -3150, -18999)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(31557, 31557, InstantCastFinder.DefaultICD), // Magic Aura VG
                new DamageCastFinder(31462, 31462, InstantCastFinder.DefaultICD), // Magic Aura Red
                new DamageCastFinder(31557, 31557, InstantCastFinder.DefaultICD), // Magic Aura Green
                new DamageCastFinder(31375, 31375, InstantCastFinder.DefaultICD), // Magic Aura Blue
            };
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.ValeGuardian,
                (int)ArcDPSEnums.TrashID.RedGuardian,
                (int)ArcDPSEnums.TrashID.BlueGuardian,
                (int)ArcDPSEnums.TrashID.GreenGuardian
            };
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ValeGuardian);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Vale Guardian not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            phases.AddRange(GetPhasesByInvul(log, 757, mainTarget, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Split " + (i) / 2;
                    var ids = new List<int>
                    {
                       (int) ArcDPSEnums.TrashID.BlueGuardian,
                       (int) ArcDPSEnums.TrashID.GreenGuardian,
                       (int) ArcDPSEnums.TrashID.RedGuardian
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mainTarget);
                }
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, friendlies, extensions);
            int curRed = 1;
            int curBlue = 1;
            int curGreen = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.ID == (int)ArcDPSEnums.TrashID.RedGuardian)
                {
                    target.OverrideName(target.Character + " " + curRed++);
                }
                if (target.ID == (int)ArcDPSEnums.TrashID.BlueGuardian)
                {
                    target.OverrideName(target.Character + " " + curBlue++);
                }
                if (target.ID == (int)ArcDPSEnums.TrashID.GreenGuardian)
                {
                    target.OverrideName(target.Character + " " + curGreen++);
                }
            }
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
               ArcDPSEnums.TrashID.Seekers
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            var lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.ValeGuardian:
                    var magicStorms = cls.Where(x => x.SkillId == 31419).ToList();
                    foreach (AbstractCastEvent c in magicStorms)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(true, start + c.ExpectedDuration, 180, (start, end), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    int distributedMagicDuration = 6700;
                    int arenaRadius = 1600;
                    int impactDuration = 110;
                    var distributedMagicGreen = cls.Where(x => x.SkillId == 31750).ToList();
                    foreach (AbstractCastEvent c in distributedMagicGreen)
                    {
                        int start = (int)c.Time;
                        int end = start + distributedMagicDuration;
                        replay.Decorations.Add(new PieDecoration(true, start + distributedMagicDuration, arenaRadius, 151, 120, (start, end), "rgba(0,255,0,0.1)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f))));
                        replay.Decorations.Add(new PieDecoration(true, 0, arenaRadius, 151, 120, (end, end + impactDuration), "rgba(0,255,0,0.3)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f))));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(0,255,0,0.2)", new PositionConnector(new Point3D(-5449.0f, -20219.0f, 0.0f))));
                    }
                    var distributedMagicBlue = cls.Where(x => x.SkillId == 31340).ToList();
                    foreach (AbstractCastEvent c in distributedMagicBlue)
                    {
                        int start = (int)c.Time;
                        int end = start + distributedMagicDuration;
                        replay.Decorations.Add(new PieDecoration(true, start + distributedMagicDuration, arenaRadius, 31, 120, (start, end), "rgba(0,255,0,0.1)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f))));
                        replay.Decorations.Add(new PieDecoration(true, 0, arenaRadius, 31, 120, (end, end + impactDuration), "rgba(0,255,0,0.3)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f))));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(0,255,0,0.2)", new PositionConnector(new Point3D(-4063.0f, -20195.0f, 0.0f))));
                    }
                    var distributedMagicRed = cls.Where(x => x.SkillId == 31391).ToList();
                    foreach (AbstractCastEvent c in distributedMagicRed)
                    {
                        int start = (int)c.Time;
                        int end = start + distributedMagicDuration;
                        replay.Decorations.Add(new PieDecoration(true, start + distributedMagicDuration, arenaRadius, 271, 120, (start, end), "rgba(0,255,0,0.1)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f))));
                        replay.Decorations.Add(new PieDecoration(true, 0, arenaRadius, 271, 120, (end, end + impactDuration), "rgba(0,255,0,0.3)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f, 0))));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(0,255,0,0.2)", new PositionConnector(new Point3D(-4735.0f, -21407.0f, 0.0f))));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.BlueGuardian:
                    replay.Decorations.Add(new CircleDecoration(false, 0, 1500, lifespan, "rgba(0, 0, 255, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.GreenGuardian:
                    replay.Decorations.Add(new CircleDecoration(false, 0, 1500, lifespan, "rgba(0, 255, 0, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.RedGuardian:
                    replay.Decorations.Add(new CircleDecoration(false, 0, 1500, lifespan, "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Seekers:
                    replay.Decorations.Add(new CircleDecoration(false, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                default:
                    break;
            }
        }
    }
}
