using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class LogEndEvent : LogDateEvent
    {
        public LogEndEvent(Combat evtcItem) : base(evtcItem)
        {
        }

    }
}
