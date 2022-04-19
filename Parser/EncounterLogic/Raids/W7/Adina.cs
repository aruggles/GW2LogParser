﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.Events;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Events.Movement;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class Adina : TheKeyOfAhdashim
    {
        public Adina(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new PlayerBuffApplyMechanic(56593, "Radiant Blindness", new MechanicPlotlySetting("circle",Colors.Magenta), "R.Blind", "Unremovable blindness", "Radiant Blindness", 0),
                new PlayerBuffApplyMechanic(56440, "Eroding Curse", new MechanicPlotlySetting("square",Colors.LightPurple), "Curse", "Stacking damage debuff from Hand of Erosion", "Eroding Curse", 0),
                new HitOnPlayerMechanic(56648, "Boulder Barrage", new MechanicPlotlySetting("hexagon",Colors.Red), "Boulder", "Hit by boulder thrown during pillars", "Boulder Barrage", 0),
                new HitOnPlayerMechanic(56390, "Perilous Pulse", new MechanicPlotlySetting("triangle-right",Colors.Pink), "Perilous Pulse", "Perilous Pulse", "Perilous Pulse", 0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
                new HitOnPlayerMechanic(56141, "Stalagmites", new MechanicPlotlySetting("pentagon",Colors.Red), "Mines", "Hit by mines", "Mines", 0),
                new HitOnPlayerMechanic(56114, "Diamond Palisade", new MechanicPlotlySetting("star-diamond",Colors.Pink), "Eye", "Looked at Eye", "Looked at Eye", 0),
                new SkillOnPlayerMechanic(56035, "Quantum Quake", new MechanicPlotlySetting("hourglass",Colors.Brown), "S.Thrower", "Hit by rotating SandThrower", "SandThrower", 0, (de, log) => de.HasKilled),
                new SkillOnPlayerMechanic(56381, "Quantum Quake", new MechanicPlotlySetting("hourglass",Colors.Brown), "S.Thrower", "Hit by rotating SandThrower", "SandThrower", 0, (de, log) => de.HasKilled),
            });
            Extension = "adina";
            Icon = "https://wiki.guildwars2.com/images/a/a0/Mini_Earth_Djinn.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(56351, 56351, InstantCastFinder.DefaultICD), // Seismic Suffering
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var attackTargets = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.AttackTarget).ToList();
            long first = 0;
            long final = fightData.FightEnd;
            foreach (Combat at in attackTargets)
            {
                Agent hand = agentData.GetAgent(at.DstAgent, at.Time);
                Agent atAgent = agentData.GetAgent(at.SrcAgent, at.Time);
                var attackables = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Targetable && x.SrcMatchesAgent(atAgent)).ToList();
                var attackOn = attackables.Where(x => x.DstAgent == 1 && x.Time >= first + 2000).Select(x => x.Time).ToList();
                var attackOff = attackables.Where(x => x.DstAgent == 0 && x.Time >= first + 2000).Select(x => x.Time).ToList();
                var posFacingHP = combatData.Where(x => x.SrcMatchesAgent(hand) && (x.IsStateChange == ArcDPSEnums.StateChange.Position || x.IsStateChange == ArcDPSEnums.StateChange.Rotation || x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)).ToList();
                Combat pos = posFacingHP.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.Position);
                int id = (int)ArcDPSEnums.TrashID.HandOfErosion;
                if (pos != null)
                {
                    (float x, float y, _) = AbstractMovementEvent.UnpackMovementData(pos.DstAgent, 0);
                    if ((Math.Abs(x - 15570.5) < 10 && Math.Abs(y + 693.117) < 10) ||
                            (Math.Abs(x - 14277.2) < 10 && Math.Abs(y + 2202.52) < 10))
                    {
                        id = (int)ArcDPSEnums.TrashID.HandOfEruption;
                    }
                }
                for (int i = 0; i < attackOn.Count; i++)
                {
                    long start = attackOn[i];
                    long end = final;
                    if (i <= attackOff.Count - 1)
                    {
                        end = attackOff[i];
                    }
                    Agent extra = agentData.AddCustomAgent(start, end, Agent.AgentType.NPC, hand.Name, hand.Spec, id, false, hand.Toughness, hand.Healing, hand.Condition, hand.Concentration, hand.HitboxWidth, hand.HitboxHeight);
                    foreach (Combat c in combatData)
                    {
                        if (extra.InAwareTimes(c.Time))
                        {
                            if (c.SrcMatchesAgent(hand, extensions))
                            {
                                c.OverrideSrcAgent(extra.AgentValue);
                            }
                            if (c.DstMatchesAgent(hand, extensions))
                            {
                                c.OverrideDstAgent(extra.AgentValue);
                            }
                        }
                    }
                    foreach (Combat c in posFacingHP)
                    {
                        var cExtra = new Combat(c);
                        cExtra.OverrideTime(extra.FirstAware);
                        cExtra.OverrideSrcAgent(extra.AgentValue);
                        combatData.Add(cExtra);
                    }
                }
            }
            ComputeFightTargets(agentData, combatData, extensions);
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Adina,
                (int)ArcDPSEnums.TrashID.HandOfErosion,
                (int)ArcDPSEnums.TrashID.HandOfEruption
            };
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Adina);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Adina not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Split phases
            List<AbstractBuffEvent> invuls = GetFilteredList(log.CombatData, 762, mainTarget, true);
            long start = 0;
            var splitPhases = new List<PhaseData>();
            var splitPhaseEnds = new List<long>();
            for (int i = 0; i < invuls.Count; i++)
            {
                PhaseData splitPhase;
                AbstractBuffEvent be = invuls[i];
                if (be is BuffApplyEvent)
                {
                    start = be.Time;
                    if (i == invuls.Count - 1)
                    {
                        splitPhase = new PhaseData(start, log.FightData.FightEnd, "Split " + (i / 2 + 1));
                        splitPhaseEnds.Add(log.FightData.FightEnd);
                        AddTargetsToPhaseAndFit(splitPhase, new List<int> { (int)ArcDPSEnums.TrashID.HandOfErosion, (int)ArcDPSEnums.TrashID.HandOfEruption }, log);
                        splitPhases.Add(splitPhase);
                    }
                }
                else
                {
                    long end = be.Time;
                    splitPhase = new PhaseData(start, end, "Split " + (i / 2 + 1));
                    splitPhaseEnds.Add(end);
                    AddTargetsToPhaseAndFit(splitPhase, new List<int> { (int)ArcDPSEnums.TrashID.HandOfErosion, (int)ArcDPSEnums.TrashID.HandOfEruption }, log);
                    splitPhases.Add(splitPhase);
                }
            }
            // Main phases
            var mainPhases = new List<PhaseData>();
            var pillarApplies = log.CombatData.GetBuffData(56204).OfType<BuffApplyEvent>().Where(x => x.To == mainTarget.AgentItem).ToList();
            Dictionary<long, List<BuffApplyEvent>> pillarAppliesGroupByTime = ParserHelper.GroupByTime(pillarApplies);
            var mainPhaseEnds = new List<long>();
            foreach (KeyValuePair<long, List<BuffApplyEvent>> pair in pillarAppliesGroupByTime)
            {
                if (pair.Value.Count == 6)
                {
                    mainPhaseEnds.Add(pair.Key);
                }
            }
            AbstractCastEvent boulderBarrage = mainTarget.GetCastEvents(log, 0, log.FightData.FightEnd).FirstOrDefault(x => x.SkillId == 56648 && x.Time < 6000);
            start = boulderBarrage == null ? 0 : boulderBarrage.EndTime;
            if (mainPhaseEnds.Any())
            {
                int phaseIndex = 1;
                foreach (long quantumQake in mainPhaseEnds)
                {
                    var curPhaseStart = splitPhaseEnds.LastOrDefault(x => x < quantumQake);
                    if (curPhaseStart == 0)
                    {
                        curPhaseStart = start;
                    }
                    long nextPhaseStart = splitPhaseEnds.FirstOrDefault(x => x > quantumQake);
                    if (nextPhaseStart != 0)
                    {
                        start = nextPhaseStart;
                        phaseIndex = splitPhaseEnds.IndexOf(start) + 1;
                    }
                    mainPhases.Add(new PhaseData(curPhaseStart, quantumQake, "Phase " + phaseIndex));
                }
                if (start != mainPhases.Last().Start)
                {
                    mainPhases.Add(new PhaseData(start, log.FightData.FightEnd, "Phase " + (phaseIndex + 1)));
                }
            }
            else if (start > 0 && !invuls.Any())
            {
                // no split
                mainPhases.Add(new PhaseData(start, log.FightData.FightEnd, "Phase 1"));
            }

            foreach (PhaseData phase in mainPhases)
            {
                phase.AddTarget(mainTarget);
            }
            phases.AddRange(mainPhases);
            phases.AddRange(splitPhases);
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            //
            try
            {
                var splitPhasesMap = new List<string>()
                {
                        "https://i.imgur.com/gJ55jKy.png",
                        "https://i.imgur.com/c2Oz5bj.png",
                        "https://i.imgur.com/P4SGbrc.png",
                };
                var mainPhasesMap = new List<string>()
                {
                        "https://i.imgur.com/IQn2RJV.png",
                        "https://i.imgur.com/3pO7eCB.png",
                        "https://i.imgur.com/ZFw590w.png",
                        "https://i.imgur.com/2P7UE8q.png"
                };
                var crMaps = new List<string>();
                int mainPhaseIndex = 0;
                int splitPhaseIndex = 0;
                for (int i = 1; i < phases.Count; i++)
                {
                    PhaseData phaseData = phases[i];
                    if (mainPhases.Contains(phaseData))
                    {
                        if (mainPhasesMap.Contains(crMaps.LastOrDefault()))
                        {
                            splitPhaseIndex++;
                        }
                        crMaps.Add(mainPhasesMap[mainPhaseIndex++]);
                    }
                    else
                    {
                        if (splitPhasesMap.Contains(crMaps.LastOrDefault()))
                        {
                            mainPhaseIndex++;
                        }
                        crMaps.Add(splitPhasesMap[splitPhaseIndex++]);
                    }
                }
                GetCombatReplayMap(log).MatchMapsToPhases(crMaps, phases, log.FightData.FightEnd);
            }
            catch (Exception)
            {
                log.UpdateProgressWithCancellationCheck("Failed to associate Adina Combat Replay maps");
            }

            //
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/IQn2RJV.png",
                            (866, 1000),
                            (13840, -2698, 15971, -248)/*,
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970)*/);
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Adina);
            if (target == null)
            {
                throw new MissingKeyActorsException("Adina not found");
            }
            return (target.GetHealth(combatData) > 23e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
