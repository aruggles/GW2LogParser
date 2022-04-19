using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.Events.Damage;
namespace Gw2LogParser.Parser.Data.Events
{
    internal class DamageModifierEvent : AbstractTimeCombatEvent
    {
        public readonly DamageModifier DamageModifier;
        private readonly AbstractHealthDamageEvent _evt;
        public Agent Src => _evt.From;
        public Agent Dst => _evt.To;
        public double DamageGain { get; }

        internal DamageModifierEvent(AbstractHealthDamageEvent evt, DamageModifier damageModifier, double damageGain) : base(evt.Time)
        {
            _evt = evt;
            DamageGain = damageGain;
            DamageModifier = damageModifier;
        }
    }
}
