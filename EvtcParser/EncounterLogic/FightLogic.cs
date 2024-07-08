﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using System.IO;

namespace GW2EIEvtcParser.EncounterLogic
{
    public abstract class FightLogic
    {

        public enum ParseModeEnum { FullInstance, Instanced10, Instanced5, Benchmark, WvW, sPvP, OpenWorld, Unknown };
        public enum SkillModeEnum { PvE, WvW, sPvP };

        [Flags]
        protected enum FallBackMethod { 
            None = 0,
            Death = 1 << 0,
            CombatExit = 1 << 1,
            ChestGadget = 1 << 2
        }


        private CombatReplayMap _map;
        protected List<Mechanic> MechanicList { get; }//Resurrects (start), Resurrect
        public ParseModeEnum ParseMode { get; protected set; } = ParseModeEnum.Unknown;
        public SkillModeEnum SkillMode { get; protected set; } = SkillModeEnum.PvE;
        public string Extension { get; protected set; }
        public string Icon { get; protected set; }
        private readonly int _basicMechanicsCount;
        public bool HasNoFightSpecificMechanics => MechanicList.Count == _basicMechanicsCount;
        public IReadOnlyCollection<AgentItem> TargetAgents { get; protected set; }
        public IReadOnlyCollection<AgentItem> NonPlayerFriendlyAgents { get; protected set; }
        public IReadOnlyCollection<AgentItem> TrashMobAgents { get; protected set; }
        public IReadOnlyList<NPC> TrashMobs => _trashMobs;
        public IReadOnlyList<AbstractSingleActor> NonPlayerFriendlies => _nonPlayerFriendlies;
        public IReadOnlyList<AbstractSingleActor> Targets => _targets;
        public IReadOnlyList<AbstractSingleActor> Hostiles => _hostiles;
        protected List<NPC> _trashMobs { get; private set; } = new List<NPC>();
        protected List<AbstractSingleActor> _nonPlayerFriendlies { get; private set; } = new List<AbstractSingleActor>();
        protected List<AbstractSingleActor> _targets { get; private set; } = new List<AbstractSingleActor>();
        protected List<AbstractSingleActor> _hostiles { get; private set; } = new List<AbstractSingleActor>();

        protected List<GenericDecoration> EnvironmentDecorations { get; private set; } = null;

        protected ArcDPSEnums.ChestID ChestID { get; set; } = ChestID.None;

        protected List<(Buff buff, int stack)> InstanceBuffs { get; private set; } = null;

        public bool Targetless { get; protected set; } = false;
        internal int GenericTriggerID { get; }

        public long EncounterID { get; protected set; } = EncounterIDs.Unknown;

        public EncounterCategory EncounterCategoryInformation { get; protected set; }
        protected FallBackMethod GenericFallBackMethod { get; set; } = FallBackMethod.Death;

        protected FightLogic(int triggerID)
        {
            GenericTriggerID = triggerID;
            MechanicList = new List<Mechanic>() {
                new PlayerStatusMechanic<DeadEvent>("Dead", new MechanicPlotlySetting(Symbols.X, Colors.Black), "Dead", "Dead", "Dead", 0, (log, a) => log.CombatData.GetDeadEvents(a)).UsingShowOnTable(false),
                new PlayerStatusMechanic<DownEvent>("Downed", new MechanicPlotlySetting(Symbols.Cross, Colors.Red), "Downed", "Downed", "Downed", 0, (log, a) => log.CombatData.GetDownEvents(a)).UsingShowOnTable(false),
                new PlayerCastStartMechanic(SkillIDs.Resurrect, "Resurrect", new MechanicPlotlySetting(Symbols.CrossOpen,Colors.Teal), "Res", "Res", "Res",0).UsingShowOnTable(false),
                new PlayerStatusMechanic<AliveEvent>("Got up", new MechanicPlotlySetting(Symbols.Cross, Colors.Green), "Got up", "Got up", "Got up", 0, (log, a) => log.CombatData.GetAliveEvents(a)).UsingShowOnTable(false),
                new PlayerStatusMechanic<DespawnEvent>("Disconnected", new MechanicPlotlySetting(Symbols.X, Colors.LightGrey), "DC", "DC", "DC", 0, (log, a) => log.CombatData.GetDespawnEvents(a)).UsingShowOnTable(false),
                new PlayerStatusMechanic<SpawnEvent>("Respawn", new MechanicPlotlySetting(Symbols.Cross, Colors.LightBlue), "Resp", "Resp", "Resp", 0, (log, a) => log.CombatData.GetSpawnEvents(a)).UsingShowOnTable(false)
            };
            _basicMechanicsCount = MechanicList.Count;
            EncounterCategoryInformation = new EncounterCategory();
        }

        internal MechanicData GetMechanicData()
        {
            return new MechanicData(MechanicList);
        }

        protected virtual CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("", (800, 800), (0, 0, 0, 0)/*, (0, 0, 0, 0), (0, 0, 0, 0)*/);
        }

        public CombatReplayMap GetCombatReplayMap(ParsedEvtcLog log)
        {
            if (_map == null)
            {
                _map = GetCombatMapInternal(log);
                _map.ComputeBoundingBox(log);
            }
            return _map;
        }

        protected virtual void SetInstanceBuffs(ParsedEvtcLog log)
        {
            InstanceBuffs = new List<(Buff buff, int stack)>();
            foreach (Buff fractalInstability in log.Buffs.BuffsBySource[ParserHelper.Source.FractalInstability])
            {
                if (log.CombatData.GetBuffData(fractalInstability.ID).Any(x => x.To.IsPlayer))
                {
                    InstanceBuffs.Add((fractalInstability, 1));
                }
            }
            int emboldenedStacks = (int)log.PlayerList.Select(x => {
                if (x.GetBuffGraphs(log).TryGetValue(SkillIDs.Emboldened, out BuffsGraphModel graph))
                {
                    return graph.BuffChart.Where(y => y.IntersectSegment(log.FightData.FightStart, log.FightData.FightEnd)).Max(y => y.Value);
                }
                else
                {
                    return 0;
                }
            }).Max();
            if (emboldenedStacks > 0)
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[SkillIDs.Emboldened], emboldenedStacks));
            }
        }

        public virtual IReadOnlyList<(Buff buff, int stack)> GetInstanceBuffs(ParsedEvtcLog log)
        {
            if (InstanceBuffs == null)
            {
                SetInstanceBuffs(log);
            }
            return InstanceBuffs;
        }

        internal virtual int GetTriggerID()
        {
            return GenericTriggerID;
        }

        protected virtual List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                GenericTriggerID
            };
        }

        protected virtual Dictionary<int, int> GetTargetsSortIDs()
        {
            var res = new Dictionary<int, int>();
            for (int i = 0; i < GetTargetsIDs().Count; i++)
            {
                res.Add(GetTargetsIDs()[i], i);
            }
            return res;
        }

        protected virtual List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>();
        }

        protected virtual List<int> GetFriendlyNPCIDs()
        {
            return new List<int>();
        }

        internal virtual string GetLogicName(CombatData combatData, AgentData agentData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID));
            if (target == null)
            {
                return "UNKNOWN";
            }
            return target.Character;
        }

        protected abstract HashSet<int> GetUniqueNPCIDs();

        internal virtual void ComputeFightTargets(AgentData agentData, List<CombatItem> combatItems, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            foreach (int id in GetUniqueNPCIDs())
            {
                RegroupTargetsByID(id, agentData, combatItems, extensions);
            }
            //
            var targetIDs = new HashSet<int>(GetTargetsIDs());
            foreach (int id in targetIDs)
            {
                IReadOnlyList<AgentItem> agents = agentData.GetNPCsByID(id);
                foreach (AgentItem agentItem in agents)
                {
                    _targets.Add(new NPC(agentItem));
                }
            }
            _targets = _targets.OrderBy(x => x.FirstAware).ToList();
            Dictionary<int, int> targetSortIDs = GetTargetsSortIDs();
            _targets = _targets.OrderBy(x =>
            {
            if (targetSortIDs.TryGetValue(x.ID, out int sortKey))
                {
                    return sortKey;
                }
                return int.MaxValue;
            }).ToList();
            //
            var trashIDs = new HashSet<TrashID>(GetTrashMobsIDs());
            if (trashIDs.Any(x => targetIDs.Contains((int)x))) {
                throw new InvalidDataException("ID collision between trash and targets");
            }
            var aList = agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => trashIDs.Contains(GetTrashID(x.ID))).ToList();
            //aList.AddRange(agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => ids2.Contains(ParseEnum.GetTrashIDS(x.ID))));
            foreach (AgentItem a in aList)
            {
                _trashMobs.Add(new NPC(a));
            }
#if DEBUG2
            var unknownAList = agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.InstID != 0 && x.LastAware - x.FirstAware > 1000 && !trashIDs.Contains(GetTrashID(x.ID)) && !targetIDs.Contains(x.ID) && !x.GetFinalMaster().IsPlayer).ToList();
            unknownAList.AddRange(agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.LastAware - x.FirstAware > 1000 && !x.GetFinalMaster().IsPlayer));
            foreach (AgentItem a in unknownAList)
            {
                _trashMobs.Add(new NPC(a));
            }
#endif
            _trashMobs = _trashMobs.OrderBy(x => x.FirstAware).ToList();
            //
            var friendlyNPCIDs = new HashSet<int>(GetFriendlyNPCIDs());
            foreach (int id in friendlyNPCIDs)
            {
                IReadOnlyList<AgentItem> agents = agentData.GetNPCsByID(id);
                foreach (AgentItem agentItem in agents)
                {
                    _nonPlayerFriendlies.Add(new NPC(agentItem));
                }
            }
            _nonPlayerFriendlies = _nonPlayerFriendlies.OrderBy(x => x.FirstAware).ToList();
            FinalizeComputeFightTargets();
        }

        protected void FinalizeComputeFightTargets()
        {
            //
            TargetAgents = new HashSet<AgentItem>(_targets.Select(x => x.AgentItem));
            NonPlayerFriendlyAgents = new HashSet<AgentItem>(_nonPlayerFriendlies.Select(x => x.AgentItem));
            TrashMobAgents = new HashSet<AgentItem>(_trashMobs.Select(x => x.AgentItem));
            _hostiles.AddRange(_targets);
            _hostiles.AddRange(_trashMobs);
        }

        internal virtual List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>();
        }
        
        internal void InvalidateEncounterID()
        {
            EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        }

        internal List<PhaseData> GetBreakbarPhases(ParsedEvtcLog log, bool requirePhases)
        {
            if (!requirePhases)
            {
                return new List<PhaseData>();
            }
            var breakbarPhases = new List<PhaseData>();
            foreach (AbstractSingleActor target in Targets)
            {
                int i = 0;
                (_, IReadOnlyList<Segment> actives, _, _) = target.GetBreakbarStatus(log);
                foreach (Segment active in actives)
                {
                    if (Math.Abs(active.End - active.Start) < ParserHelper.ServerDelayConstant)
                    {
                        continue;
                    }
                    long start = Math.Max(active.Start - 2000, log.FightData.FightStart);
                    long end = Math.Min(active.End, log.FightData.FightEnd);
                    var phase = new PhaseData(start, end, target.Character + " Breakbar " + ++i)
                    {
                        BreakbarPhase = true,
                        CanBeSubPhase = false
                    };
                    phase.AddTarget(target);
                    breakbarPhases.Add(phase);
                }
            }
            return breakbarPhases;
        }

        internal virtual List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            phases[0].AddTarget(mainTarget);
            return phases;
        }

        internal virtual List<ErrorEvent> GetCustomWarningMessages(FightData fightData, int arcdpsVersion)
        {
            if (arcdpsVersion >= ArcDPSBuilds.DirectX11Update)
            {
                return new List<ErrorEvent>
                {
                    new ErrorEvent("As of arcdps 20210923, animated cast events' durations are broken, as such, any feature having a dependency on it are to be taken with a grain of salt. Impacted features are: <br>- Rotations <br>- Time spent in animation statistics <br>- Mechanics <br>- Phases <br>- Combat Replay Decorations")
                };
            }
            return new List<ErrorEvent>();
        }

        protected void AddTargetsToPhase(PhaseData phase, List<int> ids)
        {
            foreach (AbstractSingleActor target in Targets)
            {
                if (ids.Contains(target.ID) && phase.InInterval(Math.Max(target.FirstAware + ParserHelper.ServerDelayConstant, 0)))
                {
                    phase.AddTarget(target);
                }
            }
        }

        protected void AddSecondaryTargetsToPhase(PhaseData phase, List<int> ids)
        {
            foreach (AbstractSingleActor target in Targets)
            {
                if (ids.Contains(target.ID) && phase.InInterval(Math.Max(target.FirstAware + ParserHelper.ServerDelayConstant, 0)))
                {
                    phase.AddSecondaryTarget(target);
                }
            }
        }

        protected void AddTargetsToPhaseAndFit(PhaseData phase, List<int> ids, ParsedEvtcLog log)
        {
            AddTargetsToPhase(phase, ids);
            phase.OverrideTimes(log);
        }

        internal virtual List<AbstractBuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
        {
            return new List<AbstractBuffEvent>();
        }

        internal virtual List<AbstractCastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
        {
            return new List<AbstractCastEvent>();
        }

        internal virtual List<AbstractHealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
        {
            return new List<AbstractHealthDamageEvent>();
        }

        internal virtual void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
        }

        internal virtual void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
        }

        internal virtual void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {

        }

        internal IReadOnlyList<GenericDecoration> GetEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            if (EnvironmentDecorations == null)
            {
                EnvironmentDecorations = new List<GenericDecoration>();
                ComputeEnvironmentCombatReplayDecorations(log);
                EnvironmentDecorations.RemoveAll(x => x.Lifespan.end <= x.Lifespan.start);
            }
            return EnvironmentDecorations;
        }

        internal virtual FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.Normal;
        }

        internal virtual FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterStartStatus.Normal;
        }

        protected virtual List<int> GetSuccessCheckIDs()
        {
            return new List<int>
            {
                GenericTriggerID
            };
        }

        internal virtual void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
        }

        protected IReadOnlyList<AbstractSingleActor> GetSuccessCheckTargets()
        {
            return Targets.Where(x => GetSuccessCheckIDs().Contains(x.ID)).ToList();
        }

        protected void NoBouncyChestGenericCheckSucess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            if (!fightData.Success && (GenericFallBackMethod & FallBackMethod.ChestGadget) > 0)
            {
                SetSuccessByChestGadget(ChestID, agentData, fightData);
            }
            if (!fightData.Success && (GenericFallBackMethod & FallBackMethod.Death) > 0)
            {
                SetSuccessByDeath(GetSuccessCheckTargets(), combatData, fightData, playerAgents, true);
            }
            if (!fightData.Success && (GenericFallBackMethod & FallBackMethod.CombatExit) > 0)
            {
                SetSuccessByCombatExit(GetSuccessCheckTargets(), combatData, fightData, playerAgents);
            }
        }

        internal virtual long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                startToUse = GetEnterCombatTime(fightData, agentData, combatData, logStartNPCUpdate.Time, GenericTriggerID, logStartNPCUpdate.DstAgent);
            }
            return startToUse;
        }

        internal virtual FightLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData)
        {
            return this;
        }

        internal virtual void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            ComputeFightTargets(agentData, combatData, extensions);
        }

        /// <summary>
        /// Create a <see cref="List{}"/> containing a <paramref name="buff"/> and its <paramref name="stack"/>.<br></br>
        /// The buff must be present on any player at the end of the encounter.<br></br>
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="buff">The buff ID to add.</param>
        /// <param name="stack">Amount of buff stacks (0-99).</param>
        /// <returns>
        /// A <see cref="IReadOnlyList{T}"/> containing a <paramref name="buff"/> and its <paramref name="stack"/> if present, otherwise empty.<br></br>
        /// To be used to add as range to <see cref="InstanceBuffs"/>.
        /// </returns>
        protected static IReadOnlyList<(Buff, int)> GetOnPlayerCustomInstanceBuff(ParsedEvtcLog log, long buff, int stack = 1)
        {
            var buffs = new List<(Buff, int)>();
            foreach (Player p in log.PlayerList)
            {
                if (p.HasBuff(log, buff, log.FightData.FightEnd - ServerDelayConstant))
                {
                    buffs.Add((log.Buffs.BuffsByIds[buff], stack));
                    break;
                }
            }
            return buffs;
        }
    }
}
