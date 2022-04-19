using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID.EffectStackingLogic
{
    internal class QueueLogic : StackingLogic
    {
        protected override void Sort(ParsedLog log, List<BuffStackItem> stacks)
        {
            // nothign to sort
        }

        public override bool FindLowestValue(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes)
        {
            if (stacks.Count <= 1)
            {
                throw new InvalidDataException("Queue logic based must have a >1 capacity");
            }
            BuffStackItem first = stacks[0];
            BuffStackItem minItem = stacks.Where(x => x != first).MinBy(x => x.TotalDuration);
            wastes.Add(new BuffSimulationItemWasted(minItem.Src, minItem.Duration, minItem.Start));
            if (minItem.Extensions.Any())
            {
                foreach ((Agent src, long value) in minItem.Extensions)
                {
                    wastes.Add(new BuffSimulationItemWasted(src, value, minItem.Start));
                }
            }
            stacks[stacks.IndexOf(minItem)] = stackItem;
            Sort(log, stacks);
            return true;
        }
    }
}
