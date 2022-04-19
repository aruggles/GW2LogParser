﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Helper;
using Gw2LogParser.Parser.Logic;
using Gw2LogParser.Parser.Logic.Fractals.SunquaPeak;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data
{
    public class FightData
    {
        // Fields
        private List<PhaseData> _phases = new List<PhaseData>();
        private List<PhaseData> _nonDummyPhases = new List<PhaseData>();
        public int TriggerID { get; }
        public FightLogic Logic { get; }
        public long FightEnd { get; private set; } = long.MaxValue;
        public long FightDuration => FightEnd;

        public long LogStart { get; private set; }
        public long LogEnd { get; private set; }

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

        internal enum CMStatus { NotSet, CM, NoCM, CMnoName }

        private CMStatus _isCM = CMStatus.NotSet;
        public bool IsCM => _isCM == CMStatus.CMnoName || _isCM == CMStatus.CM;
        // Constructors
        internal FightData(int id, AgentData agentData, ParserSettings parserSettings, long start, long end)
        {
            LogStart = start;
            LogEnd = end;
            FightEnd = end - start;
            TriggerID = id;
            switch (ArcDPSEnums.GetTargetID(id))
            {
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
                /*case ParseEnum.TargetIDS.Escort:
                    Logic = new Escort(id, agentData);
                    break;*/
                case ArcDPSEnums.TargetID.KeepConstruct:
                    Logic = new KeepConstruct(id);
                    break;
                case ArcDPSEnums.TargetID.Xera:
                    // some TC logs are registered as Xera
                    if (agentData.GetNPCsByID((int)ArcDPSEnums.TrashID.HauntingStatue).Count > 0)
                    {
                        TriggerID = (int)ArcDPSEnums.TrashID.HauntingStatue;
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
                    Logic = new BrokenKing(id);
                    break;
                case ArcDPSEnums.TargetID.SoulEater:
                    Logic = new EaterOfSouls(id);
                    break;
                case ArcDPSEnums.TargetID.EyeOfFate:
                case ArcDPSEnums.TargetID.EyeOfJudgement:
                    Logic = new DarkMaze(id);
                    break;
                case ArcDPSEnums.TargetID.Dhuum:
                    // some eyes logs are registered as Dhuum
                    if (agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.EyeOfFate).Count > 0 ||
                        agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.EyeOfJudgement).Count > 0)
                    {
                        TriggerID = (int)ArcDPSEnums.TargetID.EyeOfFate;
                        Logic = new DarkMaze(TriggerID);
                        break;
                    }
                    Logic = new Dhuum(id);
                    break;
                case ArcDPSEnums.TargetID.ConjuredAmalgamate:
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
                //
                case ArcDPSEnums.TargetID.WorldVersusWorld:
                    Logic = new WvWFight(id, parserSettings.DetailedWvWParse);
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
                //
                default:
                    switch (ArcDPSEnums.GetTrashID(id))
                    {
                        case ArcDPSEnums.TrashID.HauntingStatue:
                            Logic = new TwistedCastle((int)ArcDPSEnums.TargetID.DummyTarget);
                            break;
                        default:
                            // Unknown
                            Logic = new UnknownFightLogic(id);
                            break;
                    }
                    break;
            }
        }

        public string GetFightName(ParsedLog log)
        {
            return Logic.GetLogicName(log) + (_isCM == CMStatus.CM ? " CM" : "");
        }
        public IReadOnlyList<PhaseData> GetPhases(ParsedLog log)
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

        public IReadOnlyList<PhaseData> GetNonDummyPhases(ParsedLog log)
        {
            if (!_nonDummyPhases.Any())
            {
                _nonDummyPhases = GetPhases(log).Where(x => !x.Dummy).ToList();
            }
            return _nonDummyPhases;
        }

        public IReadOnlyList<AbstractSingleActor> GetMainTargets(ParsedLog log)
        {
            return GetPhases(log)[0].Targets;
        }

        // Setters
        internal void SetCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (_isCM == CMStatus.NotSet)
            {
                _isCM = Logic.IsCM(combatData, agentData, fightData);
            }
        }

        internal void SetSuccess(bool success, long fightEnd)
        {
            Success = success;
            FightEnd = fightEnd;
        }

        internal void ApplyOffset(long offset)
        {
            FightEnd += LogStart - offset;
            LogStart -= offset;
            LogEnd -= offset;
        }
    }
}
