using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class MirageHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(45449, 45449, InstantCastFinders.InstantCastFinder.DefaultICD), // Jaunt
            new BuffGainCastFinder(Skill.MirageCloakDodgeId, 40408, InstantCastFinders.InstantCastFinder.DefaultICD), // Mirage Cloak
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Mirage Cloak",40408, ParserHelper.Source.Mirage, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a5/Mirage_Cloak_%28effect%29.png"),
                new Buff("False Oasis",40802, ParserHelper.Source.Mirage, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/32/False_Oasis.png"),
        };
    }
}
