using Gw2LogParser.Parser.Data.El.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors
{
    internal class AgentConnector : Connector
    {
        private readonly AbstractSingleActor _agent;

        public AgentConnector(AbstractSingleActor agent)
        {
            _agent = agent;
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedLog log)
        {
            return _agent.CombatReplayID;
        }
    }
}
