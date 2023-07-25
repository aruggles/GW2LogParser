﻿using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData.Buffs
{
    internal static class WvWBuffs
    {
        internal static readonly List<Buff> Commons = new List<Buff>
        {
            new Buff("Minor Borderlands Bloodlust", MinorBorderlandsBloodlust, Source.Common, BuffClassification.Support, BuffImages.BorderlandBloodlust),
            new Buff("Major Borderlands Bloodlust", MajorBorderlandsBloodlust, Source.Common, BuffClassification.Support, BuffImages.BorderlandBloodlust),
            new Buff("Superior Borderlands Bloodlust", SuperiorBorderlandsBloodlust, Source.Common, BuffClassification.Support, BuffImages.BorderlandBloodlust),
            new Buff("Blessing of Elements", BlessingOfElements, Source.Common, BuffClassification.Support, BuffImages.BlessingOfAir),
            new Buff("Flame's Embrace", FlamesEmbrace, Source.Common, BuffClassification.Support, BuffImages.FlamesEmbrace),
            new Buff("Guild Objective Aura I", GuildObjectiveAuraI, Source.Common, BuffClassification.Support, BuffImages.WvWMissionSlot),
            new Buff("Guild Objective Aura II", GuildObjectiveAuraII, Source.Common, BuffClassification.Support, BuffImages.WvWMissionSlot),
            new Buff("Guild Objective Aura III", GuildObjectiveAuraIII, Source.Common, BuffClassification.Support, BuffImages.WvWMissionSlot),
            new Buff("Guild Objective Aura IV", GuildObjectiveAuraIV, Source.Common, BuffClassification.Support, BuffImages.WvWMissionSlot),
            new Buff("Guild Objective Aura V", GuildObjectiveAuraV, Source.Common, BuffClassification.Support, BuffImages.WvWMissionSlot),
            new Buff("Guild Objective Aura VI", GuildObjectiveAuraVI, Source.Common, BuffClassification.Support, BuffImages.WvWMissionSlot),
            new Buff("Guild Objective Aura VII", GuildObjectiveAuraVII, Source.Common, BuffClassification.Support, BuffImages.WvWMissionSlot),
            new Buff("Guild Objective Aura VIII", GuildObjectiveAuraVIII, Source.Common, BuffClassification.Support, BuffImages.WvWMissionSlot),
            new Buff("Protected Caravan", ProtectedCaravan, Source.Common, BuffClassification.Other, BuffImages.Perseverance),
            new Buff("Righteous Indignation", RighteousIndignation, Source.Common, BuffClassification.Other, BuffImages.Brilliance),
            new Buff("Gliding Disabled Warning", GlidingDisabledWarning, Source.Common, BuffClassification.Debuff, BuffImages.GlidingDisabledWarning),
            new Buff("Siege Disabled", SiegeDisabled, Source.Common, BuffClassification.Debuff, BuffImages.SiegeDisabled),
            new Buff("Force Dome", ForceDomeTier5Buff, Source.Common, BuffClassification.Support, BuffImages.ForceDome),
            new Buff("Structural Vulnerability", StructuralVulnerability, Source.Common, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.StructuralVulnerability),
            new Buff("Sabotage Depot", SabotageDepot, Source.Common, BuffClassification.Other, BuffImages.SabotagedWeaponParts),
            new Buff("Iron Hide (Ram)", IronHideRam, Source.Common, BuffClassification.Defensive, BuffImages.IronHide),
            new Buff("Iron Hide (Guard)", IronHideGuards, Source.Common, BuffClassification.Other, BuffImages.IronHide),
            new Buff("Siege Decay Timer", SiegeDecayTimer, Source.Common, BuffClassification.Other, BuffImages.SiegeDecayTimer),
            new Buff("Presence of the Keep", PresenceOfTheKeep, Source.Common, BuffClassification.Support, BuffImages.PresenceOfTheKeep),
            new Buff("Oil Mastery III (Increased Armor)",IncreasedArmorOilMasteryIII, Source.Common, BuffClassification.Defensive, BuffImages.BurningOilMastery),
            new Buff("Chain Attached", ChainAttached, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.ChainPull),
            new Buff("Hardened Gates", HardenedGates, Source.Common, BuffClassification.Other, BuffImages.HardenedGates),
            new Buff("Siege Deployment Blocked", SiegeDeploymentBlocked, Source.Common, BuffClassification.Debuff, BuffImages.SiegeDeploymentBlocked),
            new Buff("Outnumbered", Outnumbered, Source.Common, BuffClassification.Other, BuffImages.Bombard),
            new Buff("Hardened Siege Gear", HardenedSiegeGear, Source.Common, BuffClassification.Other, BuffImages.HardenedSiegeGear),
            new Buff("Marked (Red Sentry)", MarkedSentryRed, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Blue Sentry)", MarkedSentryBlue, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Green Sentry)", MarkedSentryGreen, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Red Tower)", MarkedTowerRed, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Blue Tower)", MarkedTowerBlue, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Green Tower)", MarkedTowerGreen, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Red Keep)", MarkedKeepRed, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Blue Keep)", MarkedKeepBlue, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Green Keep)", MarkedKeepGreen, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Dragon Banner", DragonBanner, Source.Common, BuffClassification.Support, BuffImages.DragonBanner),
            new Buff("Turtle Banner", TurtleBanner, Source.Common, BuffClassification.Support, BuffImages.TurtleBanner),
            new Buff("Centaur Banner", CentaurBanner, Source.Common, BuffClassification.Support, BuffImages.CentaurBanner),
            new Buff("Structural Integrity", StructuralIntegrity, Source.Common, BuffClassification.Other, BuffImages.ThoughtlessPotion),
            new Buff("No Mount Use (Warclaw)", NoMountUseWarclaw, Source.Common, BuffClassification.Debuff, BuffImages.MountsDisabled),
            new Buff("Speed of the Battlefield", SpeedOfTheBattlefield, Source.Common, BuffClassification.Support, BuffImages.AllysAidFeatherfoot),
            new Buff("Siege Golem Mastery Rank 2", SiegeGolemMasteryRank2, Source.Common, BuffClassification.Support, BuffImages.SiegeGolemMastery),
            new Buff("Siege Golem Mastery Rank 4 (Alpha)", SiegeGolemMasteryRank4Alpha, Source.Common, BuffClassification.Support, BuffImages.SiegeGolemMastery),
            new Buff("Siege Golem Mastery Rank 4 (Omega)", SiegeGolemMasteryRank4Omega, Source.Common, BuffClassification.Support, BuffImages.SiegeGolemMastery),
            new Buff("Dune Roller", DuneRoller, Source.Common, BuffClassification.Support, BuffImages.BattleRoar),
            new Buff("Invulnerable Dolyak", InvulnerableDolyak, Source.Common, BuffClassification.Other, BuffImages.Brilliance),
            new Buff("Superspeed (Speedy Dolyak)", SuperspeedSpeedyDolyak, Source.Common, BuffClassification.Other, BuffImages.Superspeed),
            new Buff("Spoiled Supply", SpoiledSupply, Source.Common, BuffClassification.Other, BuffImages.SpoiledSupply),
            new Buff("Commander's Presence", CommandersPresence, Source.Common, BuffClassification.Support, BuffImages.MightyBlow),
            new Buff("Inferno Hound", InfernoHound, Source.Common, BuffClassification.Other, BuffImages.HoundsOfBalthazar),
            new Buff("Smoke Form", SmokeForm, Source.Common, BuffClassification.Other, BuffImages.InkShot),
            new Buff("Chilling Fog", ChillingFogBuff, Source.Common, BuffClassification.Debuff, BuffImages.FreezingGust),
            // Edge of the Mists
            new Buff("Marked (Mists Arena)", MarkedMistsArena, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Red Sentry Turret)", MarkedSentryTurretRed, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Green Sentry Turret)", MarkedSentryTurretGreen, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Marked (Blue Sentry Turret)", MarkedSentryTurretBlue, Source.Common, BuffClassification.Debuff, BuffImages.FireTrebuchet),
            new Buff("Overgrowth Bonus Tier 1", OvergrowthBonusTier1, Source.Common, BuffClassification.Support, BuffImages.InvokeLightning),
            new Buff("Overgrowth Bonus Tier 2", OvergrowthBonusTier2, Source.Common, BuffClassification.Support, BuffImages.InvokeLightning),
            new Buff("Overgrowth Bonus Tier 3", OvergrowthBonusTier3, Source.Common, BuffClassification.Support, BuffImages.InvokeLightning),
            new Buff("Koda's Armor", KodasArmor, Source.Common, BuffClassification.Support, BuffImages.KodasArmor),
        };
    }
}
