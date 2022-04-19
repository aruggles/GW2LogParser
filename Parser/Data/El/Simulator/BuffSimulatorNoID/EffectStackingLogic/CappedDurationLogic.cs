using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID.EffectStackingLogic
{
    internal class CappedDurationLogic : StackingLogic
    {
        protected override void Sort(ParsedLog log, List<BuffStackItem> stacks)
        {
            // nothing to sort
        }

        public override bool IsFull(List<BuffStackItem> stacks, int capacity)
        {
            // never full
            return false;
        }

        public override bool FindLowestValue(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes)
        {
            // no lowest value to find
            return false;
        }
    }
}
