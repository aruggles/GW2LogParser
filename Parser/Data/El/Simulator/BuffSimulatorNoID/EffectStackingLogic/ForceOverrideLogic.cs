using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID.EffectStackingLogic
{
    internal class ForceOverrideLogic : StackingLogic
    {
        protected override void Sort(ParsedLog log, List<BuffStackItem> stacks)
        {
            // no sort
        }

        public override bool FindLowestValue(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes)
        {
            if (!stacks.Any())
            {
                return false;
            }
            BuffStackItem stack = stacks[0];
            wastes.Add(new BuffSimulationItemWasted(stack.Src, stack.Duration, stack.Start));
            if (stack.Extensions.Count > 0)
            {
                foreach ((Agent src, long value) in stack.Extensions)
                {
                    wastes.Add(new BuffSimulationItemWasted(src, value, stack.Start));
                }
            }
            stacks[0] = stackItem;
            return true;
        }

        public override bool IsFull(List<BuffStackItem> stacks, int capacity)
        {
            return stacks.Count == 1;
        }
    }
}
