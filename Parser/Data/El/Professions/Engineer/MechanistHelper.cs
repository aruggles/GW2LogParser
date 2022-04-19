using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class MechanistHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Rectifier Signet",63305, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c4/Rectifier_Signet.png"),
            new Buff("Barrier Signet",63064, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b8/Barrier_Signet.png"),
            new Buff("Force Signet",63243, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b3/Force_Signet.png"),
            new Buff("Shift Signet",63068, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d1/Shift_Signet.png"),
            new Buff("Superconducting Signet",63322, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/51/Superconducting_Signet.png"),
            new Buff("Overclock Signet",63059, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Overclock_Signet.png"),
            //
            //new Buff("Rectifier Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c4/Rectifier_Signet.png"),
            new Buff("Barrier Signet (J-Drive)",63228, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b8/Barrier_Signet.png"),
            //new Buff("Force Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b3/Force_Signet.png"),
            //new Buff("Shift Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d1/Shift_Signet.png"),
            //new Buff("Superconducting Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/51/Superconducting_Signet.png"),
            new Buff("Overclock Signet (J-Drive)",63378, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Overclock_Signet.png"),
        };
    }
}
