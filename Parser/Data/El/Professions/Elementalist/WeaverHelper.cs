﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Data.El.DamageModifiers.DamageModifier;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class WeaverHelper
    {
        private const long FireMajor = 40926;
        private const long FireMinor = 42811;
        private const long WaterMajor = 43236;
        private const long WaterMinor = 43370;
        private const long AirMajor = 41692;
        private const long AirMinor = 43229;
        private const long EarthMajor = 43740;
        private const long EarthMinor = 44822;

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(40183, 42086, InstantCastFinders.InstantCastFinder.DefaultICD), // Primordial Stance
            new BuffGainCastFinder(44926, 45097, 500), // Stone Resonance
            new BuffGainCastFinder(44612, 42683, InstantCastFinders.InstantCastFinder.DefaultICD), // Unravel
            // Fire       
            new BuffGainCastFinder(FireDual, FireDual, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(FireWater, FireWater, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(FireAir, FireAir, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(FireEarth, FireEarth, InstantCastFinders.InstantCastFinder.DefaultICD),
            // Water
            new BuffGainCastFinder(WaterFire, WaterFire, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(WaterDual, WaterDual, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(WaterAir, WaterAir, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(WaterEarth, WaterEarth, InstantCastFinders.InstantCastFinder.DefaultICD),
            // Air
            new BuffGainCastFinder(AirFire, AirFire, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(AirWater, AirWater, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(AirDual, AirDual, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(AirEarth, AirEarth, InstantCastFinders.InstantCastFinder.DefaultICD),
            // Earth
            new BuffGainCastFinder(EarthFire, EarthFire, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(EarthWater, EarthWater, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(EarthAir, EarthAir, InstantCastFinders.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(EarthDual, EarthDual, InstantCastFinders.InstantCastFinder.DefaultICD),
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(42061, "Weaver's Prowess", "10% cDam (8s) after switching element",  DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png", DamageModifierMode.All),
            new BuffDamageModifier(42416, "Elements of Rage", "10% (8s) after double attuning", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png", 0, 115190, DamageModifierMode.All),
            new BuffDamageModifier(42416, "Elements of Rage", "5% (8s) after double attuning", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png", 115190, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(45110, "Woven Fire", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/b/b1/Woven_Fire.png", DamageModifierMode.All),
            new BuffDamageModifier(45267, "Perfect Weave", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/2/2a/Weave_Self.png", DamageModifierMode.All),
            new BuffDamageModifier(new long[] { 719, 5974}, "Swift Revenge", "7% under swiftness/superspeed", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png", 0, 97950, DamageModifierMode.PvE),
            new BuffDamageModifier(new long[] { 719, 5974}, "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png", 97950, ulong.MaxValue, DamageModifierMode.All)
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Dual Fire Attunement", FireDual, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png"),
                new Buff("Fire Water Attunement", FireWater, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/ar8Hn8G.png"),
                new Buff("Fire Air Attunement", FireAir, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/YU31LwG.png"),
                new Buff("Fire Earth Attunement", FireEarth, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/64g3rto.png"),
                new Buff("Dual Water Attunement", WaterDual, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png"),
                new Buff("Water Fire Attunement", WaterFire, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/H1peqpz.png"),
                new Buff("Water Air Attunement", WaterAir, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/Gz1XwEw.png"),
                new Buff("Water Earth Attunement", WaterEarth, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/zqX3y4c.png"),
                new Buff("Dual Air Attunement", AirDual, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                new Buff("Air Fire Attunement", AirFire, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/4ekncW5.png"),
                new Buff("Air Water Attunement", AirWater, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/HIcUaXG.png"),
                new Buff("Air Earth Attunement", AirEarth, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/MCCrMls.png"),
                new Buff("Dual Earth Attunement", EarthDual, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png"),
                new Buff("Earth Fire Attunement", EarthFire, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/Vgu0B54.png"),
                new Buff("Earth Water Attunement", EarthWater, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/exrTKSW.png"),
                new Buff("Earth Air Attunement", EarthAir, Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/Z3P8cPa.png"),
                new Buff("Primordial Stance",42086, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3a/Primordial_Stance.png"),
                new Buff("Unravel",42683, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4b/Unravel.png"),
                new Buff("Weave Self",42951, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/2a/Weave_Self.png"),
                new Buff("Woven Air",43038, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/bc/Woven_Air.png"),
                new Buff("Woven Fire",45110, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b1/Woven_Fire.png"),
                new Buff("Woven Earth",45662, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Woven_Earth.png"),
                new Buff("Woven Water",43499, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a6/Woven_Water.png"),
                new Buff("Perfect Weave",45267, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/2a/Weave_Self.png"),
                new Buff("Molten Armor",43586, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/71/Lava_Skin.png"),
                new Buff("Weaver's Prowess",42061, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png"),
                new Buff("Elements of Rage",42416, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png"),
                new Buff("Stone Resonance",45097, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/57/Stone_Resonance.png"),
                new Buff("Grinding Stones",51658, Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3d/Grinding_Stones.png"),
        };


        private static readonly Dictionary<long, HashSet<long>> _minorsTranslation = new Dictionary<long, HashSet<long>>
        {
            { FireMinor, new HashSet<long> { WaterFire, AirFire, EarthFire, FireDual }},
            { WaterMinor, new HashSet<long> { FireWater, AirWater, EarthWater, WaterDual }},
            { AirMinor, new HashSet<long> { FireAir, WaterAir, EarthAir, AirDual }},
            { EarthMinor, new HashSet<long> { FireEarth, WaterEarth, AirEarth, EarthDual }},
        };

        private static readonly Dictionary<long, HashSet<long>> _majorsTranslation = new Dictionary<long, HashSet<long>>
        {
            { FireMajor, new HashSet<long> { FireWater, FireAir, FireEarth, FireDual }},
            { WaterMajor, new HashSet<long> { WaterFire, WaterAir, WaterEarth, WaterDual }},
            { AirMajor, new HashSet<long> { AirFire, AirWater, AirEarth, AirDual }},
            { EarthMajor, new HashSet<long> { EarthFire, EarthWater, EarthAir, EarthDual }},
        };

        private static long TranslateWeaverAttunement(List<BuffApplyEvent> buffApplies)
        {
            // check if more than 3 ids are present
            // Seems to happen when the attunement bug happens
            // removed the throw
            /*if (buffApplies.Select(x => x.BuffID).Distinct().Count() > 3)
            {
                throw new EIException("Too much buff apply events in TranslateWeaverAttunement");
            }*/
            var duals = new HashSet<long>
            {
                FireDual,
                WaterDual,
                AirDual,
                EarthDual
            };
            HashSet<long> major = null;
            HashSet<long> minor = null;
            foreach (BuffApplyEvent c in buffApplies)
            {
                if (duals.Contains(c.BuffID))
                {
                    return c.BuffID;
                }
                if (_majorsTranslation.ContainsKey(c.BuffID))
                {
                    major = _majorsTranslation[c.BuffID];
                }
                else if (_minorsTranslation.ContainsKey(c.BuffID))
                {
                    minor = _minorsTranslation[c.BuffID];
                }
            }
            if (major == null || minor == null)
            {
                return 0;
            }
            IEnumerable<long> inter = major.Intersect(minor);
            if (inter.Count() != 1)
            {
                throw new InvalidDataException("Intersection incorrect in TranslateWeaverAttunement");
            }
            return inter.First();
        }

        public static List<AbstractBuffEvent> TransformWeaverAttunements(IReadOnlyList<AbstractBuffEvent> buffs, Dictionary<long, List<AbstractBuffEvent>> buffsByID, Agent a, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            var attunements = new HashSet<long>
            {
                5585,
                5586,
                5575,
                5580
            };

            // not useful for us
            /*const long fireAir = 45162;
            const long fireEarth = 42756;
            const long fireWater = 45502;
            const long waterAir = 46418;
            const long waterEarth = 42792;
            const long airEarth = 45683;*/

            var weaverAttunements = new HashSet<long>
            {
               FireMajor,
                FireMinor,
                WaterMajor,
                WaterMinor,
                AirMajor,
                AirMinor,
                EarthMajor,
                EarthMinor,

                FireDual,
                WaterDual,
                AirDual,
                EarthDual,

                /*fireAir,
                fireEarth,
                fireWater,
                waterAir,
                waterEarth,
                airEarth,*/
            };
            // first we get rid of standard attunements
            var toClean = new HashSet<long>();
            var attuns = buffs.Where(x => attunements.Contains(x.BuffID)).ToList();
            foreach (AbstractBuffEvent c in attuns)
            {
                toClean.Add(c.BuffID);
                c.Invalidate(skillData);
            }
            // get all weaver attunements ids and group them by time
            var weaverAttuns = buffs.Where(x => weaverAttunements.Contains(x.BuffID)).ToList();
            if (weaverAttuns.Count == 0)
            {
                return res;
            }
            Dictionary<long, List<AbstractBuffEvent>> groupByTime = GroupByTime(weaverAttuns);
            long prevID = 0;
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in groupByTime)
            {
                var applies = pair.Value.OfType<BuffApplyEvent>().ToList();
                long curID = TranslateWeaverAttunement(applies);
                foreach (AbstractBuffEvent c in pair.Value)
                {
                    toClean.Add(c.BuffID);
                    c.Invalidate(skillData);
                }
                if (curID == 0)
                {
                    continue;
                }
                uint curInstanceID = applies.First().BuffInstance;
                res.Add(new BuffApplyEvent(a, a, pair.Key, int.MaxValue, skillData.Get(curID), curInstanceID, true));
                if (prevID != 0)
                {
                    res.Add(new BuffRemoveManualEvent(a, a, pair.Key, int.MaxValue, skillData.Get(prevID)));
                    res.Add(new BuffRemoveAllEvent(a, a, pair.Key, int.MaxValue, skillData.Get(prevID), 1, int.MaxValue));
                }
                prevID = curID;
            }
            foreach (long buffID in toClean)
            {
                buffsByID[buffID].RemoveAll(x => x.BuffID == NoBuff);
            }
            return res;
        }
    }
}
