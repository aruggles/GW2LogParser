using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.Statistics;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class DamageModData
    {
        public List<object[]> Data { get; } = new List<object[]>();
        public List<List<object[]>> DataTarget { get; } = new List<List<object[]>>();

        private DamageModData(AbstractSingleActor actor, ParsedLog log, IReadOnlyList<DamageModifier> listToUse, PhaseData phase)
        {
            IReadOnlyDictionary<string, DamageModifierStat> dModData = actor.GetDamageModifierStats(null, log, phase.Start, phase.End);
            foreach (DamageModifier dMod in listToUse)
            {
                if (dModData.TryGetValue(dMod.Name, out DamageModifierStat data))
                {
                    Data.Add(new object[]
                    {
                        data.HitCount,
                        data.TotalHitCount,
                        data.DamageGain,
                        data.TotalDamage
                    });
                }
                else
                {
                    Data.Add(new object[]
                    {
                        0,
                        dMod.GetHitDamageEvents(actor, log, null, phase.Start, phase.End).Count,
                        0,
                        dMod.GetTotalDamage(actor, log, null, phase.Start, phase.End)
                    });
                }
            }
            foreach (AbstractSingleActor target in phase.Targets)
            {
                var pTarget = new List<object[]>();
                DataTarget.Add(pTarget);
                dModData = actor.GetDamageModifierStats(target, log, phase.Start, phase.End);
                foreach (DamageModifier dMod in listToUse)
                {
                    if (dModData.TryGetValue(dMod.Name, out DamageModifierStat data))
                    {
                        pTarget.Add(new object[]
                        {
                            data.HitCount,
                            data.TotalHitCount,
                            data.DamageGain,
                            data.TotalDamage
                        });
                    }
                    else
                    {
                        pTarget.Add(new object[]
                        {
                            0,
                            dMod.GetHitDamageEvents(actor, log, target, phase.Start, phase.End).Count,
                            0,
                            dMod.GetTotalDamage(actor, log, target, phase.Start, phase.End)
                        });
                    }
                }
            }
        }
        public static List<DamageModData> BuildDmgModifiersData(ParsedLog log, PhaseData phase, IReadOnlyList<DamageModifier> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                pData.Add(new DamageModData(actor, log, damageModsToUse, phase));
            }
            return pData;
        }

        public static List<DamageModData> BuildPersonalDmgModifiersData(ParsedLog log, PhaseData phase, IReadOnlyDictionary<Spec, IReadOnlyList<DamageModifier>> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                pData.Add(new DamageModData(actor, log, damageModsToUse[actor.Spec], phase));
            }
            return pData;
        }
    }
}
