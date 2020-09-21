﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using Gw2LogParser.Parser.Data.El.Simulator.EffectStackingLogic;
using Gw2LogParser.Parser.Helper;
using System;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID
{
    internal abstract class BuffSimulator : AbstractBuffSimulator
    {
        protected int Capacity { get; }
        private readonly StackingLogic _logic;
        protected long ID { get; set; } = 0;

        // Constructor
        protected BuffSimulator(int capacity, ParsedLog log, StackingLogic logic) : base(log)
        {
            Capacity = Math.Max(capacity, 1);
            _logic = logic;
        }

        public override void Add(long duration, Agent src, long start, uint id, bool addedActive, uint overstackDuration)
        {
            var toAdd = new BuffStackItem(start, duration, src, ++ID);
            // Find empty slot
            if (BuffStack.Count < Capacity)
            {
                BuffStack.Add(toAdd);
                _logic.Sort(Log, BuffStack);
            }
            // Replace lowest value
            else
            {
                bool found = _logic.StackEffect(Log, toAdd, BuffStack, WasteSimulationResult);
                if (!found)
                {
                    OverstackSimulationResult.Add(new BuffSimulationItemOverstack(src, duration, start));
                }
            }
        }

        protected void Add(long duration, Agent src, Agent seedSrc, long time, bool atFirst, bool isExtension)
        {
            var toAdd = new BuffStackItem(time, duration, src, seedSrc, ++ID, isExtension);
            // Find empty slot
            if (BuffStack.Count < Capacity)
            {
                if (atFirst)
                {
                    BuffStack.Insert(0, toAdd);
                }
                else
                {

                    BuffStack.Add(toAdd);
                }
                _logic.Sort(Log, BuffStack);
            }
            // Replace lowest value
            else
            {
                bool found = _logic.StackEffect(Log, toAdd, BuffStack, WasteSimulationResult);
                if (!found)
                {
                    OverstackSimulationResult.Add(new BuffSimulationItemOverstack(src, duration, time));
                }
            }
        }

        public override void Remove(Agent by, long removedDuration, int removedStacks, long time, ArcDPSEnums.BuffRemove removeType, uint id)
        {
            switch (removeType)
            {
                case ArcDPSEnums.BuffRemove.All:
                    foreach (BuffStackItem stackItem in BuffStack)
                    {
                        WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                        if (stackItem.Extensions.Count > 0)
                        {
                            foreach ((Agent src, long value) in stackItem.Extensions)
                            {
                                WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                            }
                        }
                    }
                    BuffStack.Clear();
                    break;
                case ArcDPSEnums.BuffRemove.Single:
                    for (int i = 0; i < BuffStack.Count; i++)
                    {
                        BuffStackItem stackItem = BuffStack[i];
                        if (Math.Abs(removedDuration - stackItem.TotalBoonDuration()) < ParserHelper.ServerDelayConstant)
                        {
                            WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                            if (stackItem.Extensions.Count > 0)
                            {
                                foreach ((Agent src, long value) in stackItem.Extensions)
                                {
                                    WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                                }
                            }
                            BuffStack.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
            _logic.Sort(Log, BuffStack);
        }

        public override void Activate(uint id)
        {

        }
        public override void Reset(uint id, long toDuration)
        {

        }
    }
}
