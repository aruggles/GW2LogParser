﻿using Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves;

namespace Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes
{
    internal abstract class BuffRemoveMechanic : Mechanic
    {
        public delegate bool BuffRemoveChecker(BuffRemoveAllEvent rme, ParsedLog log);

        private readonly BuffRemoveChecker _triggerCondition = null;

        protected bool Keep(BuffRemoveAllEvent c, ParsedLog log)
        {
            if (_triggerCondition != null)
            {
                return _triggerCondition(c, log);
            }
            return true;
        }

        public BuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, BuffRemoveChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public BuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        public BuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public BuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}
