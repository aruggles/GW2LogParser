using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;
using System;
using System.Linq;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Actors
{
    public class AbstractPlayer : AbstractSingleActor
    {
        // Constructors
        internal AbstractPlayer(Agent agent) : base(agent)
        {
            if (agent.IsNPC)
            {
                throw new EvtcAgentException("Agent is NPC");
            }
            if (IsFakeActor)
            {
                throw new EvtcAgentException("Players can't be fake actors");
            }
        }
        internal override void OverrideName(string name)
        {
            throw new InvalidOperationException("Players' name can't be overriden");
        }
        internal override void SetManualHealth(int health)
        {
            throw new InvalidOperationException("Players' health can't be overriden");
        }

        public override string GetIcon()
        {
            return AgentItem.Type == Agent.AgentType.NonSquadPlayer && !AgentItem.IsNotInSquadFriendlyPlayer ? GetHighResolutionProfIcon(Spec) : GetProfIcon(Spec);
        }

        protected override void InitAdditionalCombatReplayData(ParsedLog log)
        {
            // Fight related stuff
            log.FightData.Logic.ComputePlayerCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any())
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
            return new PlayerCombatReplayDescription(this, log, map, CombatReplay);
        }
    }
}
