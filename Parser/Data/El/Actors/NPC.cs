using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;
using Gw2LogParser.Parser.Helper;

using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Actors
{
    public class NPC : AbstractSingleActor
    {
        internal NPC(Agent agent) : base(agent)
        {
            if (agent.IsPlayer)
            {
                throw new EvtcAgentException("Agent is a player");
            }
        }

        internal override void OverrideName(string name)
        {
            Character = name;
        }
        internal override void SetManualHealth(int health)
        {
            Health = health;
        }

        public override string GetIcon()
        {
            return ParserHelper.GetNPCIcon(ID);
        }

        protected override void InitAdditionalCombatReplayData(ParsedLog log)
        {
            log.FightData.Logic.ComputeNPCCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any() && (log.FightData.Logic.TargetAgents.Contains(AgentItem) || log.FriendlyAgents.Contains(AgentItem)))
            {
                CombatReplay.Decorations.Add(new FacingDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }


        //

        public override AbstractSingleActorCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return new NPCCombatReplayDescription(this, log, map, CombatReplay);
        }
        protected override void TrimCombatReplay(ParsedLog log)
        {
            if (!log.FriendlyAgents.Contains(AgentItem))
            {
                TrimCombatReplay(log, CombatReplay, AgentItem);
            }
        }
    }
}
