﻿using Gw2LogParser.Parser.Data.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events.Status
{
    public class DownEvent : AbstractStatusEvent
    {
        internal DownEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {

        }

        internal DownEvent(Agent src, long time) : base(src, time)
        {

        }

    }
}
