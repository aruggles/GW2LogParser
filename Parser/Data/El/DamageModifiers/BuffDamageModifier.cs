using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers.BuffTrackers;
using Gw2LogParser.Parser.Data.El.DamageModifiers.GainComputers;
using Gw2LogParser.Parser.Data.Events;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.DamageModifiers
{
    public class BuffDamageModifier : DamageModifier
    {

        internal BuffsTracker Tracker { get; }

        internal BuffDamageModifier(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, dlChecker, ulong.MinValue, ulong.MaxValue, mode)
        {
            Tracker = new BuffsTrackerSingle(id);
        }

        internal BuffDamageModifier(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, dlChecker, minBuild, maxBuild, mode)
        {
            Tracker = new BuffsTrackerSingle(id);
        }

        internal BuffDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, dlChecker, ulong.MinValue, ulong.MaxValue, mode)
        {
            Tracker = new BuffsTrackerMulti(new List<long>(ids));
        }

        internal BuffDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, dlChecker, minBuild, maxBuild, mode)
        {
            Tracker = new BuffsTrackerMulti(new List<long>(ids));
        }

        protected double ComputeGain(int stack, AbstractHealthDamageEvent dl, ParsedLog log)
        {
            if (DLChecker != null && !DLChecker(dl, log))
            {
                return -1.0;
            }
            double gain = GainComputer.ComputeGain(GainPerStack, stack);
            return gain > 0.0 ? gain * dl.HealthDamage : -1.0;
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedLog log)
        {
            Dictionary<long, BuffsGraphModel> bgms = actor.GetBuffGraphs(log);
            if (!Tracker.Has(bgms) && GainComputer != ByAbsence)
            {
                return new List<DamageModifierEvent>();
            }
            var res = new List<DamageModifierEvent>();
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = GetHitDamageEvents(actor, log, null, 0, log.FightData.FightEnd);
            foreach (AbstractHealthDamageEvent evt in typeHits)
            {
                res.Add(new DamageModifierEvent(evt, this, ComputeGain(Tracker.GetStack(bgms, evt.Time), evt, log)));
            }
            res.RemoveAll(x => x.DamageGain == -1.0);
            return res;
        }
    }
}
