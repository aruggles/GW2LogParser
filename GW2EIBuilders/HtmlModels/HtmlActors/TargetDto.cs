using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Status;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.GW2EIBuilders
{
    public class TargetDto : ActorDto
    {
        public string Icon { get; internal set; }
        public long Health { get; internal set; }
        public long HbWidth { get; internal set; }
        public long HbHeight { get; internal set; }
        public double Percent { get; internal set; }
        public double HpLeft { get; internal set; }

        internal TargetDto(NPC target, ParsedLog log, bool cr, ActorDetailsDto details) : base(target, log, cr, details)
        {
            Icon = target.GetIcon();
            Health = target.GetHealth(log.CombatData);
            HbHeight = target.HitboxHeight;
            HbWidth = target.HitboxWidth;
            if (log.FightData.Success)
            {
                HpLeft = 0;
            }
            else
            {
                List<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(target.AgentItem);
                if (hpUpdates.Count > 0)
                {
                    HpLeft = hpUpdates.Last().HPPercent;
                }
            }
            Percent = Math.Round(100.0 - HpLeft, 2);
        }
    }
}
