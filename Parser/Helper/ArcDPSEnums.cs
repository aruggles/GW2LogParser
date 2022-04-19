﻿using System;

namespace Gw2LogParser.Parser.Helper
{
    public static class ArcDPSEnums
    {
        // Activation
        public enum Activation : byte
        {
            None = 0,
            Normal = 1,
            Quickness = 2,
            CancelFire = 3,
            CancelCancel = 4,
            Reset = 5,

            Unknown
        };

        internal static Activation GetActivation(byte bt)
        {
            return bt < (byte)Activation.Unknown ? (Activation)bt : Activation.Unknown;
        }

        // Buff remove
        public enum BuffRemove : byte
        {
            None = 0,
            All = 1,
            Single = 2,
            Manual = 3,

            Unknown
        };

        internal static BuffRemove GetBuffRemove(byte bt)
        {
            return bt < (byte)BuffRemove.Unknown ? (BuffRemove)bt : BuffRemove.Unknown;
        }

        // Buff cycle
        public enum BuffCycle : byte
        {
            Cycle, // damage happened on tick timer
            NotCycle, // damage happened outside tick timer (resistable)
            NotCycle_NoResit, // BEFORE MAY 2021: the others were lumped here, now retired
            NotCycle_DamageToTargetOnHit, // damage happened to target on hiting target
            NotCycle_DamageToSourceOnHit, // damage happened to source on hiting target
            NotCycle_DamageToTargetOnStackRemove, // damage happened to target on source losing a stack
            Unknown
        };

        internal static BuffCycle GetBuffCycle(byte bt)
        {
            return bt < (byte)BuffCycle.Unknown ? (BuffCycle)bt : BuffCycle.Unknown;
        }

        // Result

        public enum PhysicalResult : byte
        {
            Normal = 0,
            Crit = 1,
            Glance = 2,
            Block = 3,
            Evade = 4,
            Interrupt = 5,
            Absorb = 6,
            Blind = 7,
            KillingBlow = 8,
            Downed = 9,
            BreakbarDamage = 10,
            Activation = 11,

            Unknown
        };

        internal static PhysicalResult GetPhysicalResult(byte bt)
        {
            return bt < (byte)PhysicalResult.Unknown ? (PhysicalResult)bt : PhysicalResult.Unknown;
        }

        public enum ConditionResult : byte
        {
            ExpectedToHit = 0,
            InvulByBuff = 1,
            InvulByPlayerSkill1 = 2,
            InvulByPlayerSkill2 = 3,
            InvulByPlayerSkill3 = 4,
            //BreakbarDamage = 5,

            Unknown
        };
        internal static ConditionResult GetConditionResult(byte bt)
        {
            return bt < (byte)ConditionResult.Unknown ? (ConditionResult)bt : ConditionResult.Unknown;
        }

        // State Change    
        public enum StateChange : byte
        {
            None = 0,
            EnterCombat = 1,
            ExitCombat = 2,
            ChangeUp = 3,
            ChangeDead = 4,
            ChangeDown = 5,
            Spawn = 6,
            Despawn = 7,
            HealthUpdate = 8,
            LogStart = 9,
            LogEnd = 10,
            WeaponSwap = 11,
            MaxHealthUpdate = 12,
            PointOfView = 13,
            Language = 14,
            GWBuild = 15,
            ShardId = 16,
            Reward = 17,
            BuffInitial = 18,
            Position = 19,
            Velocity = 20,
            Rotation = 21,
            TeamChange = 22,
            AttackTarget = 23,
            Targetable = 24,
            MapID = 25,
            ReplInfo = 26,
            StackActive = 27,
            StackReset = 28,
            Guild = 29,
            BuffInfo = 30,
            BuffFormula = 31,
            SkillInfo = 32,
            SkillTiming = 33,
            BreakbarState = 34,
            BreakbarPercent = 35,
            Error = 36,
            Tag = 37,
            BarrierUpdate = 38,
            StatReset = 39,
            Extension = 40,
            APIDelayed = 41,
            Unknown
        };

        internal static StateChange GetStateChange(byte bt)
        {
            return bt < (byte)StateChange.Unknown ? (StateChange)bt : StateChange.Unknown;
        }
        // Breakbar State

        public enum BreakbarState : byte
        {
            Active = 0,
            Recover = 1,
            Immune = 2,
            None = 3,
            Unknown
        };
        internal static BreakbarState GetBreakbarState(int value)
        {
            return value < (int)BreakbarState.Unknown ? (BreakbarState)value : BreakbarState.Unknown;
        }

        // Buff Formula

        // this enum is updated regularly to match the in game enum. The matching between the two is simply cosmetic, for clarity while comparing against an updated skill defs
        public enum BuffStackType : byte
        {
            StackingConditionalLoss = 0, // the same thing as Stacking
            Queue = 1,
            CappedDuration = 2,
            Regeneration = 3,
            Stacking = 4,
            Force = 5,
            Unknown,
        };
        internal static BuffStackType GetBuffStackType(byte bt)
        {
            return bt < (byte)BuffStackType.Unknown ? (BuffStackType)bt : BuffStackType.Unknown;
        }

        public enum BuffAttribute : short
        {
            None = 0,
            Power = 1,
            Precision = 2,
            Toughness = 3,
            Vitality = 4,
            Ferocity = 5,
            Healing = 6,
            Condition = 7,
            Concentration = 8,
            Expertise = 9,
            Armor = 10,
            Agony = 11,
            StatInc = 12,
            FlatInc = 13,
            PhysInc = 14,
            CondInc = 15,
            PhysRec = 16,
            CondRec = 17,
            AttackSpeed = 18,
            //SiphonInc = 19,
            SiphonRec = 20,
            //
            Unknown = short.MaxValue,
            //
            /*ConditionDurationIncrease = 24,
            RetaliationDamageOutput = 25,
            CriticalChance = 26,
            PowerDamageToHP = 34,
            ConditionDamageToHP = 35,
            GlancingBlow = 47,
            ConditionSkillActivationFormula = 52,
            ConditionDamageFormula = 54,
            ConditionMovementActivationFormula = 55,
            EnduranceRegeneration = 61,
            IncomingHealingEffectiveness = 65,
            OutgoingHealingEffectivenessFlatInc = 68,
            OutgoingHealingEffectivenessConvInc = 70,
            RegenerationHealingOutput = 71,
            ExperienceFromKills = 76,
            GoldFind = 77,
            MovementSpeed = 78,
            KarmaBonus = 87,
            SkillCooldown = 96,
            MagicFind = 92,
            ExperienceFromAll = 100,
            WXP = 112,*/
            // Custom Ids, matched using a very simple pattern detection, see BuffInfoSolver.cs
            ConditionDurationInc = -1,
            DamageFormulaSquaredLevel = -2,
            CriticalChance = -3,
            StrikeDamageToHP = -4,
            ConditionDamageToHP = -5,
            GlancingBlow = -6,
            SkillActivationDamageFormula = -7,
            DamageFormula = -8,
            MovementActivationDamageFormula = -9,
            EnduranceRegeneration = -10,
            HealingEffectivenessRec = -11,
            HealingEffectivenessFlatInc = -12,
            HealingEffectivenessConvInc = -13,
            HealingOutputFormula = -14,
            ExperienceFromKills = -15,
            GoldFind = -16,
            MovementSpeed = -17,
            KarmaBonus = -18,
            SkillRechargeSpeedIncrease = -19,
            MagicFind = -20,
            ExperienceFromAll = -21,
            WXP = -22,
            SiphonInc = -23,
            PhysRec2 = -24,
            CondRec2 = -25,
        }
        internal static BuffAttribute GetBuffAttribute(short bt)
        {
            return Enum.IsDefined(typeof(BuffAttribute), bt) ? (BuffAttribute)bt : BuffAttribute.Unknown;
        }

        public enum BuffCategory : byte
        {
            Boon = 0,
            Any = 1,
            Condition = 2,
            Food = 4,
            Upgrade = 6,
            Boost = 8,
            Trait = 11,
            Enhancement = 13,
            Stance = 16,
            Unknown = byte.MaxValue
        }
        internal static BuffCategory GetBuffCategory(byte bt)
        {
            return Enum.IsDefined(typeof(BuffCategory), bt) ? (BuffCategory)bt : BuffCategory.Unknown;
        }
        // WIP
        public enum SkillAction : byte
        {
            EffectHappened = 4,
            AnimationCompleted = 5,
            Unknown = byte.MaxValue,
        }
        internal static SkillAction GetSkillAction(byte bt)
        {
            return Enum.IsDefined(typeof(SkillAction), bt) ? (SkillAction)bt : SkillAction.Unknown;
        }

        // Friend of for

        public enum IFF : byte
        {
            Friend = 0,
            Foe = 1,

            Unknown
        };

        internal static IFF GetIFF(byte bt)
        {
            return bt < (byte)IFF.Unknown ? (IFF)bt : IFF.Unknown;
        }

        // Custom ids
        private const int DummyTarget = -1;
        private const int HandOfErosion = -2;
        private const int HandOfEruption = -3;
        private const int PyreGuardianProtect = -4;
        private const int PyreGuardianStab = -5;
        private const int PyreGuardianRetal = -6;
        private const int QadimLamp = -7;
        private const int AiKeeperOfThePeak2 = -8;
        private const int MatthiasSacrifice = -9;
        private const int BloodstoneFragment = -10;
        private const int BloodstoneShard = -11;
        private const int ChargedBloodstone = -12;
        private const int PyreGuardianResolution = -13;
        private const int CASword = -14;
        private const int SubArtsariiv = -15;


        //

        public enum TrashID : int
        {
            // VG
            Seekers = 15426,
            RedGuardian = 15433,
            BlueGuardian = 15431,
            GreenGuardian = 15420,
            // Gorse
            ChargedSoul = 15434,
            EnragedSpirit = 16024,
            AngeredSpirit = 16005,
            // Sab
            Kernan = 15372,
            Knuckles = 15404,
            Karde = 15430,
            BanditSapper = 15423,
            BanditThug = 15397,
            BanditArsonist = 15421,
            // Slothasor
            Slubling1 = 16064,
            Slubling2 = 16071,
            Slubling3 = 16077,
            Slubling4 = 16104,
            // Trio
            BanditSaboteur = 16117,
            Warg = 7481,
            CagedWarg = 16129,
            BanditAssassin = 16067,
            BanditSapperTrio = 16074,
            BanditDeathsayer = 16076,
            BanditBrawler = 16066,
            BanditBattlemage = 16093,
            BanditCleric = 16101,
            BanditBombardier = 16138,
            BanditSniper = 16065,
            NarellaTornado = 16092,
            OilSlick = 16096,
            Prisoner1 = 16056,
            Prisoner2 = 16103,
            // Matthias
            Spirit = 16105,
            Spirit2 = 16114,
            IcePatch = 16139,
            Storm = 16108,
            Tornado = 16068,
            MatthiasSacrificeCrystal = MatthiasSacrifice,
            // KC
            Olson = 16244,
            Engul = 16274,
            Faerla = 16264,
            Caulle = 16282,
            Henley = 16236,
            Jessica = 16278,
            Galletta = 16228,
            Ianim = 16248,
            Core = 16261,
            GreenPhantasm = 16237,
            InsidiousProjection = 16227,
            UnstableLeyRift = 16277,
            RadiantPhantasm = 16259,
            CrimsonPhantasm = 16257,
            RetrieverProjection = 16249,
            // Twisted Castle
            HauntingStatue = 16247,
            //CastleFountain = 32951,
            // Xera
            BloodstoneShard = ArcDPSEnums.BloodstoneShard,
            ChargedBloodstone = ArcDPSEnums.ChargedBloodstone,
            BloodstoneFragment = ArcDPSEnums.BloodstoneFragment,
            XerasPhantasm = 16225,
            WhiteMantleSeeker1 = 16238,
            WhiteMantleSeeker2 = 16283,
            WhiteMantleKnight1 = 16251,
            WhiteMantleKnight2 = 16287,
            WhiteMantleBattleMage1 = 16221,
            WhiteMantleBattleMage2 = 16226,
            ExquisiteConjunction = 16232,
            // MO
            Jade = 17181,
            // Samarog
            Guldhem = 17208,
            Rigom = 17124,
            // Deimos
            Saul = 17126,
            Thief = 17206,
            Gambler = 17335,
            GamblerClones = 17161,
            GamblerReal = 17355,
            Drunkard = 17163,
            Oil = 17332,
            Tear = 17303,
            Greed = 17213,
            Pride = 17233,
            Hands = 17221,
            // SH
            TormentedDead = 19422,
            SurgingSoul = 19474,
            Scythe = 19396,
            FleshWurm = 19464,
            // River
            Enervator = 19863,
            HollowedBomber = 19399,
            RiverOfSouls = 19829,
            SpiritHorde1 = 19461,
            SpiritHorde2 = 19400,
            SpiritHorde3 = 19692,
            // Statues of Darkness
            LightThieves = 19658,
            MazeMinotaur = 19402,
            // Statue of Death
            OrbSpider = 19801,
            GreenSpirit1 = 19587,
            GreenSpirit2 = 19571,
            // Skeletons are the same as Spirit hordes
            // Dhuum
            Messenger = 19807,
            Echo = 19628,
            Enforcer = 19681,
            Deathling = 19759,
            UnderworldReaper = 19831,
            DhuumDesmina = 19481,
            // CA
            ConjuredGreatsword = 21255,
            ConjuredShield = 21170,
            ConjuredPlayerSword = CASword,
            // Qadim
            LavaElemental1 = 21236,
            LavaElemental2 = 21078,
            IcebornHydra = 21163,
            GreaterMagmaElemental1 = 21150,
            GreaterMagmaElemental2 = 21223,
            FireElemental = 21221,
            FireImp = 21100,
            PyreGuardian = 21050,
            PyreGuardianRetal = ArcDPSEnums.PyreGuardianRetal,
            PyreGuardianResolution = ArcDPSEnums.PyreGuardianResolution,
            PyreGuardianProtect = ArcDPSEnums.PyreGuardianProtect,
            PyreGuardianStab = ArcDPSEnums.PyreGuardianStab,
            ReaperofFlesh = 21218,
            DestroyerTroll = 20944,
            IceElemental = 21049,
            AncientInvokedHydra = 21285,
            ApocalypseBringer = 21073,
            WyvernMatriarch = 20997,
            WyvernPatriarch = 21183,
            QadimLamp = ArcDPSEnums.QadimLamp,
            Zommoros = 20961, //21118 is probably the start and end NPC, not the one during the battle
            // Adina
            HandOfErosion = ArcDPSEnums.HandOfErosion,
            HandOfEruption = ArcDPSEnums.HandOfEruption,
            // Sabir
            ParalyzingWisp = 21955,
            VoltaicWisp = 21975,
            SmallJumpyTornado = 21961,
            SmallKillerTornado = 21957,
            BigKillerTornado = 21987,
            // Peerless Qadim
            Pylon1 = 21996,
            Pylon2 = 21962,
            EntropicDistortion = 21973,
            EnergyOrb = 21946,
            // Fraenir
            IcebroodElemental = 22576,
            // Boneskinner
            PrioryExplorer = 22561,
            PrioryScholar = 22448,
            VigilRecruit = 22389,
            VigilTactician = 22420,
            AberrantWisp = 22538,
            // Whisper of Jormal
            WhisperEcho = 22628,
            // Cold War
            PropagandaBallon = 23093,
            DominionBladestorm = 23102,
            DominionStalker = 22882,
            DominionSpy1 = 22833,
            DominionSpy2 = 22856,
            DominionAxeFiend = 22938,
            DominionEffigy = 22897,
            FrostLegionCrusher = 23005,
            FrostLegionMusketeer = 22870,
            BloodLegionBlademaster = 22993,
            CharrTank = 22953,
            SonsOfSvanirHighShaman = 22283,
            // to complete
            DoppelgangerNecro = 22713,
            DoppelgangerWarrior = 22640,
            DoppelgangerGuardian1 = 22635,
            DoppelgangerGuardian2 = 22608,
            DoppelgangerThief1 = 22656,
            DoppelgangerThief2 = 22612,
            DoppelgangerRevenant = 22610,
            // Freezie
            FreeziesFrozenHeart = 21328,
            // Fractals
            FractalVindicator = 19684,
            FractalAvenger = 15960,
            // MAMA
            GreenKnight = 16906,
            RedKnight = 16974,
            BlueKnight = 16899,
            TwistedHorror = 17009,
            // Siax
            Hallucination = 17002,
            EchoOfTheUnclean = 17068,
            // Ensolyss
            NightmareHallucination1 = 16912, // (exploding after jump and charging in last phase)
            NightmareHallucination2 = 17033, // (small adds, last phase)
            // Skorvald
            FluxAnomaly4 = 17673,
            FluxAnomaly3 = 17851,
            FluxAnomaly2 = 17770,
            FluxAnomaly1 = 17599,
            SolarBloom = 17732,
            // Artsariiv
            TemporalAnomaly = 17870,
            Spark = 17630,
            SmallArtsariiv = 17811, // tiny adds
            MediumArtsariiv = 17694, // small adds
            BigArtsariiv = 17937, // big adds
            CloneArtsariiv = SubArtsariiv, // clone adds
            // Arkk
            TemporalAnomaly2 = 17720,
            Archdiviner = 17893,
            Fanatic = 11282,
            BrazenGladiator = 17730,
            BLIGHT = 16437,
            PLINK = 16325,
            DOC = 16657,
            CHOP = 16552,
            ProjectionArkk = 17613,
            // Ai
            EnrageWaterSprite = 23270,
            SorrowDemon1 = 23265,
            SorrowDemon2 = 23242,
            SorrowDemon3 = 23279,
            SorrowDemon4 = 23245,
            SorrowDemon5 = 23256,
            DoubtDemon = 23268,
            FearDemon = 23264,
            GuiltDemon = 23252,
            //
            Unknown = int.MaxValue,
        };
        public static TrashID GetTrashID(int id)
        {
            return Enum.IsDefined(typeof(TrashID), id) ? (TrashID)id : TrashID.Unknown;
        }

        public enum TargetID : int
        {
            WorldVersusWorld = 1,
            DummyTarget = ArcDPSEnums.DummyTarget,
            // Raid
            ValeGuardian = 15438,
            Gorseval = 15429,
            Sabetha = 15375,
            Slothasor = 16123,
            Berg = 16088,
            Zane = 16137,
            Narella = 16125,
            Matthias = 16115,
            Escort = 16253, // McLeod the Silent
            KeepConstruct = 16235,
            Xera = 16246,
            Cairn = 17194,
            MursaatOverseer = 17172,
            Samarog = 17188,
            Deimos = 17154,
            SoullessHorror = 19767,
            Desmina = 19828,
            BrokenKing = 19691,
            SoulEater = 19536,
            EyeOfJudgement = 19651,
            EyeOfFate = 19844,
            Dhuum = 19450,
            ConjuredAmalgamate = 43974, // Gadget
            CARightArm = 10142, // Gadget
            CALeftArm = 37464, // Gadget
            Nikare = 21105,
            Kenut = 21089,
            Qadim = 20934,
            Freezie = 21333,
            Adina = 22006,
            Sabir = 21964,
            PeerlessQadim = 22000,
            // Strike Missions
            IcebroodConstruct = 22154,
            VoiceOfTheFallen = 22343,
            ClawOfTheFallen = 22481,
            VoiceAndClaw = 22315,
            FraenirOfJormag = 22492,
            IcebroodConstructFraenir = 22436,
            Boneskinner = 22521,
            WhisperOfJormag = 22711,
            VariniaStormsounder = 22836,
            // Fract
            MAMA = 17021,
            Siax = 17028,
            Ensolyss = 16948,
            Skorvald = 17632,
            Artsariiv = 17949,
            Arkk = 17759,
            AiKeeperOfThePeak = 23254,
            AiKeeperOfThePeak2 = ArcDPSEnums.AiKeeperOfThePeak2,
            // Golems
            MassiveGolem10M = 16169,
            MassiveGolem4M = 16202,
            MassiveGolem1M = 16178,
            VitalGolem = 16198,
            AvgGolem = 16177,
            StdGolem = 16199,
            LGolem = 19676,
            MedGolem = 19645,
            ConditionGolem = 16174,
            PowerGolem = 16176,
            //
            Unknown = int.MaxValue,
        };
        public static TargetID GetTargetID(int id)
        {
            return Enum.IsDefined(typeof(TargetID), id) ? (TargetID)id : TargetID.Unknown;
        }
    }
}
