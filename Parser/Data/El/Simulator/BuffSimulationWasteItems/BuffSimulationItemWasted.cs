using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems
{
    internal class BuffSimulationItemWasted : AbstractBuffSimulationItemWasted
    {

        public BuffSimulationItemWasted(Agent src, long waste, long time) : base(src, waste, time)
        {
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
        {
            Dictionary<Agent, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
            Agent agent = Src;
            long value = GetValue(start, end);
            if (value == 0)
            {
                return;
            }
            if (distrib.TryGetValue(agent, out BuffDistributionItem toModify))
            {
                toModify.IncrementWaste(value);
            }
            else
            {
                distrib.Add(agent, new BuffDistributionItem(
                    0,
                    0, value, 0, 0, 0));
            }
        }
    }
}
