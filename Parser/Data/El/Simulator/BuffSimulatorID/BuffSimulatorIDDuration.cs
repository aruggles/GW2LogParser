using System.Linq;
using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulatorID
{
    internal class BuffSimulatorIDDuration : BuffSimulatorID
    {
        private BuffStackItemID _activeStack;

        // Constructor
        public BuffSimulatorIDDuration(ParsedLog log, Buff buff) : base(log, buff)
        {
        }

        public override void Activate(uint stackID)
        {
            if (_activeStack != null)
            {
                _activeStack.Disable();
            }
            _activeStack = BuffStack.FirstOrDefault(x => x.StackID == stackID);
            if (_activeStack == null)
            {
                throw new EIBuffSimulatorIDException("Activate has failed");
            }
            _activeStack.Activate();
        }

        public override void Add(long duration, Agent src, long start, uint stackID, bool addedActive, uint overstackDuration)
        {
            var toAdd = new BuffStackItemID(start, duration, src, addedActive, stackID);
            BuffStack.Add(toAdd);
            if (addedActive)
            {
                if (_activeStack != null)
                {
                    _activeStack.Disable();
                }
                _activeStack = toAdd;
            }
        }

        protected override void Update(long timePassed)
        {
            if (BuffStack.Any() && timePassed > 0)
            {
                long diff = timePassed;
                long leftOver = 0;
                if (_activeStack != null && _activeStack.Duration > 0)
                {
                    var toAdd = new BuffSimulationItemDuration(BuffStack);
                    GenerationSimulation.Add(toAdd);
                    long timeDiff = _activeStack.Duration - timePassed;
                    if (timeDiff < 0)
                    {
                        diff = _activeStack.Duration;
                        leftOver = timePassed - diff;
                    }
                    if (toAdd.End > toAdd.Start + diff)
                    {
                        toAdd.OverrideEnd(toAdd.Start + diff);
                    }
                    _activeStack.Shift(0, diff);
                    // keep current stack alive while waiting for stack active/ stack remove to arrive
                    if (_activeStack.Duration == 0 && leftOver > 0 && leftOver < ParserHelper.BuffSimulatorStackActiveDelayConstant)
                    {
                        _activeStack.Shift(0, -leftOver);
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
