using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Helper.ArcDPSEnums;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class BladeswornHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffLossCastFinder(62861, 62769, InstantCastFinders.InstantCastFinder.DefaultICD, 119939, ulong.MaxValue), // Gunsaber sheath
            new BuffGainCastFinder(62745, 62769, InstantCastFinders.InstantCastFinder.DefaultICD, 119939, ulong.MaxValue), // Gunsaber         
            new DamageCastFinder(62847, 62847, InstantCastFinders.InstantCastFinder.DefaultICD, 119939, ulong.MaxValue), // Unseen Sword
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {

        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Gunsaber Mode", 62769, Source.Bladesworn, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/f0/Unsheathe_Gunsaber.png", 119939, ulong.MaxValue),
            new Buff("Dragon Trigger", 62823, Source.Bladesworn, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/b/b1/Dragon_Trigger.png", 119939, ulong.MaxValue),
            new Buff("Positive Flow", 62836, Source.Bladesworn, BuffStackType.StackingConditionalLoss, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f9/Attribute_bonus.png", 119939, ulong.MaxValue),
            new Buff("Stim State", 62846, Source.Bladesworn, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/a/ad/Combat_Stimulant.png", 119939, ulong.MaxValue),
            new Buff("Guns and Glory", 62743, Source.Bladesworn, BuffStackType.Queue, 9, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/7/72/Guns_and_Glory.png", 119939, ulong.MaxValue),
        };
    }
}
