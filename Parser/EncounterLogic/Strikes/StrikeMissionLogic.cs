using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class StrikeMissionLogic : FightLogic
    {

        protected StrikeMissionLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.Instanced10;
            EncounterCategoryInformation.Category = FightCategory.Strike;
        }

        protected virtual void SetSuccessByDeath(CombatData combatData, FightData fightData, IReadOnlyCollection<Agent> playerAgents, bool all)
        {
            SetSuccessByDeath(combatData, fightData, playerAgents, all, GenericTriggerID);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            var strikeRewardIDs = new HashSet<ulong>
                {
                    993
                };
            IReadOnlyList<RewardEvent> rewards = combatData.GetRewardEvents();
            RewardEvent reward = rewards.FirstOrDefault(x => strikeRewardIDs.Contains(x.RewardID));
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
            else
            {
                SetSuccessByDeath(combatData, fightData, playerAgents, true);
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
