using Gw2LogParser.Parser.Data.El.Buffs;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.DamageModifiers.BuffTrackers
{
    internal abstract class BuffsTracker
    {
        public abstract int GetStack(Dictionary<long, BuffsGraphModel> bgms, long time);
        public abstract bool Has(Dictionary<long, BuffsGraphModel> bgms);
    }
}
