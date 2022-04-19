﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class BanditTrio : SalvationPass
    {
        public BanditTrio(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            new PlayerBuffApplyMechanic(34108, "Shell-Shocked", new MechanicPlotlySetting("circle-open",Colors.DarkGreen), "Launchd","Shell-Shocked (Launched from pad)", "Shell-Shocked",0),
            new HitOnPlayerMechanic(34448, "Overhead Smash", new MechanicPlotlySetting("triangle-left",Colors.Orange), "Smash","Overhead Smash (CC Attack Berg)", "CC Smash",0),
            new HitOnPlayerMechanic(34383, "Hail of Bullets", new MechanicPlotlySetting("triangle-right-open",Colors.Red), "Zane Cone","Hail of Bullets (Zane Cone Shot)", "Hail of Bullets",0),
            new HitOnPlayerMechanic(34344, "Fiery Vortex", new MechanicPlotlySetting("circle-open",Colors.Yellow), "Tornado","Fiery Vortex (Tornado)", "Tornado",250),
            });
            Extension = "trio";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = "https://i.imgur.com/UZZQUdf.png";
            EncounterCategoryInformation.InSubCategoryOrder = 2;
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Narella
            };
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Berg,
                (int)ArcDPSEnums.TargetID.Zane,
                (int)ArcDPSEnums.TargetID.Narella
            };
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/dDvhEOP.png",
                            (1000, 913),
                            (-2900, -12251, 2561, -7265)/*,
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210)*/);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                IReadOnlyList<Agent> prisoners = agentData.GetNPCsByID((int)ArcDPSEnums.TrashID.Prisoner2);
                var prisonerDeaths = new List<DeadEvent>();
                foreach (Agent prisoner in prisoners)
                {
                    prisonerDeaths.AddRange(combatData.GetDeadEvents(prisoner));
                }
                if (prisonerDeaths.Count == 0)
                {
                    SetSuccessByCombatExit(new HashSet<int>(GetSuccessCheckIds()), combatData, fightData, playerAgents);
                }
            }
        }

        private static void SetPhasePerTarget(AbstractSingleActor target, List<PhaseData> phases, ParsedLog log)
        {
            long fightDuration = log.FightData.FightEnd;
            EnterCombatEvent phaseStart = log.CombatData.GetEnterCombatEvents(target.AgentItem).LastOrDefault();
            if (phaseStart != null)
            {
                long start = phaseStart.Time;
                DeadEvent phaseEnd = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
                long end = fightDuration;
                if (phaseEnd != null)
                {
                    end = phaseEnd.Time;
                }
                var phase = new PhaseData(start, Math.Min(end, log.FightData.FightEnd));
                phase.AddTarget(target);
                switch (target.ID)
                {
                    case (int)ArcDPSEnums.TargetID.Narella:
                        phase.Name = "Narella";
                        break;
                    case (int)ArcDPSEnums.TargetID.Berg:
                        phase.Name = "Berg";
                        break;
                    case (int)ArcDPSEnums.TargetID.Zane:
                        phase.Name = "Zane";
                        break;
                    default:
                        throw new MissingKeyActorsException("Unknown target in Bandit Trio");
                }
                phases.Add(phase);
            }
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Berg,
                (int)ArcDPSEnums.TargetID.Zane,
                (int)ArcDPSEnums.TargetID.Narella
            };
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor berg = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Berg);
            if (berg == null)
            {
                throw new MissingKeyActorsException("Berg not found");
            }
            AbstractSingleActor zane = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Zane);
            if (zane == null)
            {
                throw new MissingKeyActorsException("Zane not found");
            }
            AbstractSingleActor narella = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Narella);
            if (narella == null)
            {
                throw new MissingKeyActorsException("Narella not found");
            }
            phases[0].AddTargets(Targets);
            if (!requirePhases)
            {
                return phases;
            }
            foreach (AbstractSingleActor target in Targets)
            {
                SetPhasePerTarget(target, phases, log);
            }
            return phases;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.BanditSaboteur,
                ArcDPSEnums.TrashID.Warg,
                ArcDPSEnums.TrashID.CagedWarg,
                ArcDPSEnums.TrashID.BanditAssassin,
                ArcDPSEnums.TrashID.BanditSapperTrio ,
                ArcDPSEnums.TrashID.BanditDeathsayer,
                ArcDPSEnums.TrashID.BanditBrawler,
                ArcDPSEnums.TrashID.BanditBattlemage,
                ArcDPSEnums.TrashID.BanditCleric,
                ArcDPSEnums.TrashID.BanditBombardier,
                ArcDPSEnums.TrashID.BanditSniper,
                ArcDPSEnums.TrashID.NarellaTornado,
                ArcDPSEnums.TrashID.OilSlick,
                ArcDPSEnums.TrashID.Prisoner1,
                ArcDPSEnums.TrashID.Prisoner2
            };
        }

        internal override string GetLogicName(ParsedLog log)
        {
            return "Bandit Trio";
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Berg:
                    break;
                case (int)ArcDPSEnums.TargetID.Zane:
                    var bulletHail = cls.Where(x => x.SkillId == 34383).ToList();
                    foreach (AbstractCastEvent c in bulletHail)
                    {
                        int start = (int)c.Time;
                        int firstConeStart = start;
                        int secondConeStart = start + 800;
                        int thirdConeStart = start + 1600;
                        int firstConeEnd = firstConeStart + 400;
                        int secondConeEnd = secondConeStart + 400;
                        int thirdConeEnd = thirdConeStart + 400;
                        int radius = 1500;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start);
                        if (facing != null)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, facing, 28, (firstConeStart, firstConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, facing, 54, (secondConeStart, secondConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, facing, 81, (thirdConeStart, thirdConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                        }
                    }
                    break;

                case (int)ArcDPSEnums.TargetID.Narella:
                    break;
                default:
                    break;
            }
        }
    }
}
