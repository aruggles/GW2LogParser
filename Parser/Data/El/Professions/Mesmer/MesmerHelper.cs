﻿using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.InstantCastFinders;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;
using static Gw2LogParser.Parser.Data.El.DamageModifiers.DamageModifier;
using static Gw2LogParser.Parser.Helper.ArcDPSEnums;
using static Gw2LogParser.Parser.Helper.ParserHelper;
using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.El.Professions
{
    internal static class MesmerHelper
    {
        private static readonly HashSet<Spec> _canSummonClones = new HashSet<Spec>()
        {
            Spec.Mesmer,
            Spec.Chronomancer,
            Spec.Mirage
        };

        internal static bool CanSummonClones(Spec spec)
        {
            return _canSummonClones.Contains(spec);
        }

        private static readonly HashSet<long> _cloneIDs = new HashSet<long>()
        {
            6479,
            8106,
            8107,
            8108,
            8110,
            8111,
            10542,
            15003,
            15032,
            15044,
            15084,
            15090,
            15114,
            15117,
            15131,
            15156,
            15181,
            15196,
            15199,
            15233,
            15240,
            15249,
            18922,
            18939,
            19134,
            19257,
        };

        internal static bool IsClone(Agent agentItem)
        {
            return _cloneIDs.Contains(agentItem.ID);
        }

        internal static bool IsClone(long id)
        {
            return _cloneIDs.Contains(id);
        }

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(10212, 10212, InstantCastFinders.InstantCastFinder.DefaultICD, 0, 115190), // Power spike
            new DamageCastFinder(10211, 10211, InstantCastFinders.InstantCastFinder.DefaultICD, 115190, ulong.MaxValue), // Mantra of Pain
            new EXTHealingCastFinder(10213, 10213, InstantCastFinders.InstantCastFinder.DefaultICD, 115190, ulong.MaxValue), // Mantra of Recovery
            new BuffLossCastFinder(10234, 10233, InstantCastFinders.InstantCastFinder.DefaultICD, (brae, combatData) => {
                return combatData.GetBuffData(brae.To).Any(x =>
                                    x is BuffApplyEvent bae &&
                                    bae.BuffID == 10269 &&
                                    Math.Abs(bae.AppliedDuration - 2000) <= ServerDelayConstant &&
                                    bae.CreditedBy == brae.To &&
                                    Math.Abs(brae.Time - bae.Time) <= ServerDelayConstant
                                 );
                }
            ), // Signet of Midnight
            new BuffGainCastFinder(10197, 10198, InstantCastFinders.InstantCastFinder.DefaultICD), // Portal Entre
            new DamageCastFinder(30192, 30192, InstantCastFinders.InstantCastFinder.DefaultICD), // Lesser Phantasmal Defender
            /*new BuffGainCastFinder(10192, 10243, InstantCastFinders.InstantCastFinder.DefaultICD, 92715, 97950, (evt, combatData) => {
                var buffsLossToCheck = new List<long>
                {
                    10235, 30739, 21751, 10231, 10246, 10233
                }; // signets
                foreach (long buffID in buffsLossToCheck)
                {
                    if (combatData.GetBuffData(buffID).Where(x => x.Time >= evt.Time - ParserHelper.ServerDelayConstant && x.Time <= evt.Time + ParserHelper.ServerDelayConstant && x is BuffRemoveAllEvent).Any())
                    {
                        return false;
                    }
                }
                return true;

            }), // Distortion
            new BuffGainCastFinder(10192, 10243, InstantCastFinders.InstantCastFinder.DefaultICD, 97950, 104844, (evt, combatData) => {
                if (evt.To.Prof == "Chronomancer")
                {
                    return false;
                }
                var buffsLossToCheck = new List<long>
                {
                    10235, 30739, 21751, 10231, 10246, 10233
                }; // signets
                foreach (long buffID in buffsLossToCheck)
                {
                    if (combatData.GetBuffData(buffID).Where(x => x.Time >= evt.Time - ParserHelper.ServerDelayConstant && x.Time <= evt.Time + ParserHelper.ServerDelayConstant && x is BuffRemoveAllEvent).Any())
                    {
                        return false;
                    }
                }
                return true;

            }), // Distortion
            new BuffGainCastFinder(10192, 10243, InstantCastFinders.InstantCastFinder.DefaultICD, 104844, ulong.MaxValue, (evt, combatData) => {
                var buffsLossToCheck = new List<long>
                {
                    10235, 30739, 21751, 10231, 10246, 10233
                }; // signets
                foreach (long buffID in buffsLossToCheck)
                {
                    if (combatData.GetBuffData(buffID).Where(x => x.Time >= evt.Time - ParserHelper.ServerDelayConstant && x.Time <= evt.Time + ParserHelper.ServerDelayConstant && x is BuffRemoveAllEvent).Any()) 
                    {
                        return false;
                    }
                }
                return true;
                
            }), // Distortion*/
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Domination
            // Empowered illusions require knowing all illusion species ID
            // We need illusion species ID to enable Vicious Expression on All
            new BuffDamageModifierTarget(NumberOfBoonsID, "Vicious Expression", "25% on boonless target",  DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByAbsence, "https://wiki.guildwars2.com/images/f/f6/Confounding_Suggestions.png", 102321, 102389, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(NumberOfBoonsID, "Vicious Expression", "15% on boonless target",  DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByAbsence, "https://wiki.guildwars2.com/images/f/f6/Confounding_Suggestions.png", 102389, ulong.MaxValue, DamageModifierMode.All),
            new DamageLogApproximateDamageModifier("Egotism", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Mesmer, "https://wiki.guildwars2.com/images/7/78/Temporal_Enchanter.png", (x,log) =>
            {
                var selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                var dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, 92715, ulong.MaxValue, DamageModifierMode.PvE),
            new DamageLogApproximateDamageModifier("Egotism", "5% if target hp% lower than self hp%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Mesmer, "https://wiki.guildwars2.com/images/7/78/Temporal_Enchanter.png", (x,log) =>
            {
                var selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                var dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, 92715, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(738, "Fragility", "0.5% per stack vuln on target", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, "https://wiki.guildwars2.com/images/3/33/Fragility.png", DamageModifierMode.All),
            // Dueling
            // Superiority Complex can all the conditions be tracked?
            // Illusions
            new BuffDamageModifier(49058, "Compounding Power", "2% per stack (8s) after creating an illusion ", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png", DamageModifierMode.All),
            // Phantasmal Force: the current infrastructure is not capable of checking buffs on minions, once we have that, this does not require knowing illusion species id
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            
                //signets
                new Buff("Signet of the Ether", 21751, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Signet_of_the_Ether.png"),
                new Buff("Signet of Domination",10231, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3b/Signet_of_Domination.png"),
                new Buff("Signet of Illusions",10246, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Illusions.png"),
                new Buff("Signet of Inspiration",10235, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ed/Signet_of_Inspiration.png"),
                new Buff("Signet of Midnight",10233, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/24/Signet_of_Midnight.png"),
                new Buff("Signet of Humility",30739, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b5/Signet_of_Humility.png"),
                //skills
                new Buff("Distortion",10243, Source.Mesmer, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Blur", 10335 , Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Mirror",10357, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b8/Mirror.png"),
                new Buff("Echo",29664, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Echo.png"),
                new Buff("Illusionary Counter",10278, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e5/Illusionary_Counter.png"),
                new Buff("Illusionary Riposte",10279, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Illusionary_Riposte.png"),
                new Buff("Illusionary Leap",10353, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Illusionary_Leap.png"),
                new Buff("Portal Weaving",10198, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/81/Portal_Entre.png"),
                new Buff("Illusion of Life",10346, Source.Mesmer, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/9/92/Illusion_of_Life.png"),
                //traits
                new Buff("Fencer's Finesse", 30426 , Source.Mesmer, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e7/Fencer%27s_Finesse.png"),
                new Buff("Illusionary Defense",49099, Source.Mesmer, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e0/Illusionary_Defense.png"),
                new Buff("Compounding Power",49058, Source.Mesmer, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png"),
                new Buff("Phantasmal Force", 44691 , Source.Mesmer, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5f/Mistrust.png"),
        };
    }
}
