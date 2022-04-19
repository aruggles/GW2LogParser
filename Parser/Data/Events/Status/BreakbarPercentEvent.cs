using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Interfaces;
using System;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class BreakbarPercentEvent : AbstractStatusEvent, IStateable
    {
        public double BreakbarPercent { get; }

        internal BreakbarPercentEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            byte[] bytes = new byte[sizeof(float)];
            int offset = 0;
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Value))
            {
                bytes[offset++] = bt;
            }
            BreakbarPercent = Math.Round(100.0 * BitConverter.ToSingle(bytes, 0), 2);
            if (BreakbarPercent > 100.0)
            {
                BreakbarPercent = 100;
            }
        }

        public (long start, double value) ToState()
        {
            return (Time, BreakbarPercent);
        }
    }
}
