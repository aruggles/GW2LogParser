using Gw2LogParser.Parser.Data.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.Actors.ActorsHelper
{
    internal abstract class AbstractSingleActorHelper
    {
        protected AbstractSingleActor Actor { get; }

        protected Agent AgentItem => Actor.AgentItem;

        public AbstractSingleActorHelper(AbstractSingleActor actor)
        {
            Actor = actor;
        }
    }
}
