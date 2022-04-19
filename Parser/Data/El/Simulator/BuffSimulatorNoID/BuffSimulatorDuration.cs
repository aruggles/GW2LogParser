﻿using System.Linq;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID
{
    internal class BuffSimulatorDuration : BuffSimulator
    {
        private (Agent agent, bool extension) _lastSrcRemove = (ParserHelper._unknownAgent, false);
        // Constructor
        public BuffSimulatorDuration(ParsedLog log, Buff buff, int capacity) : base(log, buff, capacity)
        {
        }

        public override void Extend(long extension, long oldValue, Agent src, long start, uint stackID)
        {
            if ((BuffStack.Any() && oldValue > 0) || IsFull)
            {
                BuffStack[0].Extend(extension, src);
            }
            else
            {
                Add(oldValue + extension, src, _lastSrcRemove.agent, start, true, _lastSrcRemove.extension);
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BuffStack.Any() && timePassed > 0)
            {
                BuffStackItem activeStack = BuffStack[0];
                _lastSrcRemove = (ParserHelper._unknownAgent, false);
                var toAdd = new BuffSimulationItemDuration(BuffStack);
                GenerationSimulation.Add(toAdd);
                long timeDiff = activeStack.Duration - timePassed;
                long diff = timePassed;
                long leftOver = 0;
                if (timeDiff < 0)
                {
                    diff = activeStack.Duration;
                    leftOver = timePassed - diff;
                }
                if (toAdd.End > toAdd.Start + diff)
                {
                    toAdd.OverrideEnd(toAdd.Start + diff);
                }
                activeStack.Shift(0, diff);
                foreach (BuffStackItem buffStackItem in BuffStack)
                {
                    buffStackItem.Shift(diff, 0);
                }
                if (activeStack.Duration == 0)
                {
                    _lastSrcRemove = (activeStack.SeedSrc, activeStack.IsExtension);
                    BuffStack.RemoveAt(0);
                }
                Update(leftOver);
            }
        }
    }
}
