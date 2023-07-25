﻿using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcEffectMechanic : SrcEffectMechanic
    {

        public PlayerSrcEffectMechanic(string effectGUID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this(new string[] { effectGUID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerSrcEffectMechanic(string[] effectGUIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(effectGUIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            PlayerChecker(log, mechanicLogs);
        }
    }
}
