using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Simulator
{
    internal abstract class AbstractSimulationItem
    {
        protected static Dictionary<Agent, BuffDistributionItem> GetDistrib(BuffDistribution distribs, long boonid)
        {
            if (!distribs.TryGetValue(boonid, out Dictionary<Agent, BuffDistributionItem> distrib))
            {
                distrib = new Dictionary<Agent, BuffDistributionItem>();
                distribs.Add(boonid, distrib);
            }
            return distrib;
        }

        public abstract void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedLog log);
    }
}
