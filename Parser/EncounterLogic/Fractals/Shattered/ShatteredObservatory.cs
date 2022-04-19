using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class ShatteredObservatory : FractalLogic
    {
        public ShatteredObservatory(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.ShatteredObservatory;
        }

        protected static HashSet<Agent> GetParticipatingPlayerAgents(AbstractSingleActor target, CombatData combatData, IReadOnlyCollection<Agent> playerAgents)
        {
            if (target == null)
            {
                return new HashSet<Agent>();
            }
            var participatingPlayerAgents = new HashSet<Agent>(combatData.GetDamageTakenData(target.AgentItem).Where(x => playerAgents.Contains(x.From.GetFinalMaster())).Select(x => x.From.GetFinalMaster()));
            participatingPlayerAgents.UnionWith(combatData.GetDamageData(target.AgentItem).Where(x => playerAgents.Contains(x.To.GetFinalMaster())).Select(x => x.To.GetFinalMaster()));
            return participatingPlayerAgents;
        }

        /// <summary>
        /// Returns true if the buff count was not reached so that another method can be called, if necessary
        /// </summary>
        protected static bool SetSuccessByBuffCount(CombatData combatData, FightData fightData, HashSet<Agent> playerAgents, AbstractSingleActor target, long buffID, int count)
        {
            if (target == null)
            {
                return false;
            }
            var invulsTarget = GetFilteredList(combatData, buffID, target, true).Where(x => x.Time >= 0).ToList();
            if (invulsTarget.Count == count)
            {
                AbstractBuffEvent last = invulsTarget.Last();
                if (!(last is BuffApplyEvent))
                {
                    SetSuccessByCombatExit(new List<AbstractSingleActor> { target }, combatData, fightData, playerAgents);
                    return false;
                }
            }
            return true;
        }
    }
}
