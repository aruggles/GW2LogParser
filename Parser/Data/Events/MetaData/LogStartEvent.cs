using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class LogStartEvent : LogDateEvent
    {
        public LogStartEvent(Combat evtcItem) : base(evtcItem)
        {
        }

    }
}
