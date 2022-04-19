using Gw2LogParser.Parser.Data;
using System;

namespace Gw2LogParser.Parser.Helper.CachingCollections
{
    public abstract class AbstractCachingCollection<T>
    {
        private readonly long _start;
        private readonly long _end;

        protected AbstractCachingCollection(ParsedLog log)
        {
            _start = log.FightData.LogStart;
            _end = log.FightData.LogEnd;
        }

        protected (long, long) SanitizeTimes(long start, long end)
        {
            long newStart = Math.Max(start, _start);
            long newEnd = Math.Max(newStart, Math.Min(end, _end));
            return (newStart, newEnd);
        }

        public abstract void Clear();
    }
}
