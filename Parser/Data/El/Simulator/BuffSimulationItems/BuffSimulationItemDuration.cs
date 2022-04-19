using Gw2LogParser.Parser.Data.El.Buffs;
using System.Linq;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems
{
    internal class BuffSimulationItemDuration : BuffSimulationItemStack
    {
        public BuffSimulationItemDuration(IEnumerable<BuffStackItem> stacks) : base(stacks)
        {
        }

        public override void OverrideEnd(long end)
        {
            Stacks.First().OverrideEnd(end);
            Duration = Stacks.First().Duration;
        }

        public override int GetActiveStacks()
        {
            return 1;
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid)
        {
            Stacks.First().SetBuffDistributionItem(distribs, start, end, boonid);
        }
    }
}
