﻿using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Mechanics;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes
{
    internal class EnemyBuffApplyMechanic : BuffApplyMechanic
    {
        public EnemyBuffApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, BuffApplyChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public EnemyBuffApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffApplyChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
            IsEnemyMechanic = true;
        }

        public EnemyBuffApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public EnemyBuffApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        internal override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(SkillId))
            {
                AbstractSingleActor amp = null;
                if (c is BuffApplyEvent ba && Keep(ba, log))
                {
                    if (!regroupedMobs.TryGetValue(ba.To.ID, out amp))
                    {
                        amp = log.FindActor(ba.To, false);
                        if (amp == null)
                        {
                            continue;
                        }
                        regroupedMobs.Add(amp.ID, amp);
                    }
                }
                if (amp != null)
                {
                    mechanicLogs[this].Add(new MechanicEvent(c.Time, this, amp));
                }
            }
        }
    }
}
