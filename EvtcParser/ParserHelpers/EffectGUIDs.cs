﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser
{
    public static class EffectGUIDs
    {
        #region Generic
        // Generic
        // blue circles indicating radius of boons etc.
        public const string Generic240UnitRadius = "E7C50E0E148CBE44BB2770AF2D6750A4"; // e.g. speed of synergy, bypass coating
        public const string Generic360UnitRadius = "10873BDE22D87845AAF004B0A60FA546"; // e.g. crisis zone
        public const string Generic360UnitRadius2 = "0B3A5E8DDBB43447815547D96E7CA146"; // e.g. over shield, deathly haste, mechanical genius, barrier burst
        public const string Generic360Or600UnitRadius = "4C7A5E148F7FD642B34EE4996DDCBBAB"; // somehow both? e.g. chaos vortex, medical dispersion field, reconstruction enclosure, barrier engine
        public const string Generic600UnitRadius = "9C7C1B2379CCDD4990001A38030E4495"; // e.g. ranger spirits, protect me
        public const string Generic900UnitRadius = "EB9EBC2CB610B448BB00B7FBCB191F28"; // e.g. call of the wild
        public const string GenericTrapInactive = "D9F9B146BC2B914B874EA980B2FF0C00";
        public const string RuneOfNightmare = "149E616EB45B1E4982305B99A7952EA8";
        public const string StealthApply = "B44BAD999BEB2D4DB284745895B42BDD";
        public const string StealthReveal = "A37F8E2B550B254DA89F933BDF654B41"; // also used with e.g. infiltrators strike, infiltrators arrow, shadowstep, shadow return, infiltrators signet
        public const string WhiteMantlePortalInactive = "D43373FEFA19A54DA2A2B6BB7834A338";
        public const string WhiteMantlePortalActive = "388CF9246218A34DB2F8107E19FCA471";
        #endregion
        #region Mounts
        // Skyscale
        public const string SkyscaleLaunch = "F6A06D8222280F40B17A6984F9B5894F";
        public const string SkyscaleFireball = "0325D55E6A772047981F4EEAA5CAE537";
        public const string SkyscaleFireballExplosion = "BA8C9DDEAC761A48ACC53777F1D68C34";
        public const string SkyscaleBlast1 = "4D30A7F374E56E4F806420A81EAEA03F";
        public const string SkyscaleBlast2 = "E15FF7FD742AA24BB6C2BE38087CFC22";
        #endregion
        #region Gear
        // Relics
        public const string RelicWhiteCircle = "866307A6A0E34242BDC3067AB24A549D"; // Appears for Nightmare, Citadel, Krait
        public const string RelicOfCerusEye = "1066BEACB107C743908D860DA2D59796";
        public const string RelicOfCerusEye2 = "521B6C72BF291E4E8A895A0827AF1727";
        public const string RelicOfCerusBeam = "513AEEF08C217942A798831BD9F4903E"; // 1 second delayed
        public const string RelicOfCerusBeam2 = "43F06D75DF774C4DBB1383B8621B1047"; // 1 second delayed
        public const string RelicOfIce = "54F2B4920F7E2D4FAA56CED739BA2C41";
        public const string RelicOfTheCitadelExplosion = "DE4373D3B08DE04E903B99B6F9194F74"; // 2.5 seconds delayed
        public const string RelicOfFireworks = "2BC033D40C0AEB40A77EEF28D51AE263";
        public const string RelicOfTheNightmare = "0FCF4B1F75575949AA4997365FAE0288";
        public const string RelicOfTheKrait = "0685880038B8F441B191439DB678A5F8";
        public const string RelicOfTheSunless = "B1FFFAEFC0A3C74D96851E07C75B3FC7"; // Target Src
        public const string RelicOfTheWizardsTower = "2A1D0C23F448C348A83E9A4F2669B73F";
        public const string RelicOfPeitha = "0CBFB70434661647B68003ECD77207E6"; // Projectile
        public const string RelicOfAkeem = "8181E21A43EAFB42BC8FFB001F02CF44"; // Target Src
        #endregion
        #region Mesmer
        public const string MesmerThePrestigeDisappear1 = "48B69FBC3090E144BFC067D6C0208878";
        public const string MesmerThePrestigeDisappear2 = "5FA6527231BB8041AC783396142C6200"; // also used with elementalist cleansing fire
        public const string MesmerSignetOfMidnight = "02154B72900B5740A73CD0ADECED27BF";
        public const string MesmerFeedback = "D6C8F406E4DEE04AB16A215BE068E910";
        public const string MesmerVeil = "6B29E895E2EB9341B560FFD3A78F78F2";
        public const string MesmerNullField = "D8E8B086ACCF7549B8F50CF1AF177039";
        public const string MesmerTeleport = "C34E250B01FF534292EE6AB36D768337"; // used by blink, phase retreat, swap (illusionary leap)
        public const string MesmerPortalInactive = "F3CD4D9BFC8EAD45AAA1EA7A3AB148BF";
        public const string MesmerPortalActive = "3C346BE32EFB9E40BE39E379B061C803";
        public const string MesmerCryOfFrustration = "52F65A4D9970954BA849CB57A46A65A8";
        public const string MesmerDiversion = "916D8385083F144EBAA5BEEDE21FD47A";
        public const string MesmerDistortionOrMindWrack = "3D29ABD39CB5BD458C4D50A22FCC0E4B";
        public const string MesmerMantraOfResolveAndPowerCleanse = "593E668A006AB24D84999AED68F2E4C4";
        public const string MesmerMantraOfResolveAndPowerCleanse2 = "ABF2332D28C7D6449A5B822E5714ADA4";
        public const string MesmerMantraOfConcentrationAndPowerBreak = "5B488D552E316045AD99C4A98EEDDB1E";
        public const string MesmerPowerReturn = "F53E2CE3B06B934085D46FA59468477B";
        public const string MesmerDimensionalAperturePortal = "9246D82C91B5274396DBAB561DC8EFAF";
        public const string ChronomancerSeizeTheMomentShatter = "4C7A5E148F7FD642B34EE4996DDCBBAB"; // This seems to happen everytime split second, rewinder, time sink or continuum split are cast under SeizeTheMoment
        public const string ChronomancerSplitSecond = "C035166E3E4C414ABE640F47797D9B4A"; // this is also triggered by the clones while being sourced to the chrono
        public const string ChronomancerRewinder = "DC1C8A043ADCD24B9458688A792B04BA"; // this is also triggered by the clones while being sourced to the chrono
        public const string ChronomancerTimeSink = "AB2E22E7EE74DA4C87DA777C62E475EA";
        public const string ChronomancerWellGeneric = "643B0B821F470748BC877B089ACD0C18";
        public const string ChronomancerWellOfEternity = "F574310CA199DF4488AFFA0216BA1454";
        public const string ChronomancerWellOfEternityPulse = "AB9D0AC13EEBE7459CFA914542567F40"; // 1 pulse every second, 3 total
        public const string ChronomancerWellOfEternityExplosion = "E35E0D4F892AAD4BB76B2A5C6F9DE8C1"; // 1 second after final pulse
        public const string ChronomancerWellOfAction = "6CB76B96F4242C468BFE5FA5FA038D73";
        public const string ChronomancerWellOfActionPulse = "FD2802E2DC92124F940DDDD998B8B57B"; // 1 pulse every second, 3 total
        public const string ChronomancerWellOfActionExplosion = "F57C7DD4E9BDE348BF8583F97E1C01C1"; // 1 second after final pulse
        public const string ChronomancerWellOfCalamity = "4C7071BBE735D843B12893898A4C2688";
        public const string ChronomancerWellOfCalamityPulse = "D49238E0B3A365489A5B601EDB68F942"; // 1 pulse every second, 3 total
        public const string ChronomancerWellOfCalamityExplosion = "CEB6C2416CF40C44B0C156BCBF247E24"; // 1 second after final pulse
        public const string ChronomancerWellOfPrecognition = "AB99EA6C6534B74597277998C301866B";
        public const string ChronomancerWellOfPrecognitionPulse = "E5F4B6BC7A9F084CAB5FD548163DF7CF"; // 1 pulse every second, 3 total
        public const string ChronomancerWellOfPrecognitionExplosion = "9DDFE99A09A8FA45810B1ED41B310B74"; // 1 second after final pulse
        public const string ChronomancerWellOfSenility = "9ED7EC5CDA3A7D4DB44998CE40C8CF31";
        public const string ChronomancerWellOfSenilityPulse = "DF910E4C6F75DB4CB345889EA68808B2"; // 1 pulse every second, 3 total
        public const string ChronomancerWellOfSenilityExplosion = "8799483A7E93AB4385451033145B3345"; // 1 second after final pulse
        public const string ChronomancerGravityWell = "42B42983DA2E2D45876D201F1DCECE73";
        public const string ChronomancerGravityWellPulse = "60A74BCA1ECF974FB31CE28ABDF6D8AE"; // 1 pulse every second, 3 total
        public const string ChronomancerGravityWellExplosion = "E0D03976A4BC034E8ABFBBECCC828932"; // 1 second after final pulse
        public const string MirageCloak = "4C7A5E148F7FD642B34EE4996DDCBBAB";
        public const string MirageMirror = "1370CDF5F2061445A656A1D77C37A55C";
        public const string MirageJaunt = "3A5A38C26A1FFB438EAD734F3ED42E5E"; // may have collisions! not known which
        public const string VirtuosoUnstableBladestorm = "DEF12997FAEA6847A8786CD2920ACA91";
        public const string VirtuosoBladeturnRequiem = "87B761200637AC48B71469F553BA6F60";
        public const string VirtuosoRainOfSwords = "83834EDBA8E79946A6D5665E3519B72C";
        public const string VirtuosoThousandCuts = "E4002B7AD7DF024394D0184B47A316E7";
        // public const string MirageJauntConflict1 = "B6557C336041B24FA7CC198B6EBDAD9A"; // used with e.g. jaunt & axes of symmetry

        // public const string MirageJauntConflict2 = "D7A05478BA0E164396EB90C037DCCF42"; // used with e.g. jaunt, axes of symmetry, illusionary ambush
        // public const string MesmerTrail = "73414BA39AFCF540A90CF91DE961CCEF"; // used with e.g. mirror images, phase retreat, illusionary ambush - likely the trail left behind
        #endregion
        #region Necromancer
        public const string NecromancerNecroticTraversal = "47C48881C5AC214388F6253197A7F11A";
        public const string NecromancerUnholyBurst = "C4E8DD3234E0C647993857940ED79AC1"; // also used for spiteful spirit
        public const string NecromancerPlagueSignet = "E78ED095E97F1D4A8BEB901796449E2F"; // might be pov only?
        public const string NecromancerWellOfBlood = "159515DADB2DFB46A980A2A661BD881B";
        public const string NecromancerWellOfSuffering = "E24BA6F2CCB8374CB7F5BE829BC7228E";
        public const string NecromancerWellOfDarkness = "824EF999A0B6D14D9AC2EC843C6984D5";
        public const string NecromancerWellOfCorruption = "FF96BAE8EC4D5A4CBF6E13C15649F3DA";
        public const string NecromancerCorrosivePoisonCloud = "68D13E0EBB247A40B2F131B2C729443E";
        public const string NecromancerPlaguelands = "883D5C97F3673843A8423D01B97ED78F";
        public const string NecromancerPlaguelandsPulse1 = "7E12B3B1896BC748AE50333267CDBFB9";
        public const string NecromancerPlaguelandsPulse2 = "A442AE5DFE73D04BBC19B050540E000A";
        public const string NecromancerPlaguelandsPulse3 = "253ADDAEC2009A499FA29C44E1D73F05";
        public const string NecromancerMarkOfBloodOrChillblains = "A859FDB2593E2C4ABEFB51907393BBAA";
        public const string NecromancerPutridMark = "651695CA0DB15E4F88E30BF58630B891";
        public const string NecromancerReapersMark = "074AFD46220642429D67BF645CA81D65";
        public const string NecromancerMarkOfBloodActivated1 = "31D543A5DCEF9643A59EF9498A55ACDE";
        public const string NecromancerMarkOfBloodActivated2 = "5FA6527231BB8041AC783396142C6200";
        public const string NecromancerChillblainsActivated = "831159227814DF4FA354CAD7E0755FEE";
        public const string NecromancerPutridMarkActivated1 = "E52F2D6DBABA934882BBBB8F0832C777";
        public const string NecromancerPutridMarkActivated2 = "EFB9CDA30AEBC744B9D377A99BEBC0B2";
        public const string NecromancerPutridMarkActivated3 = "CAF4E62C2C5CC04499657C2A6A78087B"; // No src or dst
        public const string NecromancerReapersMarkActivated = "255FBE1C15D0C6488BD018748184624F";
        public const string ReaperSuffer = "6C8C388BCD26F04CA6618D2916B8D796";
        public const string ReaperYouAreAllWeaklings1 = "37242DF51D238A409E822E7A1936D7A6"; // 3 potential candidates, 4th effect has collisions
        public const string ReaperYouAreAllWeaklings2 = "FEE4F26C2866E34C9D75506A8ED94F5E";
        public const string ReaperYouAreAllWeaklings3 = "ED6A8440CB49B248A352B2073FAF1F5F";
        public const string ScourgeTrailOfAnguish = "1DAE3CAEF2228845867AAF419BF31E8C";
        public const string ScourgeShade = "78408C6DA08C2746BEABEB995187271A";
        public const string ScourgeShadeStrike = "C8B109540159AA429E83D0AA98EF3E90";
        public const string ScourgeSandSwellPortal = "086CF7823EB13047B2187E7933639703";
        public const string HarbingerCascadingCorruption = "EEDCAB61CD35E840909B03D398878B1C";
        public const string HarbingerDeathlyHaste = "9C06D9D9B0E22247A1752C426808CD80";
        public const string HarbingerDoomApproaches = "88C0010F0B7148469B88E2A1B4500DCC";
        public const string HarbingerVitalDrawSelfDst = "667EAEE89766E14E883E6ECA5D3D267B"; // Target self
        public const string HarbingerVitalDrawAoE = "859611F71893924989B056F6A011C160"; // Ground effect
        #endregion
        #region Elementalist
        public const string ElementalistArmorOfEarth1 = "D43DC34DEF81B746BC130F7A0393AAC7";
        public const string ElementalistArmorOfEarth2 = "D0C072102FAA6A4EA8A16CB73F3B96DD"; // happens at the same time as the other, could be relevant to check should collisions appear
        //public const string ElementalistCleansingFire = "5FA6527231BB8041AC783396142C6200"; // also used with mesmer the prestige, collides with some air traits
        public const string ElementalistSignetOfAir = "30A96C0E559DBD489FEE36DA96CC374A";
        //public const string ElementalistLightningFlash = "40818C8E9CC6EF4388C2821FCC26A9EC"; // Conflicts with certain field combos, thief teleport skills, guardian judges/merciful intervention
        public const string ElementalistMeteorShowerCircle = "0F42F49776A5F74E8A0CADC4BCF53904";
        public const string ElementalistMeteorShowerMeteor = "F3DD685A8E52124A9FCC653C90EA789A";
        public const string ElementalistStaticFieldStaff = "1ED1C9E57048CF419AFB9C31329FF51E";
        public const string ElementalistStaticFieldLightningHammer = "E32640807FA71947BE21177E2C75043C";
        public const string ElementalistUpdraft = "DFFD3374FA23D644A6D0BE37216938C5";
        public const string ElementalistUpdraft2 = "0FA8EF1CE419504A9D03004D6CF5F073";
        public const string ElementalistUpdraftWind = "DE8C1CE6E3AC3445911B214CA8021BDD";
        public const string ElementalistFirestorm = "172F43AB94CB214D95A6EA7F7DFCE520"; // Same for Glyph of Storms and Conjured Fiery Greatsword
        public const string ElementalistGeyserSplash = "C0FAFED39AEDD948B025AA1272B80A8B";
        public const string ElementalistGeyser = "3A15A72D28971D4D8CE5C24DB66C5595";
        public const string TempestOverloadFire1 = "675AE0297C86764ABC4A5988CE76A20E";
        public const string TempestOverloadFire2 = "977D44CE34F6B9438BCDCFA074BBDCA8";
        public const string TempestOverloadAir1 = "3CE58ECAB1EE9C4E96F70E3A64967F55";
        public const string TempestOverloadAir2 = "E8EB2CDF97F34C42A8AAC0D3BA6551D0";
        public const string TempestFeelTheBurn = "C668B5DB6220D9448817B3E5F7DE6E46";
        public const string TempestEyeOfTheStorm1 = "52FEF389CF7D014BAA375EACF1826BB6";
        public const string TempestEyeOfTheStorm2 = "31FE88E9CCF82047895FD0EF19C9BBA0"; // happens at the same time as the other, could be relevant to check should collisions appear 
        public const string TempestLightningOrb1 = "DE4C727C58DA0A4EB87D5433B2B64EAB";
        public const string TempestLightningOrb2 = "AF5462A3F3500A4B8C91D6BEAFA62B62";
        public const string CatalystDeployFireJadeSphere = "AFC5D5C7DA63D64BAAD55F787205B64F";
        public const string CatalystDeployWaterJadeSphere = "6D7EB5747873484DAF29C01FA51FE175";
        public const string CatalystDeployAirJadeSphere = "A3C8A55C3E530140A7F99AAA1CBB4E09";
        public const string CatalystDeployEarthJadeSphere = "A674D3E7BC0C4342BC7A4EF0EE8FF8F0";
        #endregion
        #region Warrior
        public const string WarriorSignetOfMight = "75EF160EAFC0394CACC436CF89819148";
        public const string WarriorSignetOfStamina = "1E720C4D42448D45BDCB6307869D3D66"; // not actually instant cast, just for reference
        public const string WarriorDolyakSignet = "D7F8FA5695F8714B99A51EE72EF6E178";
        public const string SpellbreakerWindsOfDisenchantment = "926917599B6B6E498AD62B812001B823";
        public const string BladeswornDragonspikeMine = "B5BE541DBF290E4AA381E1E52A2A3525";
        public const string BerserkerOutrage = "AC32B7F7BB281B4D94713F180C44F322";
        #endregion
        #region Revenant
        public const string RevenantTabletAutoHeal = "C715D15450E56E4998F9EB90B91C5668";
        public const string RevenantTabletVentarisWill = "D3FD740370D6B747B2DA4F8F065A0177";
        public const string RevenantProtectiveSolace = "63683ECFD27DA746BF0B16404D817978";
        public const string RevenantNaturalHarmony = "390487E4E5DFEA4C922AE3156A86D9DB";
        public const string RevenantNaturalHarmonyEnergyRelease = "E239BA17214B4943A4EC2D6B43F6175F";
        public const string RevenantPurifyingEssence = "D2B388E8DB721544A110979C3A384977";
        public const string RevenantEnergyExpulsion = "BE191381B1BC984A989D94D215DDEA1F";
        public const string RevenantInspiringReinforcement = "09171204F3936841813E518123E2F867";
        public const string RevenantInspiringReinforcementPart = "E6D6CD56B9A61E40A86F982C60421625";
        public const string RenegadeOrdersFromAbove = "F53F05F041957A47AD62B522FE030408";
        #endregion
        #region Guardian
        public const string GuardianGenericFlames = "EA98C3533AA46E4A9B550929356B7277"; // used e.g. with judges intervention, signet of judgment
        public const string GuardianGenericTeleport = "61C193EBA6526143BE01B80FF7C52217"; // usd e.g. with judges intervention, merciful intervention
        public const string GuardianGenericTeleport2 = "5E1717FB11CE1D44B59B36B6AD83B9CC"; // delayed, when reaching target? used with e.g. judges intervention, symbol of blades
        public const string GuardianRingOfWarding = "5A54592448836A46B30BC93A544A0E47";
        public const string GuardianLineOfWarding = "F8BE013B34366E458640B47BF43F257D";
        public const string GuardianWallOfReflection = "70FABE08FFCFEE48A7160A4D479E3F8B";
        public const string GuardianSanctuary = "A96093E9CB3D7F468C5235C81537301E";
        public const string GuardianShout = "122BA55CCDF2B643929F6C4A97226DC9"; // used with all shouts
        public const string GuardianSaveYourselves = "68F2C378E6C80548B5A3C89870C5DD86";
        public const string GuardianSmiteCondition = "8CBE6348BB8C9646B210AEE4BA9BCCA3"; // also lesser smite condition
        public const string GuardianContemplationOfPurity1 = "75D72E2DA47ECF47A6BD009B49B7C708";
        public const string GuardianContemplationOfPurity2 = "D0C072102FAA6A4EA8A16CB73F3B96DD"; // same as elementalist armor of earth
        public const string GuardianMercifulIntervention = "B45E7BD66E424A4CA695DE63DC13E93F"; // delayed, when reaching target?
        public const string GuardianSignetOfJudgement1 = "0AFA3936BD4D70458925660B54D47A90"; // happens twice?
        public const string GuardianSignetOfJudgement2 = "5EAC13DB0953EF4C9C5BCC10DB13C9C8";
        public const string GuardianShieldOfTheAvenger = "0885D553A0A0A341B4C31B7964243407";
        public const string FirebrandValiantBulwark = "1430A107F74F164387668DE2744A1528";
        public const string FirebrandMantraOfLiberationCone = "86CC98C9D9D2B64689F8993AB02B09E5";
        public const string FirebrandMantraOfLiberationSymbol = "A8E0E4C48848424D85503B674015D247";
        public const string FirebrandMantraOfLoreCone = "C2B55AE44B295849A2983745203D19A1";
        public const string FirebrandMantraOfLoreSymbol = "3D01B04C5700904BA279E9F135A3FAB3";
        public const string FirebrandMantraOfPotenceCone = "FB70E37EB3915F4BAB6E06E328832D1D";
        public const string FirebrandMantraOfPotenceSymbol = "95B52793B838524AB237EB9FED7834BF";
        public const string FirebrandMantraOfSolaceCone = "D2C28FC5AB651746914FC595D1591623";
        public const string FirebrandMantraOfSolaceSymbol = "8F0C77784AFD7F40B27446617DC05CDC";
        public const string FirebrandMantraOfTruthCone = "C2F283E74AC9024DBB865BA0F98AF20B";
        public const string FirebrandMantraOfTruthSymbol = "E33EA0A63898CA469F864EDA1336FCD0";
        public const string FirebrandMantraOfFlameCone = "9C2F9434C5827943A7F175EFF245D39F";
        public const string FirebrandMantraOfFlameSymbol = "AF2B09AC1145AA4880B967C32A11E81C";
        public const string FirebrandTomeOfJusticeOpen = "D573910FDB59434ABF6E7433061995BD";
        public const string FirebrandTomeOfResolveOpen = "39C1BD24ADA04C4788A99C7B0FD9B53F";
        public const string FirebrandTomeOfCourageOpen = "9EE3EAFEF333BE44AD8A7D234A1C3899";
        public const string DragonhunterTrapEffect = "CCF55B3EAA4D514BBB8340E01B6A1DEC";
        public const string DragonhunterTestOfFaith = "D7006AC247BBE74BA54E912188EF6B12";
        public const string DragonhunterFragmentsOfFaith = "C84644DDAA59E542989FDB98CD69134C";
        #endregion
        #region Engineer
        public const string EngineerHealingMist = "B02D3D0FF0A4FC47B23B1478D8E770AE"; // used with healing mist, soothing detonation
        public const string EngineerMagneticInversion = "F8BD502E5B0D9444AA6DC5B5918801EE";
        public const string EngineerMineInactive = "2EE26B8656BD424B9BF9A7EA4CB0AA06";
        public const string EngineerMineExplode1 = "885B7AAA68F09E48A926BFFE488DB5AD";
        public const string EngineerMineExplode2 = "1B3ACEE36F61DE42AB1C24BD33B5B5AD";
        public const string ScrapperThunderclap = "8C8E0AB8328CC1418F9A815E022E20B6"; // has owner, 5s duration
        public const string ScrapperThunderclapSpawn = "039F8B46E5595C4E9C2D52AA58FDD8B0"; // has owner, 1s duration
        public const string ScrapperFunctionGyro = "B4CA602E8A849F47BFC105C740005162"; // has owner, 5s duration
        public const string ScrapperFunctionGyroSpawn = "AC9C3749A245D741BC012CCAB224E37C"; // has owner, 1s duration
        public const string ScrapperBulwarkGyro = "611D90C69ECF8142BEEE84139F333388";
        public const string ScrapperPurgeGyro = "0DBE4F7115EADC4889F1E00232B2398B";
        public const string ScrapperDefenseField = "9E2D190A92E2B5498A88722910A9DECD";
        public const string ScrapperBypassCoating = "D2307A69B227BE4B831C2AA1DAAE646A"; // player is owner
        public const string HolosmithFlashSpark = "418A090D719AB44AAF1C4AD1473068C4";
        public const string HolosmitBladeBurstParticleAccelerator1 = "9D2A5C8FF1E67547A41B72D91F4355E7";
        public const string HolosmitBladeBurstParticleAccelerator2 = "5635C8217573C449905554A1BE38044B"; // happens at the same time as the other on Dst
        public const string MechanistCrashDownImpact = "80E1A21E07C03A43A21E470B95075A5A"; // happens at spawn location, no owner, no target, ~800ms after spawn
        public const string MechanistMechEyeGlow = "CDF749672C01964BAEF64CCB3D431DEE"; // used with e.g. crash down (delayed), crisis zone
        public const string MechanistDischargeArray = "5AAD58AD0259604AADA18AFD3AE0DDFD"; // likely the white radius indicator
        public const string MechanistCrisisZone = "956450E1260FB94B8691BC1378086250";
        public const string MechanistShiftSignet1 = "E1C1DD7F866B4149A1BADD216C9AA69D"; // happens twice, without owner at destination, with owner at origin?
        public const string MechanistShiftSignet2 = "DB22850AE209B34BBD11372F56D42D43";
        public const string MechanistOverclockSignet = "734834E7EB7CD74EB129ACBCE5C64C1D";
        #endregion
        #region Ranger
        public const string RangerLightningReflexes = "3CF1D1228CBC3740AA33EDA357EABED4";
        public const string RangerQuickeningZephyr = "B23157C515072E46B5514419B0F923B7";
        public const string RangerSignetOfRenewal = "EA9896A81DDF4843B18DBF6EE4F25E18";
        public const string RangerSignetOfTheHunt = "1A38CAE72C2F164BA3815441CA643A20";
        public const string RangerHunkerDown = "FAE87ED17A43E54AA3ABB3EAA2FDB754";
        public const string RangerBarrage1 = "A982C451890E704BA918B6959175D2A4"; // has owner, repeating, has duration
        public const string RangerBarrage2 = "90A4BD30E723C749A4E161C177F723A0"; // has owner, repeating
        public const string RangerBonfire = "E68388DE0702F44BB3F7E457EE9410AF"; // has owner
        public const string RangerFlameTrap = "371DA8262E27304BB1142A39FAED0731"; // has owner
        public const string RangerFrostTrap = "B2A5125C3FDDFB448F130488D32568C2"; // has owner, has duration
        public const string RangerFrostTrapTrigger = "A86A024FE2DDD147829551764894D716"; // has owner, no duration
        public const string RangerVipersNest = "1964816830EF7B47827298789EF7227B"; // has owner
        public const string RangerSpikeTrap = "E0223550EAC46A4C8CEC277CFC2B7927"; // has owner
        public const string RangerPoisonousCloud = "FDD0241186BAFE4AA451767D082D0BA9"; // has owner
        public const string DruidGlyphOfEquality = "9B8A1BE554450B4899B64F7579DF0A8C";
        public const string DruidGlyphOfEqualityCA = "74870558C43E4747955C573CAAC630A7";
        public const string DruidSeedOfLife = "19C4FA17A38E7E4780722799B48BF2BE"; // has owner
        public const string DruidSeedOfLifeBlossom = "666BCBD61F72E042B08EFE1C62555245"; // has owner, ~720ms delayed
        public const string DruidSublimeConversion1 = "5707A4A2BFFAD048BBDEC9CA0F2A61E1";
        public const string DruidSublimeConversion2 = "2F74AC468871444BB66AF5D8B25EC870";
        public const string SoulbeastEternalBond = "BF0A5B11A4076A4F98C6E1D655D507B1"; // has owner & target
        public const string UntamedMutateConditions = "D7DCD4ABF9E4A749950AF0175E02EA06";
        public const string UntamedUnnaturalTraversal = "8D36806A690A5442A983308EDCECB018";
        public const string UntamedVenomousOutburst = "60BE4692A455B140A05AD794BF4753F6";
        public const string UntamedRendingVines = "2C40B0741111444F98895A658A7F978F";
        public const string UntamedEnvelopingHaze = "F2B1B61970FC59418AC049BF3A07FFD4";
        #endregion
        #region Thief
        public const string ThiefTeleportTrail = "03A8D8B8F81FE94FB52FFE5F74F31C9E"; // likely the trail, used with infiltrators arrow, shadow step, infiltrators signet, measured shot
        // public const string ThiefTeleport = "1DEF5F2ECCF6CA4683ECC2DAED54726C"; // used with e.g. shadow shot, shadow strike
        public const string ThiefShadowstep = "2C40AE26C91BEE468E245D0009B590F9";
        public const string ThiefInfiltratorsSignet1 = "23284B87C26C9A41A887F410F930E1A2";
        public const string ThiefInfiltratorsSignet2 = "2C89A39F7B88614ABED16D4B5A5BD2EB";
        // public const string ThiefInfiltratorsSignetCollision = "70CFE546FA6A9B4E93BCAAF1ED1CD326"; // collision with shadow shot, shadow strike
        public const string ThiefSignetOfAgility = "BB5488951B60B546BB1BD5626DAE83E1";
        public const string ThiefSignetOfShadows = "14A5982DB277744CB928A4935555F563";
        public const string ThiefPitfallAoE = "7325E9B0DD2E914F9837E5FCFC740A95";
        // public const string ThiefThousandNeedlesAoECollision = "2125A13079C1C5479C150926EB60A15D"; // collision with shadow flare & other
        public const string ThiefThousandNeedlesAoE1 = "9AF103E33FC235498190448A9496C98A"; // ~280ms delayed
        public const string ThiefThousandNeedlesAoE2 = "B8DC8C6736C8E0439295A9DBBADC6296"; // ~280ms delayed
        public const string ThiefSealAreaAoE = "92A7634C2C7F2746AFDA88E1AD9AE886";
        public const string ThiefShadowRefuge = "1708CD9EDF419E41B40822C52E487E1E";
        public const string ThiefShadowPortalArmedInactive = "97AF46D347914E4FBDB37BFEC91C4711"; // unarmed portal has no effect, is this pov only?
        public const string ThiefShadowPortalActiveEntrance = "8535B486C1BCD24A87B7AC895FB26BB0";
        public const string ThiefShadowPortalActiveExit = "97AF46D347914E4FBDB37BFEC91C4711";
        public const string DeadeyeMercy = "B59FCEFCF1D5D84B9FDB17F11E9B52E6";
        public const string SpecterWellOfGloom1 = "F4260FA8B35EFC40B6990F5015E486A3"; // These 3 effects happen before the AoE, the placement can be moved with skill retargetting
        public const string SpecterWellOfGloom2 = "F5BD1268C23E0C4C85E7DFC927360EFE";
        public const string SpecterWellOfGloom3 = "1B9672DFA1F1D74DB11ADF3F0956FCF0";
        public const string SpecterWellOfGloom4 = "0FA258E85B5B2B4CBCF504F478558D3C"; // ~715ms delay - Using these two effects for the AoE placement (they happen after retargetting)
        public const string SpecterWellOfGloom5 = "63B5CB22E35C094E948DA101CA247B25"; // ~715ms delay
        public const string SpecterWellOfGloom6 = "D4CD6FCC1BABB042AA7E1779FF166F4B"; // ~960ms delay
        public const string SpecterWellOfBounty1 = "E452C4E8FD6B9A4F9C3659782ECEDEA3";
        public const string SpecterWellOfBounty2 = "704FF2761D3CA74AB7C12060F1D3D872"; // ~880ms delay
        public const string SpecterWellOfTears1 = "AEB43693461D1846BB70C2AEAB47EE2B";
        public const string SpecterWellOfTears2 = "21BF83968804A54DBF795C7A0AD385A5"; // ~1240ms delay
        public const string SpecterWellOfTears3 = "5CBC62CDE1F5204E8E63EA785CF81D59"; // ~1240ms delay
        public const string SpecterWellOfSilence1 = "51FCBBE627637D4C9EB9AC8A4CD216AC";
        public const string SpecterWellOfSilence2 = "15A73155534B204D8C9F97F5C8ED6DA1"; // ~440ms delay
        public const string SpecterWellOfSorrow1 = "036B9D5F24402C4A9ED923A0391E61C3";
        public const string SpecterWellOfSorrow2 = "5A74A8FADB71B249BD245E2FBE1D8952"; // ~1240ms delay
        public const string SpecterWellOfSorrow3 = "1B56F702912BE7428182CA57036AEE99"; // ~1240ms delay
        public const string SpecterShadowfall1 = "FB21A6E213C240459BD8E9524088FA66";
        public const string SpecterShadowfall2 = "D8E380E80E843A4092C8DD53C5A51F0F"; // ~880ms delay
        #endregion
        #region Fractals
        // Nightmare Fractal
        public const string SmallFluxBomb = "B9CB27D38747A94F817208835C41BB35";
        public const string ToxicSicknessIndicator = "3C98B00B9E795F4B8744E186EEEA7DF7";
        public const string ToxicSicknessOldIndicator = "B7DFF8C2A8DABD4C9C7F1D4CFC31FC8C";
        public const string ToxicSicknessNewIndicator = "71469269D3A1F9469D74CC96153264C0";
        public const string ToxicSicknessPuke = "E09CD66E417B59409401192201CE4B6E";
        public const string MAMAGrenadeBarrageIndicator = "8DDED161CE26964FA5952D821AD852F7";
        public const string NightmareMiasmaIndicator = "41883B3BD532124DACF93F7C2584E63C";
        public const string NightmareMiasmaDamage = "8A882A495793044D8C4A9AD9080283A7";
        public const string ArkkShieldIndicator = "5B1B9D29D6242F47A82743330AE4225B"; // Duration 7400
        public const string ArkkShieldIndicator2 = "1E267990C5098E49AFD5CFD5CA4E2B82"; // Duration 6400
        public const string SiaxNightmareHallucinationsSpawnIndicator = "0C284B1C201D1846B4D9F249AD01A5C6"; // siax src
        public const string SiaxVileSpitIndicator = "BC17A48E8DD2FF44864AA48A732BDC36";
        public const string SiaxVileSpitPoison = "6589BB8F4EE227428CC3DDDE84A67015";
        public const string CausticBarrageIndicator = "C910F1B11A21014AA99F24DBDFBF13FB";
        public const string CausticBarrageHitEffect = "CAF4E62C2C5CC04499657C2A6A78087B"; // 1000 duration - green explosion effect when orb lands - conflicts with player effects
        public const string VolatileExpulsionIndicator = "DCA047DBD6E90A41B46CDDCE5405E4BC"; // 300 - 400 duration
        public const string VolatileExpulsion2 = "F22E201EAF24DD42A43D297B2E83CC66"; // 0 duration
        public const string CascadeOfTormentRing0 = "EFF32973C7921F41AA3FD65745E06506";
        public const string CascadeOfTormentRing1 = "D919AC7D1B2ABD438F809B3B9DCE9226";
        public const string CascadeOfTormentRing2 = "A5D958EDAD66D7469CA40059915843CC";
        public const string CascadeOfTormentRing3 = "55FC7E1387EA2241B6538CAAB6017497";
        public const string CascadeOfTormentRing4 = "8CFFD69B25B7E844856A7D06D11332D5";
        public const string CascadeOfTormentRing5 = "D427C86A0E120F4A860F4570B354396D";
        public const string EnsolyssMiasmaDoughnut100_66 = "16B9D11838F68A4C8E477ED62F956226";
        public const string EnsolyssMiasmaDoughnut66_15 = "3AE042F82A10B84DB7487B0C0F4D2AB1";
        public const string EnsolyssMiasmaDoughnut15_0 = "AB294EC140644E48BC739B8E303D2762";
        public const string EnsolyssNightmareAltarShockwave = "AA31A20BDC52324B945FD660D60429EB";
        public const string EnsolyssNightmareAltarLightOrangeAoE = "66C6DEE334653342BDC578817254F7C8";
        public const string EnsolyssNightmareAltarOrangeAoE = "FA097ABEFB8CEF4B89EB12825EEE1FB9"; // same effect as Skorvald's Solar Bolt
        public const string EnsolyssArrow = "3D85505CEBCF0E4D8993625957405977";
        // Shattered Observatory Fractal
        public const string SolarBoltIndicators = "FA097ABEFB8CEF4B89EB12825EEE1FB9";
        public const string SkorvaldSolarBoltDamage = "49813989C508464B81FC45E6D24EA8C3";
        public const string KickGroundEffect = "47FE87414A88484AB05A84E1440F5FDD";
        public const string AoeIndicator130Radius = "8DDED161CE26964FA5952D821AD852F7";
        public const string MistBomb = "03FB41386DD2A54FA093795DF2870B7A";
        public const string ArtsariivBeamingSmileIndicator = "C047F635A01A4441945CD0EB85AD3D2C"; // no owner
        public const string ArtsariivBeamingSmile = "F01DC8CB8C6ACF4891BAE252FB950A24"; // no owner
        public const string ArtsariivAoeIndicator = "7948A94F5DB40D45B947F82804598027"; // no owner
        public const string ArtsariivAoeExplosion = "A09474AB8EBD2146B1A4299F3C918DB6"; // no owner
        public const string ArtsariivObliterateIndicator = "8938C846962EA045B5726F53C3ECF6AF"; // no owner
        public const string ArtsariivObliterateExplosion = "F2D51BED8214F1419A5D1684D2087093"; // no owner
        public const string ArtsariivBlackSmoke = "172355593E35D6479A742472E29CA150";
        public const string CorporealReassignmentDome = "1607FB8A58554A4E96E5AD04AF8E247A"; // owned by unknown agent
        public const string CorporealReassignmentExplosionDome = "5B8F0DCE941DF544AD0966F6158A5127";
        public const string CorporealReassignmentExplosion1 = "C93D2CA54BC7F84BBFA31B40DE056D21"; // owned by exploding player
        public const string CorporealReassignmentExplosion2 = "DAD653E8823274409610A732BE8FA188"; // owned by exploding player
        public const string HorizonStrikeArkk = "C5E4632E8131D342AA4F18222C68D8EB"; // owned by arkk
        // Silent Surf Fractal
        public const string FrighteningSpeedRedAoE = "96E8C6EA0D2FAF4C8F62B5C6CA4B611C";
        public const string AxeGroundAoE = "234949DB5ECD52409F6EDD601BBC0C19";
        public const string AxeGroundAoE2 = "CE91D2D4CD6C4141B3977FA70FFE05BB";
        public const string HarrowshotAoE = "3AE17719B3D7374BAC4899DA0A3E7DF9";
        #endregion
        #region Raids
        // Vale Guardian
        public const string ValeGuardianDistributedMagic = "43FD739499BB6040BBF9EEF37781B2CE";
        public const string ValeGuardianMagicSpike = "55364633145D264A934935C3F026B19F";
        // Escort Glenna
        public const string EscortOverHere = "64CD79C1A121EC42B1278DEF9280ED35";
        // Xera
        public const string XeraIntervention1 = "63C34770B4EFF64B8EAA21BB835BB560"; // 4294967295 duration - Src Player - Usable with ComputeDynamicEffectLifespan
        public const string XeraIntervention2 = "79EA3F01274B4F418B2C571BAE1B9E17"; // 0 duration - Src Player
        public const string XeraIntervention3 = "5FA6527231BB8041AC783396142C6200"; // 0 duration - No Src No Dst
        // Cairn
        public const string CairnDisplacement = "7798B97ED6B6EB489F7E33DF9FE6BD99";
        public const string CairnDashGreen = "D2E6D55CC94F79418BB907F063CBDD81";
        // Mursaat Overseen
        public const string MursaarOverseerDispelProjectile = "DE71A86A0867764BB5789265E8C0CF6A"; // No Src - Dst Jade Scout
        public const string MursaarOverseerProtectBubble = "17BC358A51ED2D43BF2ABE8AB642B86B"; // Src player
        public const string MursaarOverseerClaimMarker = "94F3501D777FAC439E78E143CE756B0A"; // No Src - No Dst
        public const string MursaarOverseerShockwave = "0F62A1315A00FC438B2F1273E6BC4054";
        // Broken King
        public const string BrokenKingNumbingBreachIndicator = "5341E83B29B534408E90DBE7BE6F452D";
        public const string BrokenKingNumbingBreachDamage = "1BF014091BFD1E40A11ED36B92601342";
        public const string BrokenKingHailstormGreen = "C97A7665B2AA6C4482026D4F2562E25E";
        public const string BrokenKingIceBreakerGreenExplosion = "957ADB83D139704F8CB865E86E389228";
        public const string BrokenKingKingsWrathConeAoEIndicator = "FA4B726574C96E489D73529CFE390D3D"; // Currently unused, we don't know how to determinate the aoe size
        public const string BrokenKingKingsWrathConeAoEDamage = "22AC6BFC0B06C1459DFEF1E380F50165"; // Currently unused, we don't know how to determinate the aoe size
        // Dhuum
        public const string DhuumScytheSwingIndicator = "91A23D51294E80458BE9C3C89A2ED138"; // 1200 duration
        public const string DhuumScytheSwingDamage = "C79F5D95E11070448A39ACD7F6C5D0D3"; // 0 duration
        public const string DhuumCullAoEIndicator = "1BB71ED45AF4354AB65BBEB976E8CFEE"; // 0 duration
        public const string DhuumCullCracksIndicator = "F28528CBE08E0D43B3227A157CD1CCF2"; // dynamic duration, earlier cracks have longer duration than last ones.
        public const string DhuumCullCracksDamage = "13B5022FBF7D884C9AA9ED667FEEC22F"; // 0 duration
        public const string DhuumDeathMarkFirstIndicator = "6A0D725CD03D8D48BEA939CD1BBA7A9A"; // 2000 duration - Soul split warning indicator
        public const string DhuumDeathMarkSecondIndicator = "4BA74BA044B7BD4BB1E3392641078D97"; // 1000 duration - Hit indicator (black smoke)
        public const string DhuumDeathMarkDeathZone = "B8F90FE6AF4F2A4C84D349861A098392"; // 120000 duration
        public const string DhuumSuperspeedOrb = "8F89945581099142B598977188BAC8E1"; // max duration - has end effect
        public const string DhuumConeSlash = "21BA95CC014CC944A71E2A6FB28D9A86";
        // CA
        public const string CAArmSmash = "B1AAD873DB07E04E9D69627156CA8918";
        // Qadim
        public const string QadimCMIncinerationOrbs = "F0EC05F2019BD3429E7F8349BEB5A1DF"; // 2600 duration - 180 corner orb - 540 central orb
        public const string QadimPyresIncinerationOrbs = "D3D9E94418D8094BAE0E0C510DDF2A91"; // 2300 duration - 240 radius
        public const string QadimInfernoAoEs = "37DF91103EC45240AA7910575F1FC55F"; // On non static platform - 3000 duration - 150 radius
        public const string QadimJumpingBlueOrbs = "9FE9CEE3B3B1A743B769D16B196AD45D";
        public const string QadimPlatformStartsOrEndsMoving = "98891680AFB80A4E9CAFCCBD1662DF88";
        // Sabir
        public const string SabirFlashDischarge = "40818C8E9CC6EF4388C2821FCC26A9EC";
        // Qadim the Peerless
        public const string QadimPeerlessRainOfChaos = "D8259BFD4E6B8348AF15D862F7DBC8FA";
        public const string QadimPeerlessResidualImpactFireAoE = "EFAC2FC0F661404D84F0291CAB76FF0E";
        public const string QadimPeerlessChaosCalledElectricShark = "7A5A2002C855A440BCC22E2C76B0C405";
        public const string QadimPeerlessForceOfHavoc1 = "5F3B01764915FD41A02B2FBAD788651B"; // 2000 duration
        public const string QadimPeerlessForceOfHavoc2 = "B2396CC1F4A73B4EAEA86F66978DC895"; // 1000 duration
        public const string QadimPeerlessForceOfHavoc3 = "A2E91B50829AB64097D217E468189F52"; // 22400 duration
        public const string QadimPeerlessEtherStrikesOrbs = "625838F1175E25459A5293CA6C911290"; // 1500 duration
        public const string QadimPeerlessEtherStrikesAoEs = "A89E436B20CFC142B159F3D2195F75AE"; // 0 duration
        public const string QadimPeerlessShowerOfChaosAoE = "845D252D05631740B3B2309457FB4338"; // 5000 duration
        public const string QadimPeerlessShowerOfChaosExplosion = "3AAEF82C63C4424FAA0F55CD02256E00";
        public const string QadimPeerlessShowerOfChaosOnPlayer1 = "27B83B9DF241F94DB16414852EA68354";
        public const string QadimPeerlessShowerOfChaosOnPlayer2 = "0BA57434DC93604096B870FB98B3C4F1"; // Src Qadim
        public const string QadimPeerlessMeteorIllusion1 = "00C4F7C59E4D8449B565CC00FC30D9DD"; // 5000 duration
        public const string QadimPeerlessMeteorIllusion2 = "ED04DA4F2B31D74CBEF501CAFFDAFAAD"; // 4294967295 duration - Usable with ComputeDynamicEffectLifespan
        public const string QadimPeerlessBrandstormLightning1 = "C5B4846F6A548D47B0856AA8A2CE283C"; // 0 duration
        public const string QadimPeerlessBrandstormLightning2 = "995E6709BB16B44DBABCC707F10E5345"; // 3000 duration
        public const string QadimPeerlessMagmaWarningAoE = "E269977C2FC9474EAAD1051CDAFAD653"; // 4000 duration - Src player
        public const string QadimPeerlessMagmaLandingExplosion = "6617FA23565EE646ADAA7A646C895927"; // 1000 duration - No Src
        public const string QadimPeerlessMagmaDamagingAoE = "BABE69EC5AC7AF48A2F14A9FB8920C7F"; // 600000 duration - Src Qadim
        #endregion
        #region Strikes
        // Freezie
        public const string FreezieFrozenPatch = "2CE301ED692ACA4E964BFDFEED9D055E"; // 30000 duration
        public const string FreezieOrangeAoE120 = "0760BCD6779C0248B480E59D41E785B4"; // Has multiple durations
        public const string FreezieDoughnutRing = "3627917E07E3344EB97B795BE437DDF0"; // 10000 duration
        // Boneskinner
        public const string GraspAoeIndicator = "B9B32815D670DC4E8B8CF71E92A9FFD5"; // Orange aoe indicator
        public const string GraspClaws1 = "75B096EF78F3AB4CB1D05BAE9CA3235C"; // One is the claw, the other the red aoe indicator
        public const string GraspClaws2 = "4C290CBF719C0E448391E9415EF307A7";
        public const string CascadeBonesEffect = "3E370A8629BB134F83902A8F14B99CCE";
        public const string CascadeAoEIndicator1 = "4692619BBBFE6346B409C4A2B93B9BA6";
        public const string CascadeAoEIndicator2 = "8E8592D62B48834180C66FE806278C86";
        public const string CascadeAoEIndicator3 = "89CB4BCA7B012244B0864DFAD7E9F3AC";
        public const string CascadeAoEIndicator4 = "965355FD1C53F24085A9C422B8333780";
        public const string CascadeAoEIndicator5 = "F26A2240C0F1E24E81EAEFDE64EFA3BF";
        // Ankka
        public const string DeathsEmbrace = "4AC57C4159E0804D8DBEB6F0F39F5EF3";
        public const string DeathsHandOnPlayerCM = "9A64DC8F21EEC046BA1D4412863F2940";
        public const string DeathsHandByAnkkaRadius380 = "651CA3631083EF4A81159989AB58F787";
        public const string DeathsHandByAnkkaRadius300 = "805E3CE2A313584797C614082C44197D";
        // Kaineng Overlook
        public const string KainengOverlookSharedDestructionGreen = "BFFF308926A8B647A729197D364C1095"; // 6250 duration
        public const string KainengOverlookSharedDestructionGreenSuccess = "F2D28874FE961C40837B97DA1159A541";
        public const string KainengOverlookSharedDestructionGreenFailure = "C460400C2CADAA4880CD74F95D011A36";
        public const string KainengOverlookDragonSlashBurstRedAoE1 = "4BE73D3E16294149A1829230F9E1F363"; // 208000 duration
        public const string KainengOverlookDragonSlashBurstRedAoE2 = "E9DDC9F070B9514F8B4C6F5D428356E4"; // 0 duration - probably the explosion effect on player hit
        public const string KainengOverlookJadeMine1 = "DE7F3CF2B6C1794F97F5DC6F6B1C5F7C"; // 4294967295 duration
        public const string KainengOverlookJadeMine2 = "FAAC4919C404C945ACEF2ABE3C8CCF08"; // 2000 duration
        public const string KainengOverlookSniperRicochetBeamCM = "A5C623040E6810468F2C9E518DB09D83"; // 10000 duration
        public const string KainengOverlookSmallOrangeAoE = "34724E94CD4E974C95A8D9D1D1162658";
        public const string KainengOverlookTargetedExpulsion = "67C0C333F91A5443BA894BEE5E88E202"; // 5000 duration
        public const string KainengOverlookJadeLobPulsingGreen = "D5CD93218B9CBE4B93B6B5D54ED71273";
        public const string KainengOverlookJadeLobSomething = "D36F4CE327D701449358B19E23C8AED0";
        public const string KainengOverlookEnforcerRushingJusticeFlames = "0E5D42F70AF65E4ABBB7EE94C3D5BD1C"; // 4294967295 duration
        public const string KainengOverlookEnforcerMiddleAoE = "BA8654BD3D252C4B9A170EE404FBEA15"; // 1500 duration
        public const string KainengOverlookEnforcerMiddleRedAoE = "C0F88EBEA179344092D4BB193A741F1D"; // 0 duration
        public const string KainengOverlookEnforcerOrbsAoE = "766B7DACFC18974B8F6AA46BCD779563"; // 2708 duration
        public const string KainengOverlookMindbladeRainOfBladesFirstOrangeAoEOnPlayer = "D7FB6DB480A6D14DB4561E03172B705D"; // 8000 duration
        public const string KainengOverlookMindbladeRainOfBladesConsecutiveOrangeAoEOnPlayer = "D4089DD8E0040146B3899EB2955AAE87"; // 2000 duration
        public const string KainengOverlookMindbladeRainOfBladesRedAoECM = "6814DF4DB1EB4541996056FF4E805AC4";
        public const string KainengOverlookVolatileExpulsionAoE = "A673F658E9B67C41AD469BAD8E7ACEA7";
        public const string KainengOverlookVolatileBurstAoE = "6C2F5A0A632627419B77D52D8CC9E4DB";
        public const string KainengOverlookJadeBusterCannonWarning = "C047F635A01A4441945CD0EB85AD3D2C";
        public const string KainengOverlookEnforcerHeavensPalmAoE = "BDF708225224C64183BA3CE2A609D37F"; // 5000 duration
        public const string KainengOverlookEnforcerHeavensPalmAnimation = "92F0566A1A0A9E4B919C796DB434052C"; // Should be the actual palm
        public const string KainengOverlookRitualistSpiritualLightningAoE = "3AEC5A729A1D624B80CABCFDA11D82C6";
        public const string KainengOverlookDragonSlashWaveIndicator = "CB877C57D1423240BACDF8D6B52A440F";
        public const string KainengOverlookStormOfSwordsIndicator = "F019EA6ADC183B4599FDF7E67071E181";
        // Harvest Temple
        public const string HarvestTemplePurificationLightningOfJormag = "ADDDB6E725094240845270262E59F2BD";
        public const string HarvestTemplePurificationStormfall = "F5A9E487E2B3A64A83661D87DE1CAF1F";
        public const string HarvestTemplePurificationZones = "D5B07DF36991DD48B64AC403EFAA6F9F";
        public const string HarvestTemplePurificationFlamesOfPrimordus = "D49EB86EB17A0D4793768B19978C1B2C";
        public const string HarvestTemplePurificationBeeLaunch = "73FE43AEE78ADC4B9527DF683481984F";
        public const string HarvestTemplePurificationPoolOfUndeath = "CCBA0AD77B52774DA48EE37AED9108F4"; // duration 21000
        public const string HarvestTemplePurificationWaterProjectiles = "F8F9628F58DA09438574D66424399151";
        public const string HarvestTempleJormagFrostMeteorIceField = "40C38381C43B184A885960714F9388D5";
        public const string HarvestTempleJormagGraspOfJormagIndicator = "3A39297503D1C542AFC16CB5C1D2D3F7"; // duration 2000
        public const string HarvestTemplePrimordusLavaSlamIndicator = "EDA1C033B296404BA403E106F6F258C0";
        public const string HarvestTemplePrimordusGeneralJawAttack = "160CBAE34F4A2941885EB3F3CD6BB0C3";
        public const string HarvestTemplePrimordusJawsOfDestructionIndicator = "4D8CA1836969BD4BBF345719576ACAAF";
        public const string HarvestTemplePrimordusJawsOfDestructionDamage = "363F831AD54DB7489DFDC31F659B222E";
        public const string HarvestTempleKralkatorrikBeamIndicator = "4ACBA11BFAC6B940BF6FD11CB332FFB8"; // This is the effect for the AoE indicator, the actual puddles are a different effect
        public const string HarvestTempleKralkatorrikBeamAoe = "8B55EBC6025EB3429D464EDA5710E419"; // This is the effect for the actual circular puddles
        public const string HarvestTempleKralkatorrikCrystalBarrageImpact = "32B9E497929F054E8633EF013583E20C";
        public const string HarvestTempleMordremothPoisonRoarIndicator = "171A7BD24B5D0B4BA3770FF8A6A37EC0";
        public const string HarvestTempleMordremothPoisonRoarImpact = "E500544171F13643899C178EC3FB38A9";
        public const string HarvestTempleMordremothShockwave1 = "3DFB20FECCAF794EA194E1F93CB0146A"; // duration 0
        public const string HarvestTempleMordremothShockwave2 = "17E4CA4ED7CAF843895AD75F2D45D9A6"; // duration 0
        public const string HarvestTempleZhaitanPutridDelugeImpact = "FE8B96A200376B4BA75297FF2367C5C4";
        public const string HarvestTempleZhaitanPutridDelugeAoE = "82A8BC954DD69E4DBBF526EE1C6A3E74";
        public const string HarvestTempleZhaitanTailSlamImpact = "7D0FBDEC2B1DEF4B8BC0FD6A5BFD3705";
        public const string HarvestTempleZhaitanScreamIndicator = "12B49E1A9D034F45A3BD754331418F9B";
        public const string HarvestTempleSpreadNM = "F39933B190100B4C87E808EF8E6C654A";
        public const string HarvestTempleSpreadCM = "BDF708225224C64183BA3CE2A609D37F";
        public const string HarvestTempleRedPuddleSelectNM = "0CD6F76C1BF9C049A2FCE4D86CB46475";
        public const string HarvestTempleRedPuddleNM = "60EE2CA1A95C514F8A325B654E0D9478";
        public const string HarvestTempleRedPuddleSelectCM = "61C1CD7E89346843B04FCE613EC487AA";
        public const string HarvestTempleRedPuddleCM = "FF0A7D32AD894E45993BE5ED748BF484";
        public const string HarvestTempleGreen = "72EE47DE4F63D3438E193578011FBCBF";
        public const string HarvestTempleFailedGreen = "F4F80E9AF2B6AF49AFE46D8CF797B604";
        public const string HarvestTempleOrbExplosion = "B329CFB6B354C148A537E114DC14CED6";
        public const string HarvestTemplePurificationOrbSpawns = "4F982CD060507C44A25844BF0ADFCB54";
        public const string HarvestTempleVoidPoolOrbGettingReadyToBeDangerous = "D11320204E28E643A48469AA8E4845BA";
        public const string HarvestTempleInfluenceOfTheVoidPool = "912F68E45158C14E9A30D6011B7B0C7F";
        public const string HarvestTempleSooWonClaw = "CB877C57D1423240BACDF8D6B52A440F";
        public const string HarvestTempleSooWonVoidOrbs1 = "F6964A4DE51DF04CA7E0F011BEE7D854"; // 2080 duration - these are the orbs spawning just before the claw swipe
        public const string HarvestTempleSooWonVoidOrbs2 = "88E9C3112BF6DA4486845A0433782E9C"; // 2080 duration - these are the orbs spawning just before the claw swipe
        public const string HarvestTempleTormentOfTheVoidClawIndicator = "3F24896D3EF8D5459B399DAC8D0AD150"; // AoE indicator for bouncing orbs after Soo-Won's Claw Slap attack
        public const string HarvestTempleScalableOrangeAoE = "C1A523D71A841048897211B1020B8D95"; // Generic orange aoe - variable radius - variable duration
        public const string HarvestTempleTsunamiIndicator = "8B0EBA3241E1ED469DAC7AFD4E385FF2";
        public const string HarvestTempleTsunami1 = "8F96447526A09B4F8545CBEA1B0046D4"; // There are multiple effects when the Tsunami goes off
        public const string HarvestTempleTsunami2 = "C2CF236673BC0141B6EE5A918869728A"; // There are multiple effects when the Tsunami goes off
        public const string HarvestTempleTsunami3 = "E4700E828E058649B9B94F170DEF8659"; // There are multiple effects when the Tsunami goes off
        public const string HarvestTempleSooWonTsunamiSlamIndicator = "0D594F550B0BF043B0B299FC26A8463B";
        public const string HarvestTempleTailSlamIndicator = "49BD7FF8309E4047B4D17C83E660A461";
        public const string HarvestTempleVoidBrandbomberBrandedArtillery = "3ED61C8A1C2E594A8AD2E2E69AF16322"; // duration 2310
        public const string HarvestTempleVoidExplosion = "A478BD35F568974091FC99670B5A9700"; // Last Laugh AoE - Same effect for all sizes - 2050 duration
        public const string HarvestTempleVoidStormseerIceSpike = "2B9395E6BDE51E4C90AD3A9CA78FBCE7"; // duration 5000
        public const string HarvestTempleVoidStormseerIceSpikeIndicator = "BFF48E3A55F94E48ACE3820EEB4B0E71"; // duration 1750
        public const string HarvestTempleVoidGiantRottingBileIndicator = "912F68E45158C14E9A30D6011B7B0C7F"; // duration 1400
        public const string HarvestTempleVoidGiantRottingBileDamage = "73931DCBD7D25E4FAE930BA1B896D07E"; // duration 10000
        public const string HarvestTempleVoidTimecasterGravityCrushIndicator = "1344E9C82608BB47AADA2B850DB7DEF7"; // duration 1600
        public const string HarvestTempleVoidTimecasterNightmareEpoch = "7D94F6283F23FC4A839D0F8EEE0549C5"; // duration 10000
        public const string HarvestTempleVoidSaltsprayDragonHydroBurstWhirlpools = "8198E9B46FF7AB438927745B76759A7F"; // duration 3000
        public const string HarvestTempleVoidSaltsprayDragonCallLightning = "6724030E0E64CA4E8D947A5CCFA8188E"; // duration 0 - using this for mechanic instead of orange aoe effect
        public const string HarvestTempleVoidSaltsprayDragonFrozenFuryCone = "ECD550A176BD9249A6925E8C2DD0CA30";
        public const string HarvestTempleVoidSaltsprayDragonFrozenFuryRectangle = "56E2F67D3D550442A7BE11E85FDDE65D";
        public const string HarvestTempleVoidSaltsprayDragonRollingFlames = "084A4E29CD66A04C9ECDB8033EFFE6A1";
        public const string HarvestTempleVoidSaltsprayDragonShatterEarth = "5978C1D3AE4BCD4BAB286BF4FD8B24E9";
        public const string HarvestTempleVoidGoliathGlacialSlam = "B28E156F3C93ED4B842B4479ABF5F5C1"; // duration 5000
        public const string HarvestTempleVoidObliteratorChargeIndicator = "8F65EC18AC385342982BCB28F9742B37"; // duration 1000
        public const string HarvestTempleVoidObliteratorWyvernBreathIndicator = "8A1D085CA69E8A42A52196C99AE86CAF"; // duration 3400
        public const string HarvestTempleVoidObliteratorWyvernBreathFire = "453283E51FF9EF489980B6F0208F5F43"; // duration 30000
        public const string HarvestTempleVoidObliteratorFirebomb = "D2E7228A6225FB44911507A45EF2CCEC"; // duration 21000
        public const string HarvestTempleVoidObliteratorShockwave = "4254DCF4AF72FF4A83847908DA98E427"; // duration 0, should probably be 2900
        // Old Lion's Court
        // Cosmic Observatory
        public const string CosmicObservatoryDemonicBlastSliceIndicator = "A21A92783688A847963B86E96B8CC9BE";
        public const string CosmicObservatoryDemonicBlastDagdaEffect1 = "D03CDF37E0AC8246ABD4E741ADD61427"; // 0 duration no effect end
        public const string CosmicObservatoryDemonicBlastDagdaEffect2 = "3A19BC0143715C419504C25EA0B7ADFE"; // 0 duration no effect end
        public const string CosmicObservatoryDemonicFever = "BDF708225224C64183BA3CE2A609D37F";
        public const string CosmicObservatorySharedDestructionCosmicMeteorGreen = "3EEDE16455C8C8449237BCC77F107548";
        public const string CosmicObservatorySharedDestructionCosmicMeteorGreenEnd = "FED0256743CC534695F30EB3655933AD";
        public const string CosmicObservatorySpinningNebula = "8196855C5F76874CAF1DB683BD163811";
        public const string CosmicObservatoryShootingStarsGreenArrow = "046AFA0B20E07447BDBB94A03FCA2662"; // 9000 duration
        public const string CosmicObservatoryDemonicPoolsIndicator = "52C0855CB838424D91343DD5C176EC2E"; // 3000 duration
        public const string CosmicObservatoryDemonicPoolsDamage = "F9020791BEE9BC41B6A17955120EDD32"; // 20000 duration - can end early
        public const string CosmicObservatoryRainOfComets = "43A9C4FBF0628A4C9D38084854653547";
        public const string CosmicObservatoryPlanetCrash = "B4529A29DF12BA4D913973FFAAE22926";
        // Temple of Febe
        public const string TempleOfFebeCerusGreen = "0651E35503F642419A21378FBD29F777"; // Owner Cerus Target Player
        public const string TempleOfFebeCerusGreen2 = "E7E95B11D4AAD2469DD2FD0AF9631ED5"; // Owner Cerus Target Player
        public const string TempleOfFebeRegretGreen = "015E5CF598A13D4F8D6CCFD66643525F"; // Owner Embodiment of Regret Target Player
        public const string TempleOfFebeGreenFailure = "217289F02841EE498070E653723A3991"; // Owner Cerus/Embodiment Target None
        public const string TempleOfFebeGreenSuccess = "D7C64FEAB21040428D14CC3B2B4018F0"; // Owner Cerus/Embodiment Target None
        public const string TempleOfFebeWailOfDespair = "9EDCB6E2E3C11448B37F7D07B6B2D4F5"; // Owner Cerus/Embodiment Target Player Duration 5000
        public const string TempleOfFebeWailOfDespair2 = "00E0C7FC6A454B43B0FB93A6CA7BE83F"; // No Owner Target Player Duration 0
        public const string TempleOfFebeWailOfDespairEmpowered = "246326728CB3704C93FEA75C402D65CA"; // Owner Cerus/Embodiment Target Player Duration 5000
        public const string TempleOfFebePoolOfDespair = "3CF93AB93143B64A879D1D63FBA9282A"; // Owner Cerus/Embodiment Duration 60000
        public const string TempleOfFebePoolOfDespair2 = "4F982CD060507C44A25844BF0ADFCB54"; // No Owner Target Player Duration 0
        public const string TempleOfFebePoolOfDespairEmpowered = "24E40E1F15BE2142B61F3568D23AE799"; // Owner Cerus/Embodiment Duration 999000
        public const string TempleOfFebeEnviousGazeIndicator = "62B3766CDF54BD4EA964DAA462954A4A"; // Duration 1500
        public const string TempleOfFebeEnviousGaze1 = "246ECEBC28173B498B9A6886D7937D59"; // Duration 12500
        public const string TempleOfFebeEnviousGaze2 = "DA42718917F2304FBA10AF6898217788"; // Duration 12500
        public const string TempleOfFebeEnviousGaze3 = "B77A06C842511949889877EDC1448D49"; // Duration 11000
        public const string TempleOfFebeEnviousGaze4 = "0D2192849D53B4469F56B1C74542DBE9"; // Duration 11000
        public const string TempleOfFebeMaliciousIntentTether = "518369328A12B74EAC49702A785FBA19"; // Duration 0
        #endregion
    }
}
