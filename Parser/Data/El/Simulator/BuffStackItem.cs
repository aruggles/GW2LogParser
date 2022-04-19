using Gw2LogParser.Parser.Data.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.Simulator
{
    internal class BuffStackItem
    {
        public long Start { get; private set; }
        public long Duration { get; private set; }
        public Agent Src { get; private set; }
        public Agent SeedSrc { get; }
        public bool IsExtension { get; private set; }

        public long TotalDuration
        {
            get
            {
                long res = Duration;
                foreach ((Agent src, long value) in Extensions)
                {
                    res += value;
                }
                return res;
            }
        }

        public List<(Agent src, long value)> Extensions { get; } = new List<(Agent src, long value)>();

        public BuffStackItem(long start, long boonDuration, Agent src, Agent seedSrc, bool isExtension)
        {
            Start = start;
            SeedSrc = seedSrc;
            Duration = boonDuration;
            Src = src;
            IsExtension = isExtension;
        }

        public BuffStackItem(long start, long boonDuration, Agent src)
        {
            Start = start;
            SeedSrc = src;
            Duration = boonDuration;
            Src = src;
            IsExtension = false;
        }

        public virtual void Shift(long startShift, long durationShift)
        {
            Start += startShift;
            Duration -= durationShift;
            if (Duration == 0 && Extensions.Any())
            {
                (Agent src, long value) = Extensions.First();
                Extensions.RemoveAt(0);
                Src = src;
                Duration = value;
                IsExtension = true;
            }
        }

        public void Extend(long value, Agent src)
        {
            Extensions.Add((src, value));
        }
    }
}
