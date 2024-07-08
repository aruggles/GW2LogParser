﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ScourgeHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(TrailOfAnguish, TrailOfAnguishBuff),
            // Trail of Anguish? Unique effect?
            // new EffectCastFinder(TrailOfAnguish, EffectGUIDs.ScourgeTrailOfAnguish).UsingSrcSpecChecker(Spec.Scourge).UsingICD(6100),
            new DamageCastFinder(NefariousFavorSkill, NefariousFavorShadeHit),
            new DamageCastFinder(GarishPillarSkill, GarishPillarHit),
            new BuffGainCastFinder(DesertShroud, DesertShroudBuff).UsingDurationChecker(6000),
            new BuffGainCastFinder(SandstormShroudSkill, DesertShroudBuff).UsingDurationChecker(3500),
            // new EXTBarrierCastFinder(DesertShroud, DesertShroud),
            new EXTBarrierCastFinder(SandCascadeSkill, SandCascadeBarrier),
            new BuffGainCastFinder(SadisticSearing, SadisticSearing).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new BuffLossCastFinder(SadisticSearingActivation, SadisticSearing).UsingChecker((blcf, combatData, agentData, skillData) => 
            {
                long sadisticSearingDuration = 10000 - blcf.RemovedDuration;
                if (combatData.GetDamageData(ManifestSandShadeShadeHit).Any(x => x.CreditedFrom == blcf.To && x.Time >= blcf.Time - sadisticSearingDuration && x.Time <= blcf.Time)) 
                {
                    return true;
                }
                return false;
            }).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Investigate for Blood as Sand, add a custom buff like for weaver attunements/ virtuoso blades using effect events?
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Sadistic Searing", SadisticSearing, Source.Scourge, BuffClassification.Other, BuffImages.SadisticSearing),
            new Buff("Path Uses", PathUses, Source.Scourge, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.SandSwell),
        };

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Necromancer;

            // Sand Swell portal locations
            if (log.CombatData.TryGetGroupedEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ScourgeSandSwellPortal, out IReadOnlyList<IReadOnlyList<EffectEvent>> sandswellPortals))
            {
                var skill = new SkillModeDescriptor(player, Spec.Scourge, SandSwell, SkillModeCategory.Portal);
                foreach (IReadOnlyList<EffectEvent> group in sandswellPortals)
                {
                    GenericAttachedDecoration first = null;
                    foreach (EffectEvent effect in group)
                    {
                        (long, long) lifespan = effect.ComputeLifespan(log, 8000, player.AgentItem, PathUses);
                        var connector = new PositionConnector(effect.Position);
                        replay.Decorations.Add(new CircleDecoration( 90, lifespan, color, 0.5, connector).UsingSkillMode(skill));
                        GenericAttachedDecoration icon = new IconDecoration(ParserIcons.PortalSandswell, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, connector).UsingSkillMode(skill);
                        if (first == null)
                        {
                            first = icon;
                        }
                        else
                        {
                            replay.Decorations.Add(first.LineTo(icon, color, 0.5).UsingSkillMode(skill));
                        }
                        replay.Decorations.Add(icon);
                    }
                }

            }
            // Shade
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ScourgeShade, out IReadOnlyList<EffectEvent> scourgeShades))
            {
               var skill = new SkillModeDescriptor(player, Spec.Scourge, ManifestSandShadeSkill);
                foreach (EffectEvent effect in scourgeShades)
                {
                    long duration;
                    if (log.FightData.Logic.SkillMode == EncounterLogic.FightLogic.SkillModeEnum.WvW || log.FightData.Logic.SkillMode == EncounterLogic.FightLogic.SkillModeEnum.sPvP)
                    {
                        duration = log.LogData.GW2Build >= GW2Builds.October2019Balance ? 15000 : 10000;
                    } 
                    else
                    {
                        duration = log.LogData.GW2Build >= GW2Builds.July2023BalanceAndSilentSurfCM ? 8000 : 20000;
                    }
                    (long, long) lifespan = effect.ComputeLifespan(log, duration);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(180, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectShade, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
        }
    }
}
