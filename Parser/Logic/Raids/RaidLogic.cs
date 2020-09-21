﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class RaidLogic : FightLogic
    {
        protected enum FallBackMethod { None, Death, CombatExit }

        protected FallBackMethod GenericFallBackMethod { get; set; } = FallBackMethod.Death;

        protected RaidLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.Instanced10;
        }

        protected virtual List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                GenericTriggerID
            };
        }

        protected void SetSuccessByCombatExit(HashSet<int> targetIds, CombatData combatData, FightData fightData, HashSet<Agent> playerAgents)
        {
            var targets = Targets.Where(x => targetIds.Contains(x.ID)).ToList();
            SetSuccessByCombatExit(targets, combatData, fightData, playerAgents);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<Agent> playerAgents)
        {
            var raidRewardsTypes = new HashSet<int>
                {
                    55821,
                    60685,
                    914,
                    22797
                };
            List<RewardEvent> rewards = combatData.GetRewardEvents();
            RewardEvent reward = rewards.FirstOrDefault(x => raidRewardsTypes.Contains(x.RewardType));
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
            else
            {
                switch (GenericFallBackMethod)
                {
                    case FallBackMethod.Death:
                        SetSuccessByDeath(combatData, fightData, playerAgents, true, GetSuccessCheckIds());
                        break;
                    case FallBackMethod.CombatExit:
                        SetSuccessByCombatExit(new HashSet<int>(GetSuccessCheckIds()), combatData, fightData, playerAgents);
                        break;
                    default:
                        break;
                }
            }
        }

        protected static void AdjustTimeRefreshBuff(Dictionary<Agent, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, long id)
        {
            if (buffsById.TryGetValue(id, out List<AbstractBuffEvent> buffList))
            {
                var agentsToSort = new HashSet<Agent>();
                foreach (AbstractBuffEvent be in buffList)
                {
                    if (be is AbstractBuffRemoveEvent abre)
                    {
                        // to make sure remove events are before applications
                        abre.OverrideTime(abre.Time - 1);
                        agentsToSort.Add(abre.To);
                    }
                }
                if (buffList.Count > 0)
                {
                    buffsById[id].Sort((x, y) => x.Time.CompareTo(y.Time));
                }
                foreach (Agent a in agentsToSort)
                {
                    buffsByDst[a].Sort((x, y) => x.Time.CompareTo(y.Time));
                }
            }
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }
    }
}
