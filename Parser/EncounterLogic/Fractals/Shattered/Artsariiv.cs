﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class Artsariiv : ShatteredObservatory
    {
        public Artsariiv(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(38880, "Corporeal Reassignment", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Skull","Exploding Skull mechanic application","Corporeal Reassignment",0),
            new HitOnPlayerMechanic(38977, "Vault", new MechanicPlotlySetting("triangle-down-open","rgb(255,200,0)"), "Vault","Vault from Big Adds", "Vault (Add)",0),
            new HitOnPlayerMechanic(39925, "Slam", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Slam","Slam (Vault) from Boss", "Vault (Arts)",0),
            new HitOnPlayerMechanic(39469, "Teleport Lunge", new MechanicPlotlySetting("star-triangle-down-open","rgb(255,140,0)"), "3 Jump","Triple Jump Mid->Edge", "Triple Jump",0),
            new HitOnPlayerMechanic(39035, "Astral Surge", new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Floor Circle","Different sized spiraling circles", "1000 Circles",0),
            new HitOnPlayerMechanic(39029, "Red Marble", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Marble","Red KD Marble after Jump", "Red Marble",0),
            new HitOnPlayerMechanic(39863, "Red Marble", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Marble","Red KD Marble after Jump", "Red Marble",0),
            new PlayerBuffApplyMechanic(791, "Fear", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)" ,0, (ba, log) => ba.AppliedDuration == 3000), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new SpawnMechanic(17630, "Spark", new MechanicPlotlySetting("star","rgb(0,255,255)"),"Spark","Spawned a Spark (missed marble)", "Spark",0),
            });
            Extension = "arts";
            Icon = "https://i.imgur.com/aFlYs1I.png";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
                            (914, 914),
                            (8991, 112, 11731, 2812)/*,
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462)*/);
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
            };
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Artsariiv,
                (int)ArcDPSEnums.TrashID.CloneArtsariiv
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.TemporalAnomaly,
                ArcDPSEnums.TrashID.Spark,
                ArcDPSEnums.TrashID.SmallArtsariiv,
                ArcDPSEnums.TrashID.MediumArtsariiv,
                ArcDPSEnums.TrashID.BigArtsariiv,
            };
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor artsariiv = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Artsariiv);
            if (artsariiv == null)
            {
                throw new MissingKeyActorsException("Artsariiv not found");
            }
            phases[0].AddTarget(artsariiv);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 762, artsariiv, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Split " + (i) / 2;
                    var ids = new List<int>
                    {
                       (int)ArcDPSEnums.TrashID.CloneArtsariiv,
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(artsariiv);
                }
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var artsariivs = new List<Agent>(agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Artsariiv));
            if (artsariivs.Any())
            {
                artsariivs.Remove(artsariivs.MaxBy(x => x.LastAware - x.FirstAware));
                if (artsariivs.Any())
                {
                    foreach (Agent subartsariiv in artsariivs)
                    {
                        subartsariiv.OverrideID(ArcDPSEnums.TrashID.CloneArtsariiv);
                    }
                }
                agentData.Refresh();
            }
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, friendlies, extensions);
            int count = 0;
            foreach (NPC trashMob in _trashMobs)
            {
                if (trashMob.ID == (int)ArcDPSEnums.TrashID.SmallArtsariiv)
                {
                    trashMob.OverrideName("Small " + trashMob.Character);
                }
                if (trashMob.ID == (int)ArcDPSEnums.TrashID.MediumArtsariiv)
                {
                    trashMob.OverrideName("Medium " + trashMob.Character);
                }
                if (trashMob.ID == (int)ArcDPSEnums.TrashID.BigArtsariiv)
                {
                    trashMob.OverrideName("Big " + trashMob.Character);
                }
            }
            foreach (NPC target in _targets)
            {
                if (target.ID == (int)ArcDPSEnums.TrashID.CloneArtsariiv)
                {
                    target.OverrideName("Clone " + target.Character + " " + (++count));
                }
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.CMnoName;
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<Combat> combatData)
        {
            return GetFightOffsetByFirstInvulFilter(fightData, agentData, combatData, (int)ArcDPSEnums.TargetID.Artsariiv, 762, 1500);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            // reward or death worked
            if (fightData.Success)
            {
                return;
            }
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Artsariiv);
            if (target == null)
            {
                throw new MissingKeyActorsException("Artsariiv not found");
            }
            SetSuccessByBuffCount(combatData, fightData, GetParticipatingPlayerAgents(target, combatData, playerAgents), target, 762, 4);
        }
    }
}
