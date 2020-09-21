using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems
{
    internal abstract class AbstractBuffSimulationItemWasted : AbstractSimulationItem
    {
        protected Agent Src { get; }
        private readonly long _waste;
        protected long Time { get; }
        protected AbstractBuffSimulationItemWasted(Agent src, long waste, long time)
        {
            Src = src;
            _waste = waste;
            Time = time;
        }

        protected long GetValue(long start, long end)
        {
            return (start <= Time && Time <= end) ? _waste : 0;
        }
    }
}
