using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class PlayerDamageChartDto<T>
    {
        public List<IReadOnlyList<T>> Targets { get; set; }
        public IReadOnlyList<T> Total { get; set; }
    }
}
