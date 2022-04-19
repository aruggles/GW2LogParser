using Gw2LogParser.Parser.Data.El.Actors;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class NPCCombatReplayDescription : AbstractSingleActorCombatReplayDescription
    {
        internal NPCCombatReplayDescription(NPC npc, ParsedLog log, CombatReplayMap map, CombatReplay replay) : base(npc, log, map, replay, log.FightData.Logic.TargetAgents.Contains(npc.AgentItem) ? "Target" : log.FriendlyAgents.Contains(npc.AgentItem) ? "Friendly" : "Mob")
        {

            if (log.FriendlyAgents.Contains(npc.AgentItem))
            {
                SetStatus(log, npc);
            }
        }
    }
}
