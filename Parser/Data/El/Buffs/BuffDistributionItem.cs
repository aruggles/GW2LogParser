using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.Buffs
{
    public class BuffDistributionItem
    {
        public long Value { get; set; }
        public long Overstack { get; set; }
        public long Waste { get; set; }
        public long UnknownExtension { get; set; }
        public long Extension { get; set; }
        public long Extended { get; set; }

        public BuffDistributionItem(long value, long overstack, long waste, long unknownExtension, long extension, long extended)
        {
            Value = value;
            Overstack = overstack;
            Waste = waste;
            UnknownExtension = unknownExtension;
            Extension = extension;
            Extended = extended;
        }
    }

    public class BuffDistribution : Dictionary<long, Dictionary<Agent, BuffDistributionItem>>
    {
        public bool HasSrc(long boonid, Agent src)
        {
            return ContainsKey(boonid) && this[boonid].ContainsKey(src);
        }

        public List<AbstractSingleActor> GetSrcs(long boonid, ParsedLog log)
        {
            if (!ContainsKey(boonid))
            {
                return new List<AbstractSingleActor>();
            }
            var actors = new List<AbstractSingleActor>();
            foreach (Agent agent in this[boonid].Keys)
            {
                actors.Add(log.FindActor(agent, true));
            }
            return actors;
        }

        public long GetUptime(long boonid)
        {
            if (!ContainsKey(boonid))
            {
                return 0;
            }
            return this[boonid].Sum(x => x.Value.Value);
        }

        public long GetGeneration(long boonid, Agent src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Value;
        }

        public long GetOverstack(long boonid, Agent src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Overstack;
        }

        public long GetWaste(long boonid, Agent src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Waste;
        }

        public long GetUnknownExtension(long boonid, Agent src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].UnknownExtension;
        }

        public long GetExtension(long boonid, Agent src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Extension;
        }

        public long GetExtended(long boonid, Agent src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Extended;
        }
    }
}
