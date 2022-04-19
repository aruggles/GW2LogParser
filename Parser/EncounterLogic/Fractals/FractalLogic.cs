﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.Events;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class FractalLogic : FightLogic
    {
        protected FractalLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.Instanced5;
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(37695, "Flux Bomb", new MechanicPlotlySetting("circle","rgb(150,0,255)",10), "Flux","Flux Bomb application", "Flux Bomb",0),
            new HitOnPlayerMechanic(36393, "Flux Bomb", new MechanicPlotlySetting("circle-open","rgb(150,0,255)",10), "Flux dmg","Flux Bomb hit", "Flux Bomb dmg",0),
            new SpawnMechanic(19684, "Fractal Vindicator", new MechanicPlotlySetting("star-diamond-open","rgb(0,0,0)",10), "Vindicator","Fractal Vindicator spawned", "Vindicator spawn",0),
            });
            EncounterCategoryInformation.Category = FightCategory.Fractal;
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 762, mainTarget, false, true));
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].AddTarget(mainTarget);
            }
            return phases;
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            // check reward
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            RewardEvent reward = combatData.GetRewardEvents().LastOrDefault(x => x.RewardType == 13);
            AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
            if (lastDamageTaken != null)
            {
                if (reward != null && Math.Abs(lastDamageTaken.Time - reward.Time) < 100)
                {
                    fightData.SetSuccess(true, Math.Min(lastDamageTaken.Time, reward.Time));
                }
                else
                {
                    SetSuccessByDeath(combatData, fightData, playerAgents, true, GenericTriggerID);
                }
            }
        }
        protected static long GetFightOffsetByFirstInvulFilter(FightData fightData, AgentData agentData, List<Combat> combatData, int targetID, long invulID, long invulGainOffset)
        {
            // Find target
            Agent target = agentData.GetNPCsByID(targetID).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            // check first invul gain at the start of the fight
            Combat invulGain = combatData.FirstOrDefault(x => x.DstMatchesAgent(target) && x.IsBuffApply() && x.SkillID == invulID);
            // get invul lost
            Combat invulLost = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target) && x.IsBuffRemove == ArcDPSEnums.BuffRemove.All && x.SkillID == invulID);
            // invul loss matches the gained invul
            if (invulGain != null && invulLost != null && invulLost.Time > invulGain.Time)
            {
                // check against offset
                if (invulGain.Time - fightData.LogStart < invulGainOffset)
                {
                    return invulLost.Time + 1;
                }
            }
            else if (invulLost != null)
            {
                // only invul lost, missing buff apply event
                Combat enterCombat = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target) && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat);
                // verify that first enter combat matches the moment invul is lost
                if (enterCombat != null && Math.Abs(enterCombat.Time - invulLost.Time) < ParserHelper.ServerDelayConstant)
                {
                    return invulLost.Time + 1;
                }
            }
            return fightData.LogStart;
        }
    }
}
