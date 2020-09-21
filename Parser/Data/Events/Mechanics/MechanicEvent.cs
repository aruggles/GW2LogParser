using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;

namespace Gw2LogParser.Parser.Data.Events.Mechanics
{
    public class MechanicEvent : AbstractTimeCombatEvent
    {
        private readonly Mechanic _mechanic;
        public AbstractSingleActor Actor { get; }
        public string ShortName => _mechanic.ShortName;
        public string Description => _mechanic.Description;

        internal MechanicEvent(long time, Mechanic mech, AbstractSingleActor actor) : base(time)
        {
            Actor = actor;
            _mechanic = mech;
        }
    }
}
