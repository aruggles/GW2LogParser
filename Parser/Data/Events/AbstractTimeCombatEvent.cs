using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events
{
    public abstract class AbstractTimeCombatEvent
    {
        public long Time { get; protected set; }

        protected AbstractTimeCombatEvent(long time)
        {
            Time = time;
        }

        internal void OverrideTime(long time)
        {
            Time = time;
        }
    }
}
