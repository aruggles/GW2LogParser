using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events
{
    public class RewardEvent : AbstractTimeCombatEvent
    {
        public ulong RewardID { get; }
        public int RewardType { get; }

        internal RewardEvent(Combat evtcItem) : base(evtcItem.Time)
        {
            RewardID = evtcItem.DstAgent;
            RewardType = evtcItem.Value;
        }
    }
}
