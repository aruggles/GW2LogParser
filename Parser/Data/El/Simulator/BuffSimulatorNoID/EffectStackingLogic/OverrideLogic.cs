﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID.EffectStackingLogic
{
    internal class OverrideLogic : StackingLogic
    {
        // Prefer using Add, to insert to the list in a sorted manner
        protected override void Sort(ParsedLog log, List<BuffStackItem> stacks)
        {
            throw new InvalidOperationException("Do not use Sort for OverrideLogic");
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
            stacks.RemoveAt(0);
            Add(log, stacks, stackItem);
            return true;
        }

        // https://www.c-sharpcorner.com/blogs/binary-search-implementation-using-c-sharp1
        private static int BinarySearchRecursive(List<BuffStackItem> stacks, long totalDuration, int minIndex, int maxIndex)
        {
            if (stacks[minIndex].TotalDuration > totalDuration)
            {
                return minIndex;
            }
            if (stacks[maxIndex].TotalDuration < totalDuration)
            {
                return maxIndex + 1;
            }
            if (minIndex > maxIndex)
            {
                return minIndex;
            }
            else
            {
                int midIndex = (minIndex + maxIndex) / 2;
                if (totalDuration == stacks[midIndex].TotalDuration)
                {
                    return ++midIndex;
                }
                else if (totalDuration < stacks[midIndex].TotalDuration)
                {
                    return BinarySearchRecursive(stacks, totalDuration, minIndex, midIndex - 1);
                }
                else
                {
                    return BinarySearchRecursive(stacks, totalDuration, midIndex + 1, maxIndex);
                }
            }
        }

        public override void Add(ParsedLog log, List<BuffStackItem> stacks, BuffStackItem stackItem)
        {
            if (!stacks.Any())
            {
                stacks.Add(stackItem);
                return;
            }
            int insertIndex = BinarySearchRecursive(stacks, stackItem.TotalDuration, 0, stacks.Count - 1);
            if (insertIndex > stacks.Count - 1)
            {
                stacks.Add(stackItem);
            }
            else
            {
                stacks.Insert(insertIndex, stackItem);
            }
        }
    }
}
