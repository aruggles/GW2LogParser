﻿using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcPlayerDstBuffApplyMechanic : PlayerSrcBuffApplyMechanic
    {

        public PlayerSrcPlayerDstBuffApplyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerSrcPlayerDstBuffApplyMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        protected override void AddMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, BuffApplyEvent ba, AbstractSingleActor actor)
        {
            mechanicLogs[this].Add(new MechanicEvent(ba.Time, this, actor));
            mechanicLogs[this].Add(new MechanicEvent(ba.Time, this, log.PlayerList.FirstOrDefault(x => x.AgentItem == ba.CreditedBy)));
        }
    }
}
