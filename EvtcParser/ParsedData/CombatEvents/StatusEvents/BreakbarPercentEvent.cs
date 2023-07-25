﻿using System;
using GW2EIEvtcParser.Interfaces;

namespace GW2EIEvtcParser.ParsedData
{
    public class BreakbarPercentEvent : AbstractStatusEvent, IStateable
    {
        public double BreakbarPercent { get; }

        internal BreakbarPercentEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
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
