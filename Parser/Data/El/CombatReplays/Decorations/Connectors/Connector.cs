using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors
{
    internal abstract class Connector
    {
        public abstract object GetConnectedTo(CombatReplayMap map, ParsedLog log);
    }
}
