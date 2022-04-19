using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class SpecterHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(63155, 63239, InstantCastFinders.InstantCastFinder.DefaultICD), // Shadow Shroud Enter
            new BuffLossCastFinder(63251, 63239, InstantCastFinders.InstantCastFinder.DefaultICD), // Shadow Shroud Exit
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Shadow Shroud",63239, Source.Specter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f3/Enter_Shadow_Shroud.png"),
            new Buff("Shrouded Ally",63207, Source.Specter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3a/Siphon.png"),
            new Buff("Rot Wallow Venom",63168, Source.Specter, ArcDPSEnums.BuffStackType.StackingConditionalLoss, 100, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/5/57/Dark_Sentry.png"),
        };
    }
}
