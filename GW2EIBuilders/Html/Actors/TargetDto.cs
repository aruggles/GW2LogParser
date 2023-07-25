﻿using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using Gw2LogParser.EvtcParserExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class TargetDto : ActorDto
    {
        public long HbWidth { get; set; }
        public long HbHeight { get; set; }
        public double Percent { get; set; }
        public double HpLeft { get; set; }

        public TargetDto(AbstractSingleActor target, ParsedLog log, ActorDetailsDto details) : base(target, log, details)
        {
            HbHeight = target.HitboxHeight;
            HbWidth = target.HitboxWidth;
            HpLeft = 100.0;
            if (log.FightData.Success)
            {
                HpLeft = 0;
            }
            else
            {
                IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(target.AgentItem);
                if (hpUpdates.Count > 0)
                {
                    HpLeft = hpUpdates.Last().HPPercent;
                }
            }
            Percent = Math.Round(100.0 - HpLeft, 2);
        }
    }
}
