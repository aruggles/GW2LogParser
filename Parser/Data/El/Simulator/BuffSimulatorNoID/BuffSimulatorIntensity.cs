﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID
{
    internal class BuffSimulatorIntensity : BuffSimulator
    {
        private readonly List<(Agent agent, bool extension)> _lastSrcRemoves = new List<(Agent agent, bool extension)>();
        // Constructor
        public BuffSimulatorIntensity(ParsedLog log, Buff buff, int capacity) : base(log, buff, capacity)
        {
        }

        public override void Extend(long extension, long oldValue, Agent src, long start, uint stackID)
        {
            if ((BuffStack.Any() && oldValue > 0) || IsFull)
            {
                BuffStackItem minItem = BuffStack.MinBy(x => Math.Abs(x.TotalDuration - oldValue));
                if (minItem != null)
                {
                    minItem.Extend(extension, src);
                }
            }
            else
            {
                if (_lastSrcRemoves.Any())
                {
                    Add(oldValue + extension, src, _lastSrcRemoves.First().agent, start, false, _lastSrcRemoves.First().extension);
                    _lastSrcRemoves.RemoveAt(0);
                }
                else
                {
                    Add(oldValue + extension, src, start, 0, true, 0);
                }
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BuffStack.Any() && timePassed > 0)
            {
                _lastSrcRemoves.Clear();
                var toAdd = new BuffSimulationItemIntensity(BuffStack);
                GenerationSimulation.Add(toAdd);
                long diff = Math.Min(BuffStack.Min(x => x.Duration), timePassed);
                long leftOver = timePassed - diff;
                if (toAdd.End > toAdd.Start + diff)
                {
                    toAdd.OverrideEnd(toAdd.Start + diff);
                }
                // Subtract from each
                foreach (BuffStackItem buffStackItem in BuffStack)
                {
                    buffStackItem.Shift(diff, diff);
                    if (buffStackItem.Duration == 0)
                    {
                        _lastSrcRemoves.Add((buffStackItem.SeedSrc, buffStackItem.IsExtension));
                    }
                }
                BuffStack.RemoveAll(x => x.Duration == 0);
                Update(leftOver);
            }
        }
    }
}
