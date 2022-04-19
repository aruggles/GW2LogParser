using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.DamageModifiers.GainComputers;
using Gw2LogParser.Parser.Data.Events;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.DamageModifiers
{
    public class DamageLogDamageModifier : DamageModifier
    {

        internal DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, ulong minBuild, ulong maxBuild, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, checker, minBuild, maxBuild, mode)
        {
        }

        internal DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, checker, ulong.MinValue, ulong.MaxValue, mode)
        {
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedLog log)
        {
            var res = new List<DamageModifierEvent>();
            double gain = GainComputer.ComputeGain(GainPerStack, 1);
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = GetHitDamageEvents(actor, log, null, 0, log.FightData.FightEnd);
            foreach (AbstractHealthDamageEvent evt in typeHits)
            {
                if (DLChecker(evt, log))
                {
                    res.Add(new DamageModifierEvent(evt, this, gain * evt.HealthDamage));
                }
            }
            return res;
        }
    }
}
