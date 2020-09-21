using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Simulator.AbstractBuffSimulator;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID.EffectStackingLogic
{
    internal abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes);

        public abstract void Sort(ParsedLog log, List<BuffStackItem> stacks);
    }
}
