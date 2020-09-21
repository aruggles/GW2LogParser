using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Simulator.AbstractBuffSimulator;

namespace Gw2LogParser.Parser.Data.El.Simulator.EffectStackingLogic
{
    internal class HealingLogic : QueueLogic
    {

        private struct CompareHealing
        {

            private static uint GetHealing(BuffStackItem stack)
            {
                return stack.SeedSrc.Healing;
            }

            public static int Compare(BuffStackItem x, BuffStackItem y)
            {
                return -GetHealing(x).CompareTo(GetHealing(y));
            }
        }

        public override void Sort(ParsedLog log, List<BuffStackItem> stacks)
        {
            stacks.Sort(CompareHealing.Compare);
        }
    }
}
