using Gw2LogParser.Parser.Data.El.DamageModifiers.GainComputers;
using Gw2LogParser.Parser.Helper;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.DamageModifiers
{
    public class DamageLogApproximateDamageModifier : DamageLogDamageModifier
    {
        internal DamageLogApproximateDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, ulong minBuild, ulong maxBuild, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, checker, gainComputer, minBuild, maxBuild, mode)
        {
            Approximate = true;
            Tooltip += "<br>Approximate";
        }

        internal DamageLogApproximateDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, checker, gainComputer, ulong.MinValue, ulong.MaxValue, mode)
        {
            Approximate = true;
            Tooltip += "<br>Approximate";
        }
    }
}
