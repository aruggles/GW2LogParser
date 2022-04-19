using Gw2LogParser.Parser.Data.Agents;
using System;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class HealthUpdateEvent : AbstractStatusEvent
    {
        public double HPPercent { get; }

        internal HealthUpdateEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            HPPercent = Math.Round(evtcItem.DstAgent / 100.0, 2);
            if (HPPercent > 100.0)
            {
                HPPercent = 100;
            }
        }

        public (long start, double value) ToState()
        {
            return (Time, HPPercent);
        }

    }
}
