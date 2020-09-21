using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.Buffs.BuffSourceFinders
{
    internal class BuffSourceFinder01102019 : BuffSourceFinder05032019
    {
        public BuffSourceFinder01102019(HashSet<long> boonIds) : base(boonIds)
        {
            ImbuedMelodies = -1;
        }
    }
}
