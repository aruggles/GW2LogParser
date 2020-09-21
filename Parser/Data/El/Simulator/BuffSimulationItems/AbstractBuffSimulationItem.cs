using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems
{
    public abstract class AbstractBuffSimulationItem
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

        public abstract void SetBoonDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedLog log);
    }
}
