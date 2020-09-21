﻿using Gw2LogParser.Parser.Data.Events.Cast;

namespace Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes
{
    internal abstract class CastMechanic : Mechanic
    {
        public delegate bool CastChecker(AbstractCastEvent ce, ParsedLog log);

        private readonly CastChecker _triggerCondition = null;

        protected bool Keep(AbstractCastEvent c, ParsedLog log)
        {
            if (_triggerCondition != null)
            {
                return _triggerCondition(c, log);
            }
            return true;
        }

        protected virtual long GetTime(AbstractCastEvent evt)
        {
            return evt.Time;
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CastChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CastChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}
