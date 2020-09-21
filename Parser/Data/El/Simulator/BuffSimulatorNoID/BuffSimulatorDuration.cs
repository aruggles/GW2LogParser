﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using Gw2LogParser.Parser.Data.El.Simulator.EffectStackingLogic;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID
{
    internal class BuffSimulatorDuration : BuffSimulator
    {
        private (Agent agent, bool extension) _lastSrcRemove = (ParserHelper._unknownAgent, false);
        // Constructor
        public BuffSimulatorDuration(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, Agent src, long start, uint stackID)
        {
            if ((BuffStack.Count > 0 && oldValue > 0) || BuffStack.Count == Capacity)
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
            if (BuffStack.Count > 0 && timePassed > 0)
            {
                _lastSrcRemove = (ParserHelper._unknownAgent, false);
                var toAdd = new BuffSimulationItemDuration(BuffStack[0]);
                GenerationSimulation.Add(toAdd);
                long timeDiff = BuffStack[0].Duration - timePassed;
                long diff;
                long leftOver = 0;
                if (timeDiff < 0)
                {
                    diff = BuffStack[0].Duration;
                    leftOver = timePassed - diff;
                }
                else
                {
                    diff = timePassed;
                }
                if (toAdd.End > toAdd.Start + diff)
                {
                    toAdd.OverrideEnd(toAdd.Start + diff);
                }
                BuffStack[0].Shift(diff, diff);
                for (int i = 1; i < BuffStack.Count; i++)
                {
                    BuffStack[i].Shift(diff, 0);
                }
                if (BuffStack[0].Duration == 0)
                {
                    _lastSrcRemove = (BuffStack[0].SeedSrc, BuffStack[0].IsExtension);
                    BuffStack.RemoveAt(0);
                }
                Update(leftOver);
            }
        }
    }
}
