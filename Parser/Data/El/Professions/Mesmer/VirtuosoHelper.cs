using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using static Gw2LogParser.Parser.Helper.ParserHelper;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.DamageModifiers.DamageModifier;
using Gw2LogParser.Parser.Data.El.Statistics;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class VirtuosoHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new DamageLogApproximateDamageModifier("Mental Focus", "15% to foes within 600 range", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Virtuoso, "https://wiki.guildwars2.com/images/d/da/Mental_Focus.png", (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 600;
            }, ByPresence, 118697, ulong.MaxValue, DamageModifierMode.All)
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            // blade?
        };
    }
}
