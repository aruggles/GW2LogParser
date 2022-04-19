﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class Skorvald : ShatteredObservatory
    {
        public Skorvald(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(39916, "Combustion Rush", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge",0),
            new HitOnPlayerMechanic(39615, "Combustion Rush", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge",0),
            new HitOnPlayerMechanic(39581, "Combustion Rush", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Charge","Combustion Rush", "Charge ",0),
            new HitOnPlayerMechanic(39910, "Punishing Kick", new MechanicPlotlySetting("triangle-right-open","rgb(200,0,200)"), "Add Kick","Punishing Kick (Single purple Line, Add)", "Kick (Add)",0),
            new HitOnPlayerMechanic(38896, "Punishing Kick", new MechanicPlotlySetting("triangle-right","rgb(200,0,200)"), "Kick","Punishing Kick (Single purple Line)", "Kick",0),
            new HitOnPlayerMechanic(39534, "Cranial Cascade", new MechanicPlotlySetting("triangle-right-open","rgb(255,200,0)"), "Add Cone KB","Cranial Cascade (3 purple Line Knockback, Add)", "Small Cone KB (Add)",0),
            new HitOnPlayerMechanic(39686, "Cranial Cascade", new MechanicPlotlySetting("triangle-right","rgb(255,200,0)"), "Cone KB","Cranial Cascade (3 purple Line Knockback)", "Small Cone KB",0),
            new HitOnPlayerMechanic(39845, "Radiant Fury", new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Burn Circle","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new HitOnPlayerMechanic(38926, "Radiant Fury", new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Burn Circle","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new HitOnPlayerMechanic(39257, "Focused Anger", new MechanicPlotlySetting("triangle-down","rgb(255,100,0)"), "Large Cone KB","Focused Anger (Large Cone Overhead Crosshair Knockback)", "Large Cone Knockback",0),
            new HitOnPlayerMechanic(39031, "Horizon Strike", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new HitOnPlayerMechanic(39507, "Horizon Strike", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new HitOnPlayerMechanic(39846, "Crimson Dawn", new MechanicPlotlySetting("circle","rgb(50,0,0)"), "Horizon Strike End","Crimson Dawn (almost Full platform attack after Horizon Strike)", "Horizon Strike (last)",0),
            new HitOnPlayerMechanic(39228, "Solar Cyclone", new MechanicPlotlySetting("asterisk-open","rgb(140,0,140)"), "Cyclone","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new HitOnPlayerMechanic(39228, "Solar Cyclone", new MechanicPlotlySetting("asterisk-open","rgb(140,0,140)"), "Cyclone","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new PlayerBuffApplyMechanic(791, "Fear", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0, (ba, log) => ba.AppliedDuration == 3000), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new PlayerBuffApplyMechanic(39131, "Fixate", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new HitOnPlayerMechanic(39491, "Explode", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Bloom Expl","Hit by Solar Bloom Explosion", "Bloom Explosion",0), //shockwave, not damage? (damage is 50% max HP, not tracked)
            new HitOnPlayerMechanic(39911, "Spiral Strike", new MechanicPlotlySetting("circle-open","rgb(0,200,0)"), "Spiral","Hit after Warp (Jump to Player with overhead bomb)", "Spiral Strike",0),
            new HitOnPlayerMechanic(39133, "Wave of Mutilation", new MechanicPlotlySetting("triangle-sw","rgb(0,200,0)"), "KB Jump","Hit by KB Jump (player targeted)", "Knockback jump",0),
            });
            Extension = "skorv";
            Icon = "https://i.imgur.com/B1nhJ9m.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/EhblRgb.png",
                            (987, 1000),
                            (-22267, 14955, -17227, 20735)/*,
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462)*/);
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor skorvald = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Skorvald);
            if (skorvald == null)
            {
                throw new MissingKeyActorsException("Skorvald not found");
            }
            phases[0].AddTarget(skorvald);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 762, skorvald, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Split " + (i) / 2;
                    var ids = new List<int>
                    {
                        (int)ArcDPSEnums.TrashID.FluxAnomaly4,
                        (int)ArcDPSEnums.TrashID.FluxAnomaly3,
                        (int)ArcDPSEnums.TrashID.FluxAnomaly2,
                        (int)ArcDPSEnums.TrashID.FluxAnomaly1,
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(skorvald);
                }
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, friendlies, extensions);
            AbstractSingleActor skorvald = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Skorvald);
            if (skorvald == null)
            {
                throw new MissingKeyActorsException("Skorvald not found");
            }
            skorvald.OverrideName("Skorvald");
            int count = 0;
            foreach (NPC target in _targets)
            {
                if (target.ID == (int)ArcDPSEnums.TrashID.FluxAnomaly1 ||
                    target.ID == (int)ArcDPSEnums.TrashID.FluxAnomaly2 ||
                    target.ID == (int)ArcDPSEnums.TrashID.FluxAnomaly3 ||
                    target.ID == (int)ArcDPSEnums.TrashID.FluxAnomaly4)
                {
                    target.OverrideName(target.Character + " " + (++count));
                }
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Skorvald);
            if (target == null)
            {
                throw new MissingKeyActorsException("Skorvald not found");
            }
            if (combatData.GetBuildEvent().Build >= 106277)
            {
                // Agent check not reliable, produces false positives and regular false negatives
                /*if (agentData.GetNPCsByID(16725).Any() && agentData.GetNPCsByID(11245).Any())
                {
                    return FightData.CMStatus.CM;
                }*/
                // Check some CM skills instead, not perfect but helps, 
                // Solar Bolt is the first thing he tries to cast, that looks very consistent
                // If the phase 1 is super fast to the point skorvald does not cast anything, supernova should be there
                // Otherwise we are looking at a super fast phase 1 (< 7 secondes) where the team ggs just before supernova
                // Joining the encounter mid fight may also yield a false negative but at that point the log is incomplete already
                var cmSkills = new HashSet<long>
                {
                    39313,// CM Solar Bolt
                    39225,// CM Supernova
                };
                if (combatData.GetSkills().Intersect(cmSkills).Any())
                {
                    return FightData.CMStatus.CM;
                }
                return FightData.CMStatus.NoCM;
            }
            else
            {
                return (target.GetHealth(combatData) == 5551340) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
            }
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Skorvald,
                (int)ArcDPSEnums.TrashID.FluxAnomaly4,
                (int)ArcDPSEnums.TrashID.FluxAnomaly3,
                (int)ArcDPSEnums.TrashID.FluxAnomaly2,
                (int)ArcDPSEnums.TrashID.FluxAnomaly1,
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            // reward or death worked
            if (fightData.Success)
            {
                return;
            }
            AbstractSingleActor skorvald = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Skorvald);
            if (skorvald == null)
            {
                throw new MissingKeyActorsException("Skorvald not found");
            }
            AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(skorvald.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
            if (lastDamageTaken != null)
            {
                BuffApplyEvent invul895Apply = combatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == skorvald.AgentItem && x.Time > lastDamageTaken.Time - 500).LastOrDefault();
                if (invul895Apply != null)
                {
                    fightData.SetSuccess(true, Math.Min(invul895Apply.Time, lastDamageTaken.Time));
                }
            }
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.SolarBloom
            };
        }
    }
}
