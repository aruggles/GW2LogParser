using Gw2LogParser.Parser.Data.Agents;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Buffs.BuffSourceFinders
{
    internal class BuffSourceFinder20191001 : BuffSourceFinder20190305
    {
        public BuffSourceFinder20191001(HashSet<long> boonIds) : base(boonIds)
        {
            ImbuedMelodies = -1;
        }
        protected override bool CouldBeImbuedMelodies(Agent agent, long time, long extension, ParsedLog log)
        {
            return false;
        }
    }
}
