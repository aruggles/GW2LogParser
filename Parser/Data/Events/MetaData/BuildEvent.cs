using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class BuildEvent : AbstractMetaDataEvent
    {
        public ulong Build { get; }

        public BuildEvent(Combat evtcItem) : base(evtcItem)
        {
            Build = evtcItem.SrcAgent;
        }

    }
}
