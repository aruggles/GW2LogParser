using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class ShatteredFractal : FractalLogic
    {
        public ShatteredFractal(int triggerID) : base(triggerID)
        {

        }

        protected static void SetSuccessByBuffCount(CombatData combatData, FightData fightData, HashSet<Agent> playerAgents, NPC target, long buffID, int count)
        {
            if (target == null)
            {
                return;
            }
            List<AbstractBuffEvent> invulsTarget = GetFilteredList(combatData, buffID, target, true);
            if (invulsTarget.Count == count)
            {
                AbstractBuffEvent last = invulsTarget.Last();
                if (!(last is BuffApplyEvent))
                {
                    SetSuccessByCombatExit(new List<NPC> { target }, combatData, fightData, playerAgents);
                }
            }
        }
    }
}
