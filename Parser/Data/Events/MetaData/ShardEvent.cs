using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class ShardEvent : AbstractMetaDataEvent
    {
        public int ShardID { get; }

        public ShardEvent(Combat evtcItem) : base(evtcItem)
        {
            ShardID = (int)evtcItem.SrcAgent;
        }

    }
}
