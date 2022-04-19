using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorID
{
    internal class BuffSimulatorIDIntensity : BuffSimulatorID
    {
        private readonly int _capacity;
        // Constructor
        public BuffSimulatorIDIntensity(ParsedLog log, Buff buff, int capacity) : base(log, buff)
        {
            _capacity = capacity;
        }

        public override void Activate(uint stackID)
        {
            BuffStackItemID active = BuffStack.FirstOrDefault(x => x.StackID == stackID);
            if (active == null)
            {
                throw new EIBuffSimulatorIDException("Activate has failed");
            }
            active.Activate();
        }

        public override void Add(long duration, Agent src, long start, uint stackID, bool addedActive, uint overstackDuration)
        {
            var toAdd = new BuffStackItemID(start, duration, src, addedActive, stackID);
            BuffStack.Add(toAdd);
        }

        protected override void Update(long timePassed)
        {
            if (BuffStack.Any() && timePassed > 0)
            {
                long diff = timePassed;
                long leftOver = 0;
                var activeStacks = BuffStack.Where(x => x.Active && x.Duration > 0).Take(_capacity).ToList();
                if (activeStacks.Any())
                {
                    var toAdd = new BuffSimulationItemIntensity(activeStacks);
                    GenerationSimulation.Add(toAdd);
                    long currentDur = activeStacks.Min(x => x.Duration);
                    long timeDiff = currentDur - timePassed;
                    if (timeDiff < 0)
                    {
                        diff = currentDur;
                        leftOver = timePassed - diff;
                    }
                    if (toAdd.End > toAdd.Start + diff)
                    {
                        toAdd.OverrideEnd(toAdd.Start + diff);
                    }
                    foreach (BuffStackItemID buffStackItem in activeStacks)
                    {
                        buffStackItem.Shift(0, diff);
                    }
                }
                foreach (BuffStackItemID buffStackItem in BuffStack)
                {
                    buffStackItem.Shift(diff, 0);
                }
                Update(leftOver);
            }
        }
    }
}
