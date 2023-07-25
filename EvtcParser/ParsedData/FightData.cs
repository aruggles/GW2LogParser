﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.EncounterLogic.OpenWorld;

namespace GW2EIEvtcParser.ParsedData
{
    public class FightData
    {
        // Fields
        private List<PhaseData> _phases = new List<PhaseData>();
        public int TriggerID { get; }
        public FightLogic Logic { get; }
        public long FightEnd { get; private set; } = long.MaxValue;
        public long FightStart { get; } = 0;
        public long FightDuration => FightEnd;

        public string FightName { get; private set; }
        public long LogStart { get; private set; }
        public long LogEnd { get; private set; }
        public long LogOffset { get; private set; }

        public long FightStartOffset => -LogStart;
        public string DurationString
        {
            get
            {
                var duration = TimeSpan.FromMilliseconds(FightDuration);
                string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s " + duration.Milliseconds + "ms";
                if (duration.Hours > 0)
                {
                    durationString = duration.ToString("hh") + "h " + durationString;
                }
                return durationString;
            }
        }
        public bool Success { get; private set; }

        internal enum EncounterMode { NotSet, CM, Normal, CMNoName, Story }

        private EncounterMode _encounterStatus = EncounterMode.NotSet;
        public bool IsCM => _encounterStatus == EncounterMode.CMNoName || _encounterStatus == EncounterMode.CM;
        // Constructors
        internal FightData(int id, AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings, long start, long end, int evtcVersion)
        {
            LogStart = start;
            LogEnd = end;
            FightEnd = end - start;
            switch (ArcDPSEnums.GetTargetID(id))
            {
                case ArcDPSEnums.TargetID.Mordremoth:
                    Logic = new Mordremoth(id);
                    break;
                //
                case ArcDPSEnums.TargetID.ValeGuardian:
                    Logic = new ValeGuardian(id);
                    break;
                case ArcDPSEnums.TargetID.Gorseval:
                    Logic = new Gorseval(id);
                    break;
                case ArcDPSEnums.TargetID.Sabetha:
                    Logic = new Sabetha(id);
                    break;
                case ArcDPSEnums.TargetID.Slothasor:
                    Logic = new Slothasor(id);
                    break;
                case ArcDPSEnums.TargetID.Zane:
                case ArcDPSEnums.TargetID.Berg:
                case ArcDPSEnums.TargetID.Narella:
                    Logic = new BanditTrio(id);
                    break;
                case ArcDPSEnums.TargetID.Matthias:
                    Logic = new Matthias(id);
                    break;
                case ArcDPSEnums.TargetID.McLeodTheSilent:
                    // No proper escort support by arc dps before that build, redirect to unknown
                    if (evtcVersion >= ArcDPSEnums.ArcDPSBuilds.NewLogStart)
                    {
                        Logic = new Escort(id);
                    }  
                    else
                    {
                        Logic = new UnknownFightLogic(id);
                    }
                    break;
                case ArcDPSEnums.TargetID.KeepConstruct:
                    Logic = new KeepConstruct(id);
                    break;
                case ArcDPSEnums.TargetID.Xera:
                    // some TC logs are registered as Xera
                    if (agentData.GetNPCsByID(ArcDPSEnums.TrashID.HauntingStatue).Count > 0)
                    {
                        Logic = new TwistedCastle((int)ArcDPSEnums.TargetID.DummyTarget);
                        break;
                    }
                    Logic = new Xera(id);
                    break;
                case ArcDPSEnums.TargetID.Cairn:
                    Logic = new Cairn(id);
                    break;
                case ArcDPSEnums.TargetID.MursaatOverseer:
                    Logic = new MursaatOverseer(id);
                    break;
                case ArcDPSEnums.TargetID.Samarog:
                    Logic = new Samarog(id);
                    break;
                case ArcDPSEnums.TargetID.Deimos:
                    Logic = new Deimos(id);
                    break;
                case ArcDPSEnums.TargetID.SoullessHorror:
                    Logic = new SoullessHorror(id);
                    break;
                case ArcDPSEnums.TargetID.Desmina:
                    Logic = new River((int)ArcDPSEnums.TargetID.DummyTarget);
                    break;
                case ArcDPSEnums.TargetID.BrokenKing:
                    Logic = new StatueOfIce(id);
                    break;
                case ArcDPSEnums.TargetID.EaterOfSouls:
                    Logic = new StatueOfDeath(id);
                    break;
                case ArcDPSEnums.TargetID.EyeOfFate:
                case ArcDPSEnums.TargetID.EyeOfJudgement:
                    Logic = new StatueOfDarkness(id);
                    break;
                case ArcDPSEnums.TargetID.Dhuum:
                    // some eyes logs are registered as Dhuum
                    if (agentData.GetNPCsByID(ArcDPSEnums.TargetID.EyeOfFate).Count > 0 ||
                        agentData.GetNPCsByID(ArcDPSEnums.TargetID.EyeOfJudgement).Count > 0)
                    {
                        Logic = new StatueOfDarkness((int)ArcDPSEnums.TargetID.EyeOfFate);
                        break;
                    }
                    Logic = new Dhuum(id);
                    break;
                case ArcDPSEnums.TargetID.ConjuredAmalgamate:
                case ArcDPSEnums.TargetID.ConjuredAmalgamate_CHINA:
                case ArcDPSEnums.TargetID.CALeftArm_CHINA:
                case ArcDPSEnums.TargetID.CARightArm_CHINA:
                    Logic = new ConjuredAmalgamate(id);
                    break;
                case ArcDPSEnums.TargetID.Kenut:
                case ArcDPSEnums.TargetID.Nikare:
                    Logic = new TwinLargos(id);
                    break;
                case ArcDPSEnums.TargetID.Qadim:
                    Logic = new Qadim(id);
                    break;
                case ArcDPSEnums.TargetID.Freezie:
                    Logic = new Freezie(id);
                    break;
                case ArcDPSEnums.TargetID.Adina:
                    Logic = new Adina(id);
                    break;
                case ArcDPSEnums.TargetID.Sabir:
                    Logic = new Sabir(id);
                    break;
                case ArcDPSEnums.TargetID.PeerlessQadim:
                    Logic = new PeerlessQadim(id);
                    break;
                //
                case ArcDPSEnums.TargetID.IcebroodConstruct:
                    Logic = new IcebroodConstruct(id);
                    break;
                case ArcDPSEnums.TargetID.FraenirOfJormag:
                    Logic = new FraenirOfJormag(id);
                    break;
                case ArcDPSEnums.TargetID.VoiceOfTheFallen:
                case ArcDPSEnums.TargetID.ClawOfTheFallen:
                    Logic = new SuperKodanBrothers(id);
                    break;
                case ArcDPSEnums.TargetID.Boneskinner:
                    Logic = new Boneskinner(id);
                    break;
                case ArcDPSEnums.TargetID.WhisperOfJormag:
                    Logic = new WhisperOfJormag(id);
                    break;
                case ArcDPSEnums.TargetID.VariniaStormsounder:
                    Logic = new ColdWar(id);
                    break;
                case ArcDPSEnums.TargetID.MaiTrinStrike:
                    Logic = new AetherbladeHideout(id);
                    break;
                case ArcDPSEnums.TargetID.MinisterLi:
                case ArcDPSEnums.TargetID.MinisterLiCM:
                    Logic = new KainengOverlook(id);
                    break;
                case ArcDPSEnums.TargetID.Ankka:
                    Logic = new XunlaiJadeJunkyard(id);
                    break;
                // This will most likely require a chinese client version
                case ArcDPSEnums.TargetID.GadgetTheDragonVoid1:
                case ArcDPSEnums.TargetID.GadgetTheDragonVoid2:
                    if (agentData.GetNPCsByID(ArcDPSEnums.TrashID.VoidAmalgamate).Any())
                    {
                        Logic = new HarvestTemple((int)ArcDPSEnums.TargetID.GadgetTheDragonVoid1);
                    }
                    else
                    {
                        Logic = new UnknownFightLogic(id);
                    }
                    break;
                case ArcDPSEnums.TargetID.PrototypeVermilion:
                case ArcDPSEnums.TargetID.PrototypeArsenite:
                case ArcDPSEnums.TargetID.PrototypeIndigo:
                case ArcDPSEnums.TargetID.PrototypeVermilionCM:
                case ArcDPSEnums.TargetID.PrototypeArseniteCM:
                case ArcDPSEnums.TargetID.PrototypeIndigoCM:
                    Logic = new OldLionsCourt(id);
                    break;
                //
                case ArcDPSEnums.TargetID.MAMA:
                    Logic = new MAMA(id);
                    break;
                case ArcDPSEnums.TargetID.Siax:
                    Logic = new Siax(id);
                    break;
                case ArcDPSEnums.TargetID.Ensolyss:
                    Logic = new Ensolyss(id);
                    break;
                case ArcDPSEnums.TargetID.Skorvald:
                    Logic = new Skorvald(id);
                    break;
                case ArcDPSEnums.TargetID.Artsariiv:
                    Logic = new Artsariiv(id);
                    break;
                case ArcDPSEnums.TargetID.Arkk:
                    Logic = new Arkk(id);
                    break;
                case ArcDPSEnums.TargetID.AiKeeperOfThePeak:
                    Logic = new AiKeeperOfThePeak(id);
                    break;
                case ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM:
                    Logic = new Kanaxai(id);
                    break;
                //
                case ArcDPSEnums.TargetID.WorldVersusWorld:
                    if (agentData.GetNPCsByID(ArcDPSEnums.TargetID.Desmina).Any())
                    {
                        Logic = new River((int)ArcDPSEnums.TargetID.DummyTarget);
                        break;
                    }
                    Logic = new WvWFight(id, parserSettings.DetailedWvWParse);
                    break;
                //
                case ArcDPSEnums.TargetID.SooWonOW:
                    Logic = new SooWon(id);
                    break;
                //
                case ArcDPSEnums.TargetID.MassiveGolem10M:
                case ArcDPSEnums.TargetID.MassiveGolem4M:
                case ArcDPSEnums.TargetID.MassiveGolem1M:
                case ArcDPSEnums.TargetID.VitalGolem:
                case ArcDPSEnums.TargetID.AvgGolem:
                case ArcDPSEnums.TargetID.StdGolem:
                case ArcDPSEnums.TargetID.ConditionGolem:
                case ArcDPSEnums.TargetID.PowerGolem:
                case ArcDPSEnums.TargetID.LGolem:
                case ArcDPSEnums.TargetID.MedGolem:
                    Logic = new Golem(id);
                    break;
                case ArcDPSEnums.TargetID.Instance:
                    Logic = new Instance(id);
                    break;
                //
                default:
                    switch (ArcDPSEnums.GetTrashID(id))
                    {
                        case ArcDPSEnums.TrashID.HauntingStatue:
                            Logic = new TwistedCastle((int)ArcDPSEnums.TargetID.DummyTarget);
                            break;
                        case ArcDPSEnums.TrashID.VoidAmalgamate:
                            Logic = new HarvestTemple(id);
                            break;
                        case ArcDPSEnums.TrashID.AncientInvokedHydra:
                            Logic = new Qadim((int)ArcDPSEnums.TargetID.Qadim);
                            break;
                        case ArcDPSEnums.TrashID.VoidMelter:
                            if (agentData.GetNPCsByID(ArcDPSEnums.TrashID.VoidAmalgamate).Any())
                            {
                                Logic = new HarvestTemple((int)ArcDPSEnums.TargetID.GadgetTheDragonVoid1);
                            } 
                            else
                            {
                                Logic = new UnknownFightLogic(id);
                            }
                            break;
                        default:
                            // Unknown
                            Logic = new UnknownFightLogic(id);
                            break;
                    }
                    break;
            }
            Logic = Logic.AdjustLogic(agentData, combatData);
            TriggerID = Logic.GetTriggerID();
        }

        internal void SetFightName(CombatData combatData, AgentData agentData)
        {
            FightName = Logic.GetLogicName(combatData, agentData) + (_encounterStatus == EncounterMode.CM ? " CM" : "") + (_encounterStatus == EncounterMode.Story ? " Story" : "");
        }

        public IReadOnlyList<GenericDecoration> GetEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            return Logic.GetEnvironmentCombatReplayDecorations(log);
        }

        public IReadOnlyList<PhaseData> GetPhases(ParsedEvtcLog log)
        {

            if (!_phases.Any())
            {
                _phases = Logic.GetPhases(log, log.ParserSettings.ParsePhases);
                _phases.AddRange(Logic.GetBreakbarPhases(log, log.ParserSettings.ParsePhases));
                _phases.RemoveAll(x => x.Targets.Count == 0);
                if (_phases.Exists(x => x.BreakbarPhase && x.Targets.Count != 1))
                {
                    throw new InvalidOperationException("Breakbar phases can only have one target");
                }
                _phases.RemoveAll(x => x.DurationInMS < ParserHelper.PhaseTimeLimit);
                _phases.Sort((x, y) =>
                {
                    int startCompare = x.Start.CompareTo(y.Start);
                    if (startCompare == 0)
                    {
                        return -x.DurationInMS.CompareTo(y.DurationInMS);
                    }
                    return startCompare;
                });
            }
            return _phases;
        }

        public IReadOnlyList<AbstractSingleActor> GetMainTargets(ParsedEvtcLog log)
        {
            return GetPhases(log)[0].Targets;
        }

        // Setters
        internal void SetEncounterMode(CombatData combatData, AgentData agentData)
        {
            if (_encounterStatus == EncounterMode.NotSet)
            {
                _encounterStatus = Logic.GetEncounterMode(combatData, agentData, this);
                if (_encounterStatus == EncounterMode.Story)
                {
                    Logic.InvalidateEncounterID();
                }
            }
        }

        internal void SetSuccess(bool success, long fightEnd)
        {
            Success = success;
            FightEnd = fightEnd;
        }

        internal void ApplyOffset(long offset)
        {
            LogOffset = offset;
            FightEnd += LogStart - offset;
            LogStart -= offset;
            LogEnd -= offset;
        }
    }
}
