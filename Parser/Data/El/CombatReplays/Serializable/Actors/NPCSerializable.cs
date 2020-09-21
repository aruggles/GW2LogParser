using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using System.Linq;

namespace Gw2LogParser.Parser.Data
{
    public class NPCSerializable : AbstractSingleActorSerializable
    {
        public long Start { get; }
        public long End { get; }

        internal NPCSerializable(NPC npc, ParsedLog log, CombatReplayMap map, CombatReplay replay) : base(npc, log, map, replay, log.FightData.Logic.Targets.Contains(npc) ? "Target" : "Mob")
        {
            Start = replay.TimeOffsets.start;
            End = replay.TimeOffsets.end;
        }
    }
}
