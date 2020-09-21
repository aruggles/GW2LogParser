﻿using Gw2LogParser.Parser.Data.El.Professions;
using Gw2LogParser.Parser.Helper;
using Gw2LogParser.Parser.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.DamageModifiers
{
    public class DamageModifiersContainer
    {

        public Dictionary<ParserHelper.Source, List<DamageModifier>> DamageModifiersPerSource { get; }

        public Dictionary<string, DamageModifier> DamageModifiersByName { get; }

        internal DamageModifiersContainer(ulong build, FightLogic.ParseMode mode)
        {
            var AllDamageModifiers = new List<List<DamageModifier>>
            {
                DamageModifier.ItemDamageModifiers,
                DamageModifier.CommonDamageModifiers,
                DamageModifier.FightSpecificDamageModifiers,
                //
                RevenantHelper.DamageMods,
                HeraldHelper.DamageMods,
                RenegadeHelper.DamageMods,
                //
                WarriorHelper.DamageMods,
                BerserkerHelper.DamageMods,
                SpellbreakerHelper.DamageMods,
                //
                GuardianHelper.DamageMods,
                DragonhunterHelper.DamageMods,
                FirebrandHelper.DamageMods,
                //
                EngineerHelper.DamageMods,
                ScrapperHelper.DamageMods,
                HolosmithHelper.DamageMods,
                //
                ThiefHelper.DamageMods,
                DaredevilHelper.DamageMods,
                DeadeyeHelper.DamageMods,
                //
                RangerHelper.DamageMods,
                DruidHelper.DamageMods,
                SoulbeastHelper.DamageMods,
                //
                MesmerHelper.DamageMods,
                ChronomancerHelper.DamageMods,
                MirageHelper.DamageMods,
                //
                NecromancerHelper.DamageMods,
                ReaperHelper.DamageMods,
                ScourgeHelper.DamageMods,
                //
                ElementalistHelper.DamageMods,
                TempestHelper.DamageMods,
                WeaverHelper.DamageMods,
            };
            var currentDamageMods = new List<DamageModifier>();
            foreach (List<DamageModifier> boons in AllDamageModifiers)
            {
                currentDamageMods.AddRange(boons.Where(x => x.Available(build) && x.Keep(mode)));
            }
            DamageModifiersPerSource = currentDamageMods.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
            DamageModifiersByName = currentDamageMods.GroupBy(x => x.Name).ToDictionary(x => x.Key, x =>
            {
                var list = x.ToList();
                if (list.Count > 1)
                {
                    throw new InvalidOperationException("Same name present multiple times in damage mods - " + x.First().Name);
                }
                return list.First();
            });
        }

        public List<DamageModifier> GetModifiersPerProf(string prof)
        {
            var res = new List<DamageModifier>();
            List<ParserHelper.Source> srcs = ParserHelper.ProfToEnum(prof);
            foreach (ParserHelper.Source src in srcs)
            {
                if (DamageModifiersPerSource.TryGetValue(src, out List<DamageModifier> list))
                {
                    res.AddRange(list);
                }
            }
            return res;
        }
    }
}
