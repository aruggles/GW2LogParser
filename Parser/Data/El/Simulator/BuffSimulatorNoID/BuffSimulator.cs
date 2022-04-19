using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID.EffectStackingLogic;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static Gw2LogParser.Parser.Helper.ArcDPSEnums;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorNoID
{
    internal abstract class BuffSimulator : AbstractBuffSimulator
    {
        protected List<BuffStackItem> BuffStack { get; set; } = new List<BuffStackItem>();
        private StackingLogic _logic { get; }

        private readonly int _capacity;

        private static readonly QueueLogic _queueLogic = new QueueLogic();
        private static readonly HealingLogic _healingLogic = new HealingLogic();
        private static readonly ForceOverrideLogic _forceOverrideLogic = new ForceOverrideLogic();
        private static readonly OverrideLogic _overrideLogic = new OverrideLogic();
        private static readonly CappedDurationLogic _cappedDurationLogic = new CappedDurationLogic();

        // Constructor
        protected BuffSimulator(ParsedLog log, Buff buff, int capacity) : base(log, buff)
        {
            _capacity = capacity;
            switch (buff.StackType)
            {
                case BuffStackType.Queue:
                    _logic = _queueLogic;
                    break;
                case BuffStackType.CappedDuration:
                    _logic = _cappedDurationLogic;
                    break;
                case BuffStackType.Regeneration:
                    _logic = _healingLogic;
                    break;
                case BuffStackType.Force:
                    _logic = _forceOverrideLogic;
                    break;
                case BuffStackType.Stacking:
                case BuffStackType.StackingConditionalLoss:
                    _logic = _overrideLogic;
                    break;
                case BuffStackType.Unknown:
                default:
                    throw new InvalidDataException("Buffs can not be typless");
            }
        }

        protected bool IsFull => _logic.IsFull(BuffStack, _capacity);

        protected override void Clear()
        {
            BuffStack.Clear();
        }

        public override void Add(long duration, Agent src, long start, uint stackID, bool addedActive, uint overstackDuration)
        {
            var toAdd = new BuffStackItem(start, duration, src);
            // Find empty slot
            if (!IsFull)
            {
                _logic.Add(Log, BuffStack, toAdd);
            }
            // Replace lowest value
            else
            {
                if (!_logic.FindLowestValue(Log, toAdd, BuffStack, WasteSimulationResult))
                {
                    OverstackSimulationResult.Add(new BuffSimulationItemOverstack(src, duration, start));
                }
            }
        }

        protected void Add(long duration, Agent src, Agent seedSrc, long time, bool atFirst, bool isExtension)
        {
            var toAdd = new BuffStackItem(time, duration, src, seedSrc, isExtension);
            // Find empty slot
            if (!IsFull)
            {
                if (atFirst)
                {
                    BuffStack.Insert(0, toAdd);
                }
                else
                {
                    _logic.Add(Log, BuffStack, toAdd);
                }
            }
            // Replace lowest value
            else
            {
                if (!_logic.FindLowestValue(Log, toAdd, BuffStack, WasteSimulationResult))
                {
                    OverstackSimulationResult.Add(new BuffSimulationItemOverstack(src, duration, time));
                }
            }
        }

        public override void Remove(Agent by, long removedDuration, int removedStacks, long time, ArcDPSEnums.BuffRemove removeType, uint stackID)
        {
            switch (removeType)
            {
                case BuffRemove.All:
                    foreach (BuffStackItem stackItem in BuffStack)
                    {
                        WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                        if (stackItem.Extensions.Any())
                        {
                            foreach ((Agent src, long value) in stackItem.Extensions)
                            {
                                WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                            }
                        }
                    }
                    BuffStack.Clear();
                    break;
                case BuffRemove.Single:
                    for (int i = 0; i < BuffStack.Count; i++)
                    {
                        BuffStackItem stackItem = BuffStack[i];
                        if (Math.Abs(removedDuration - stackItem.TotalDuration) < ParserHelper.BuffSimulatorDelayConstant)
                        {
                            WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                            if (stackItem.Extensions.Any())
                            {
                                foreach ((Agent src, long value) in stackItem.Extensions)
                                {
                                    WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                                }
                            }
                            BuffStack.RemoveAt(i);
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Activate(uint id)
        {

        }
        public override void Reset(uint id, long toDuration)
        {

        }
    }
}
