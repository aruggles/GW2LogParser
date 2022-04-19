using Gw2LogParser.Parser.Data.El.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class PlayerCombatReplayDescription : AbstractSingleActorCombatReplayDescription
    {
        public int Group { get; }

        internal PlayerCombatReplayDescription(AbstractPlayer player, ParsedLog log, CombatReplayMap map, CombatReplay replay) : base(player, log, map, replay, !log.FriendlyAgents.Contains(player.AgentItem) ? "TargetPlayer" : "Player")
        {
            Group = player.Group;
            SetStatus(log, player);
        }
    }
}
