using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Simulator
{
    internal abstract class AbstractSimulationItem
    {
        public abstract void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid);
    }
}
