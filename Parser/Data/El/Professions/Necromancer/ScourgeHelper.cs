using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Helper.ArcDPSEnums;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class ScourgeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            // Trail of Anguish
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Sadistic Searing",43626, ParserHelper.Source.Scourge, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dd/Sadistic_Searing.png"),
                new Buff("Path Uses",43410, ParserHelper.Source.Scourge, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/20/Sand_Swell.png"),
        };
    }
}
