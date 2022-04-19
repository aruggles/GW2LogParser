using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Data.El.DamageModifiers.DamageModifier;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class UntamedHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(63191, "Ferocious Symbiosis", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, "https://wiki.guildwars2.com/images/7/73/Ferocious_Symbiosis.png", DamageModifierMode.All),
            new BuffDamageModifier(63232, "Vow of the Untamed", "15% when unleashed", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Vow_of_the_Untamed.png", DamageModifierMode.All),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Ferocious Symbiosis",63191, Source.Untamed, ArcDPSEnums.BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/73/Ferocious_Symbiosis.png"),
            new Buff("Unleashed",63232, Source.Untamed, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Unleash_Ranger.png"),
            new Buff("Pet Unleashed",63145, Source.Untamed, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/43/Unleash_Pet.png"),
            new Buff("Perilous Gift",63284, Source.Untamed, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d6/Perilous_Gift.png"),
            new Buff("Forest's Fortification",63240, Source.Untamed, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/43/Forest%27s_Fortification.png"),
        };
    }
}
