using Gw2LogParser.Parser.Data.Agents;
using System.IO;

namespace Gw2LogParser.Parser.Data.El.Actors
{
    class PlayerNonSquad : AbstractPlayer
    {
        private static int NonSquadPlayers = 0;
        // Constructors
        internal PlayerNonSquad(Agent agent) : base(agent)
        {
            if (agent.Type == Agent.AgentType.Player)
            {
                throw new InvalidDataException("Agent is not a squad Player");
            }
            Account = "Non Squad Player " + (++NonSquadPlayers);
        }
        protected override void TrimCombatReplay(ParsedLog log)
        {
            if (!AgentItem.IsNotInSquadFriendlyPlayer)
            {
                TrimCombatReplay(log, CombatReplay, AgentItem);
            }
        }
    }
}
