using Gw2LogParser.Parser.Data.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class HealthUpdateEvent : AbstractStatusEvent
    {
        public double HPPercent { get; }

        internal HealthUpdateEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            HPPercent = evtcItem.DstAgent / 100.0;
        }

        public (long start, double value) ToState()
        {
            return (Time, HPPercent);
        }

    }
}
