using Gw2LogParser.Parser.Data.Agents;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems
{
    internal abstract class BuffSimulationItemStack : BuffSimulationItem
    {
        protected readonly List<BuffSimulationItemBase> Stacks = new List<BuffSimulationItemBase>();
        private readonly List<Agent> _sources;

        public BuffSimulationItemStack(IEnumerable<BuffStackItem> stacks) : base(stacks.First().Start, stacks.First().Duration)
        {
            foreach (BuffStackItem stack in stacks)
            {
                Stacks.Add(new BuffSimulationItemBase(stack));
            }
            _sources = new List<Agent>();
            foreach (BuffSimulationItemBase item in Stacks)
            {
                _sources.AddRange(item.GetSources());
            }
        }
        public override int GetStacks()
        {
            return Stacks.Count;
        }

        public override IReadOnlyList<long> GetActualDurationPerStack()
        {
            return new List<long>(Stacks.Select(x => x.GetActualDurationPerStack().First()));
        }

        public override List<Agent> GetSources()
        {
            return _sources;
        }
    }
}
