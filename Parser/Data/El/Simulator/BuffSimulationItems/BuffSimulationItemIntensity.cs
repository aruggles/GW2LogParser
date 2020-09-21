using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Data.El.Simulator.AbstractBuffSimulator;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems
{
    internal class BuffSimulationItemIntensity : BuffSimulationItem
    {
        private readonly List<BuffSimulationItemDuration> _stacks = new List<BuffSimulationItemDuration>();
        private readonly List<Agent> _sources;

        public BuffSimulationItemIntensity(List<BuffStackItem> stacks) : base(stacks[0].Start, 0)
        {
            foreach (BuffStackItem stack in stacks)
            {
                _stacks.Add(new BuffSimulationItemDuration(stack));
            }
            Duration = _stacks.Max(x => x.Duration);
            _sources = new List<Agent>();
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                _sources.AddRange(item.GetSources());
            }
        }

        public override void OverrideEnd(long end)
        {
            foreach (BuffSimulationItemDuration stack in _stacks)
            {
                stack.OverrideEnd(end);
            }
            Duration = _stacks.Max(x => x.Duration);
        }

        public override int GetStack()
        {
            return _stacks.Count;
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            long cDur = GetClampedDuration(start, end);
            if (cDur == 0)
            {
                return;
            }
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                item.SetBuffDistributionItem(distribs, start, end, boonid, log);
            }
        }

        public override List<Agent> GetSources()
        {
            return _sources;
        }
    }
}
