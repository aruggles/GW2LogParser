using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID.EffectStackingLogic
{
    internal abstract class StackingLogic
    {
        public abstract bool FindLowestValue(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes);

        public virtual bool IsFull(List<BuffStackItem> stacks, int capacity)
        {
            return stacks.Count == capacity;
        }

        protected abstract void Sort(ParsedLog log, List<BuffStackItem> stacks);
        public virtual void Add(ParsedLog log, List<BuffStackItem> stacks, BuffStackItem stackItem)
        {
            stacks.Add(stackItem);
            Sort(log, stacks);
        }
    }
}
