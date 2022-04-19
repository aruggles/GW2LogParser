using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Interfaces;
using System;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class BarrierUpdateEvent : AbstractStatusEvent, IStateable
    {
        public double BarrierPercent { get; }

        internal BarrierUpdateEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            BarrierPercent = Math.Round(evtcItem.DstAgent / 100.0, 2);
            if (BarrierPercent > 100.0)
            {
                BarrierPercent = 0;
            }
        }

        public (long start, double value) ToState()
        {
            return (Time, BarrierPercent);
        }
    }
}
