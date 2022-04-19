using Gw2LogParser.Parser.Data.El.DamageModifiers.GainComputers;
using Gw2LogParser.Parser.Helper;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.DamageModifiers
{
    public class BuffApproximateDamageModifier : BuffDamageModifier
    {
        internal BuffApproximateDamageModifier(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode, dlChecker)
        {
            Approximate = true;
            Tooltip += "<br>Approximate";
        }

        internal BuffApproximateDamageModifier(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, mode, dlChecker)
        {
            Approximate = true;
            Tooltip += "<br>Approximate";
        }

        internal BuffApproximateDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode, dlChecker)
        {
            Approximate = true;
            Tooltip += "<br>Approximate";
        }

        internal BuffApproximateDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, mode, dlChecker)
        {
            Approximate = true;
            Tooltip += "<br>Approximate";
        }
    }
}
