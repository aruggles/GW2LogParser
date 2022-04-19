using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors.ActorsHelper;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using Gw2LogParser.Parser.Data.El.Professions;
using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Data.Events;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using Gw2LogParser.Parser.Helper.CachingCollections;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Helper.ParserHelper;
using Point3D = Gw2LogParser.Parser.Data.El.Statistics.Point3D;

namespace Gw2LogParser.Parser.Data.El.Actors
{
    public abstract class AbstractSingleActor : AbstractActor
    {
        public new Agent AgentItem => base.AgentItem;
        public bool HasCommanderTag => AgentItem.HasCommanderTag;
        public string Account { get; protected set; }
        public int Group { get; protected set; }

        // Helpers
        private readonly SingleActorBuffsHelper _buffHelper;
        private readonly SingleActorGraphsHelper _graphHelper;
        private readonly SingleActorDamageModifierHelper _damageModifiersHelper;
        private readonly SingleActorStatusHelper _statusHelper;
        public EXTAbstractSingleActorHealingHelper EXTHealing { get; }
        // Minions
        private Dictionary<long, Minions> _minions;
        // Replay
        private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>> _typedSelfHitDamageEvents = new Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>>();
        protected CombatReplay CombatReplay { get; set; }
        // Statistics
        private CachingCollectionWithTarget<FinalDPS> _dpsStats;
        private CachingCollectionWithTarget<FinalDefenses> _defenseStats;
        private CachingCollectionWithTarget<FinalGameplayStats> _gameplayStats;
        private CachingCollectionWithTarget<FinalSupport> _supportStats;
        private CachingCollection<FinalToPlayersSupport> _toPlayerSupportStats;

        protected AbstractSingleActor(Agent agent) : base(agent)
        {
            Group = 51;
            Account = Character;
            _buffHelper = new SingleActorBuffsHelper(this);
            _graphHelper = new SingleActorGraphsHelper(this);
            _damageModifiersHelper = new SingleActorDamageModifierHelper(this);
            _statusHelper = new SingleActorStatusHelper(this);
            EXTHealing = new EXTAbstractSingleActorHealingHelper(this);
        }

        // Status

        protected int Health { get; set; } = -2;

        public int GetHealth(CombatData combatData)
        {
            if (Health == -2)
            {
                IReadOnlyList<MaxHealthUpdateEvent> maxHpUpdates = combatData.GetMaxHealthUpdateEvents(AgentItem);
                Health = maxHpUpdates.Count > 0 ? maxHpUpdates.Max(x => x.MaxHealth) : -1;
            }
            return Health;
        }

        internal abstract void SetManualHealth(int health);

        internal abstract void OverrideName(string name);

        public (IReadOnlyList<(long start, long end)>, IReadOnlyList<(long start, long end)>, IReadOnlyList<(long start, long end)>) GetStatus(ParsedLog log)
        {
            return _statusHelper.GetStatus(log);
        }

        public long GetActiveDuration(ParsedLog log, long start, long end)
        {
            return _statusHelper.GetActiveDuration(log, start, end);
        }
        public bool IsDowned(ParsedLog log, long time)
        {
            (_, IReadOnlyList<(long start, long end)> downs, _) = _statusHelper.GetStatus(log);
            return downs.Any(x => x.start <= time && x.end >= time);
        }
        public bool IsDead(ParsedLog log, long time)
        {
            (IReadOnlyList<(long start, long end)> deads, _, _) = _statusHelper.GetStatus(log);
            return deads.Any(x => x.start <= time && x.end >= time);
        }
        public bool IsDC(ParsedLog log, long time)
        {
            (_, _, IReadOnlyList<(long start, long end)> dcs) = _statusHelper.GetStatus(log);
            return dcs.Any(x => x.start <= time && x.end >= time);
        }

        //


        public IReadOnlyList<string> GetWeaponsArray(ParsedLog log)
        {
            return _statusHelper.GetWeaponsArray(log);
        }

        //
        public IReadOnlyList<Consumable> GetConsumablesList(ParsedLog log, long start, long end)
        {
            return _buffHelper.GetConsumablesList(log, start, end);
        }
        //
        public IReadOnlyList<DeathRecap> GetDeathRecaps(ParsedLog log)
        {
            return _statusHelper.GetDeathRecaps(log);
        }

        //

        public abstract string GetIcon();

        public IReadOnlyList<Segment> GetHealthUpdates(ParsedLog log)
        {
            return _graphHelper.GetHealthUpdates(log);
        }

        public double GetCurrentHealthPercent(ParsedLog log, long time)
        {
            return _graphHelper.GetCurrentHealthPercent(log, time);
        }

        public IReadOnlyList<Segment> GetBreakbarPercentUpdates(ParsedLog log)
        {
            return _graphHelper.GetBreakbarPercentUpdates(log);
        }

        public IReadOnlyList<Segment> GetBarrierUpdates(ParsedLog log)
        {
            return _graphHelper.GetBarrierUpdates(log);
        }

        public double GetCurrentBarrierPercent(ParsedLog log, long time)
        {
            return _graphHelper.GetCurrentBarrierPercent(log, time);
        }

        // Minions
        public IReadOnlyDictionary<long, Minions> GetMinions(ParsedLog log)
        {
            if (_minions == null)
            {
                _minions = new Dictionary<long, Minions>();
                // npcs, species id based
                var combatMinion = log.AgentData.GetAgentByType(Agent.AgentType.NPC).Where(x => x.Master != null && x.GetFinalMaster() == AgentItem).ToList();
                var auxMinions = new Dictionary<long, Minions>();
                foreach (Agent agent in combatMinion)
                {
                    long id = agent.ID;
                    AbstractSingleActor singleActor = log.FindActor(agent);
                    if (singleActor is NPC npc)
                    {
                        if (auxMinions.TryGetValue(id, out Minions values))
                        {
                            values.AddMinion(npc);
                        }
                        else
                        {
                            auxMinions[id] = new Minions(this, npc);
                        }
                    }
                }
                foreach (KeyValuePair<long, Minions> pair in auxMinions)
                {
                    if (pair.Value.GetDamageEvents(null, log, 0, log.FightData.FightEnd).Count > 0 || pair.Value.GetCastEvents(log, 0, log.FightData.FightEnd).Any(x => x.SkillId != Skill.WeaponSwapId && x.SkillId != Skill.MirageCloakDodgeId) || MesmerHelper.IsClone(pair.Key))
                    {
                        _minions[pair.Value.UniqueID] = pair.Value;
                    }
                }
                // gadget, string based
                var combatGadgetMinion = log.AgentData.GetAgentByType(Agent.AgentType.Gadget).Where(x => x.Master != null && x.GetFinalMaster() == AgentItem).ToList();
                var auxGadgetMinions = new Dictionary<string, Minions>();
                foreach (Agent agent in combatGadgetMinion)
                {
                    string id = agent.Name;
                    AbstractSingleActor singleActor = log.FindActor(agent);
                    if (singleActor is NPC npc)
                    {
                        if (auxGadgetMinions.TryGetValue(id, out Minions values))
                        {
                            values.AddMinion(npc);
                        }
                        else
                        {
                            auxGadgetMinions[id] = new Minions(this, npc);
                        }
                    }
                }
                foreach (KeyValuePair<string, Minions> pair in auxGadgetMinions)
                {
                    if (pair.Value.GetDamageEvents(null, log, 0, log.FightData.FightEnd).Count > 0 || pair.Value.GetCastEvents(log, 0, log.FightData.FightEnd).Any(x => x.SkillId != Skill.WeaponSwapId && x.SkillId != Skill.MirageCloakDodgeId))
                    {
                        _minions[pair.Value.UniqueID] = pair.Value;
                    }
                }
            }
            return _minions;
        }

        // Graph
        public IReadOnlyList<int> Get1SDamageList(ParsedLog log, long start, long end, AbstractSingleActor target, ParserHelper.DamageType damageType)
        {
            return _graphHelper.Get1SDamageList(log, start, end, target, damageType);
        }

        public IReadOnlyList<double> Get1SBreakbarDamageList(ParsedLog log, long start, long end, AbstractSingleActor target)
        {
            return _graphHelper.Get1SBreakbarDamageList(log, start, end, target);
        }

        // Damage Modifiers

        public IReadOnlyDictionary<string, DamageModifierStat> GetDamageModifierStats(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            return _damageModifiersHelper.GetDamageModifierStats(target, log, start, end);
        }

        public IReadOnlyCollection<string> GetPresentDamageModifier(ParsedLog log)
        {
            return _damageModifiersHelper.GetPresentDamageModifier(log);
        }

        // Buffs
        public BuffDistribution GetBuffDistribution(ParsedLog log, long start, long end)
        {
            return _buffHelper.GetBuffDistribution(log, start, end);
        }

        public Dictionary<long, long> GetBuffPresence(ParsedLog log, long start, long end)
        {
            return _buffHelper.GetBuffPresence(log, start, end);
        }

        internal virtual Dictionary<long, FinalActorBuffs>[] ComputeBuffs(ParsedLog log, long start, long end, BuffEnum type)
        {
            Dictionary<long, FinalActorBuffs>[] empty =
            {
                        new Dictionary<long, FinalActorBuffs>(),
                        new Dictionary<long, FinalActorBuffs>()
             };
            switch (type)
            {
                case BuffEnum.Group:
                    return empty;
                case BuffEnum.OffGroup:
                    return empty;
                case BuffEnum.Squad:
                    var otherPlayers = log.PlayerList.Where(p => p != this).ToList();
                    return FinalActorBuffs.GetBuffsForPlayers(otherPlayers, log, AgentItem, start, end);
                case BuffEnum.Self:
                default:
                    return FinalActorBuffs.GetBuffsForSelf(log, this, start, end);
            }
        }


        public Dictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedLog log)
        {
            return _buffHelper.GetBuffGraphs(log);
        }

        /// <summary>
        /// Checks if a buff is present on the actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        /// <param name="log"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedLog log, long buffId, long time)
        {
            return _buffHelper.HasBuff(log, buffId, time);
        }

        public IReadOnlyDictionary<long, FinalActorBuffs> GetBuffs(BuffEnum type, ParsedLog log, long start, long end)
        {
            return _buffHelper.GetBuffs(type, log, start, end);
        }

        public IReadOnlyDictionary<long, FinalActorBuffs> GetActiveBuffs(BuffEnum type, ParsedLog log, long start, long end)
        {
            return _buffHelper.GetActiveBuffs(type, log, start, end);
        }

        public IReadOnlyCollection<Buff> GetTrackedBuffs(ParsedLog log)
        {
            return _buffHelper.GetTrackedBuffs(log);
        }


        public Dictionary<long, FinalBuffsDictionary> GetBuffsDictionary(ParsedLog log, long start, long end)
        {
            return _buffHelper.GetBuffsDictionary(log, start, end);
        }

        //
        protected void SetMovements(ParsedLog log)
        {
            foreach (AbstractMovementEvent movementEvent in log.CombatData.GetMovementData(AgentItem))
            {
                movementEvent.AddPoint3D(CombatReplay);
            }
        }

        public IReadOnlyList<Point3D> GetCombatReplayPolledPositions(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return CombatReplay.PolledPositions;
        }

        protected static void TrimCombatReplay(ParsedLog log, CombatReplay replay, Agent agentItem)
        {
            // Trim
            DespawnEvent despawnCheck = log.CombatData.GetDespawnEvents(agentItem).LastOrDefault();
            SpawnEvent spawnCheck = log.CombatData.GetSpawnEvents(agentItem).LastOrDefault();
            DeadEvent deathCheck = log.CombatData.GetDeadEvents(agentItem).LastOrDefault();
            AliveEvent aliveCheck = log.CombatData.GetAliveEvents(agentItem).LastOrDefault();
            if (deathCheck != null && (aliveCheck == null || aliveCheck.Time < deathCheck.Time))
            {
                replay.Trim(agentItem.FirstAware, deathCheck.Time);
            }
            else if (despawnCheck != null && (spawnCheck == null || spawnCheck.Time < despawnCheck.Time))
            {
                replay.Trim(agentItem.FirstAware, despawnCheck.Time);
            }
            else
            {
                replay.Trim(agentItem.FirstAware, agentItem.LastAware);
            }
        }

        protected virtual void TrimCombatReplay(ParsedLog log)
        {

        }

        protected void InitCombatReplay(ParsedLog log)
        {
            CombatReplay = new CombatReplay(log);
            if (!log.CombatData.HasMovementData)
            {
                // no combat replay support on fight
                return;
            }
            SetMovements(log);
            CombatReplay.PollingRate(log.FightData.FightEnd);
            TrimCombatReplay(log);
            if (!IsFakeActor)
            {
                InitAdditionalCombatReplayData(log);
            }
        }

        public IReadOnlyList<GenericDecoration> GetCombatReplayDecorations(ParsedLog log)
        {
            if (!log.CanCombatReplay)
            {
                // no combat replay support on fight
                return new List<GenericDecoration>();
            }
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return CombatReplay.Decorations;
        }
        protected abstract void InitAdditionalCombatReplayData(ParsedLog log);

        public abstract AbstractSingleActorCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedLog log);

        // Cast logs
        public override IReadOnlyList<AbstractCastEvent> GetCastEvents(ParsedLog log, long start, long end)
        {
            if (CastEvents == null)
            {
                SetCastEvents(log);
            }
            return CastEvents.Where(x => x.Time >= start && x.Time <= end).ToList();

        }
        public override IReadOnlyList<AbstractCastEvent> GetIntersectingCastEvents(ParsedLog log, long start, long end)
        {
            if (CastEvents == null)
            {
                SetCastEvents(log);
            }
            return CastEvents.Where(x => KeepIntersectingCastLog(x, start, end)).ToList();

        }
        protected void SetCastEvents(ParsedLog log)
        {
            CastEvents = new List<AbstractCastEvent>(log.CombatData.GetAnimatedCastData(AgentItem));
            CastEvents.AddRange(log.CombatData.GetInstantCastData(AgentItem));
            foreach (WeaponSwapEvent wepSwap in log.CombatData.GetWeaponSwapData(AgentItem))
            {
                if (CastEvents.Count > 0 && (wepSwap.Time - CastEvents.Last().Time) < ParserHelper.ServerDelayConstant && CastEvents.Last().SkillId == Skill.WeaponSwapId)
                {
                    CastEvents[CastEvents.Count - 1] = wepSwap;
                }
                else
                {
                    CastEvents.Add(wepSwap);
                }
            }
            CastEvents = CastEvents.OrderBy(x => x.Time).ThenBy(x => x.Skill.IsSwap).ToList();
        }

        // DPS Stats

        public FinalDPS GetDPSStats(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (_dpsStats == null)
            {
                _dpsStats = new CachingCollectionWithTarget<FinalDPS>(log);
            }
            if (!_dpsStats.TryGetValue(start, end, target, out FinalDPS value))
            {
                value = new FinalDPS(log, start, end, this, target);
                _dpsStats.Set(start, end, target, value);
            }
            return value;
        }

        public FinalDPS GetDPSStats(ParsedLog log, long start, long end)
        {
            return GetDPSStats(null, log, start, end);
        }

        // Defense Stats

        public FinalDefenses GetDefenseStats(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (_defenseStats == null)
            {
                _defenseStats = new CachingCollectionWithTarget<FinalDefenses>(log);
            }
            if (!_defenseStats.TryGetValue(start, end, target, out FinalDefenses value))
            {
                value = target != null ? new FinalDefenses(log, start, end, this, target) : new FinalDefensesAll(log, start, end, this);
                _defenseStats.Set(start, end, target, value);
            }
            return value;
        }

        public FinalDefensesAll GetDefenseStats(ParsedLog log, long start, long end)
        {
            return GetDefenseStats(null, log, start, end) as FinalDefensesAll;
        }

        // Gameplay Stats

        public FinalGameplayStatsAll GetGameplayStats(ParsedLog log, long start, long end)
        {
            return GetGameplayStats(null, log, start, end) as FinalGameplayStatsAll;
        }

        public FinalGameplayStats GetGameplayStats(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (_gameplayStats == null)
            {
                _gameplayStats = new CachingCollectionWithTarget<FinalGameplayStats>(log);
            }
            if (!_gameplayStats.TryGetValue(start, end, target, out FinalGameplayStats value))
            {
                value = target != null ? new FinalGameplayStats(log, start, end, this, target) : new FinalGameplayStatsAll(log, start, end, this);
                _gameplayStats.Set(start, end, target, value);
            }
            return value;
        }

        // Support stats
        public FinalSupportAll GetSupportStats(ParsedLog log, long start, long end)
        {
            return GetSupportStats(null, log, start, end) as FinalSupportAll;
        }

        public FinalSupport GetSupportStats(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (_supportStats == null)
            {
                _supportStats = new CachingCollectionWithTarget<FinalSupport>(log);
            }
            if (!_supportStats.TryGetValue(start, end, target, out FinalSupport value))
            {
                value = target != null ? new FinalSupport(log, start, end, this, target) : new FinalSupportAll(log, start, end, this);
                _supportStats.Set(start, end, target, value);
            }
            return value;
        }

        public FinalToPlayersSupport GetToPlayerSupportStats(ParsedLog log, long start, long end)
        {
            if (_toPlayerSupportStats == null)
            {
                _toPlayerSupportStats = new CachingCollection<FinalToPlayersSupport>(log);
            }
            if (!_toPlayerSupportStats.TryGetValue(start, end, out FinalToPlayersSupport value))
            {
                value = new FinalToPlayersSupport(log, this, start, end);
                _toPlayerSupportStats.Set(start, end, value);
            }
            return value;
        }


        // Damage logs
        public override IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (DamageEvents == null)
            {
                DamageEvents = new List<AbstractHealthDamageEvent>();
                DamageEvents.AddRange(log.CombatData.GetDamageData(AgentItem).Where(x => !x.ToFriendly));
                IReadOnlyDictionary<long, Minions> minionsList = GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    DamageEvents.AddRange(mins.GetDamageEvents(null, log, 0, log.FightData.FightEnd));
                }
                DamageEvents = DamageEvents.OrderBy(x => x.Time).ToList();
                DamageEventByDst = DamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageEventByDst.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractHealthDamageEvent>();
                }
            }
            return DamageEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public IReadOnlyList<AbstractHealthDamageEvent> GetJustActorDamageEvents(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            return GetDamageEvents(target, log, start, end).Where(x => x.From == AgentItem).ToList();
        }

        public IReadOnlyList<AbstractBreakbarDamageEvent> GetJustActorBreakbarDamageEvents(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            return GetBreakbarDamageEvents(target, log, start, end).Where(x => x.From == AgentItem).ToList();
        }

        public override IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageEvents(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (BreakbarDamageEvents == null)
            {
                BreakbarDamageEvents = new List<AbstractBreakbarDamageEvent>();
                BreakbarDamageEvents.AddRange(log.CombatData.GetBreakbarDamageData(AgentItem).Where(x => !x.ToFriendly));
                IReadOnlyDictionary<long, Minions> minionsList = GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    BreakbarDamageEvents.AddRange(mins.GetBreakbarDamageEvents(null, log, 0, log.FightData.FightEnd));
                }
                BreakbarDamageEvents = BreakbarDamageEvents.OrderBy(x => x.Time).ToList();
                BreakbarDamageEventsByDst = BreakbarDamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageEventsByDst.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractBreakbarDamageEvent>();
                }
            }
            return BreakbarDamageEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<AbstractHealthDamageEvent> GetDamageTakenEvents(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (DamageTakenEvents == null)
            {
                DamageTakenEvents = new List<AbstractHealthDamageEvent>();
                DamageTakenEvents.AddRange(log.CombatData.GetDamageTakenData(AgentItem));
                DamageTakenEvents = DamageTakenEvents.OrderBy(x => x.Time).ToList();
                DamageTakenEventsBySrc = DamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageTakenEventsBySrc.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<AbstractHealthDamageEvent>();
                }
            }
            return DamageTakenEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenEvents(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (BreakbarDamageTakenEvents == null)
            {
                BreakbarDamageTakenEvents = new List<AbstractBreakbarDamageEvent>();
                BreakbarDamageTakenEvents.AddRange(log.CombatData.GetBreakbarDamageTakenData(AgentItem));
                BreakbarDamageTakenEvents = BreakbarDamageTakenEvents.OrderBy(x => x.Time).ToList();
                BreakbarDamageTakenEventsBySrc = BreakbarDamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageTakenEventsBySrc.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<AbstractBreakbarDamageEvent>();
                }
            }
            return BreakbarDamageTakenEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        /// <summary>
        /// cached method for damage modifiers
        /// </summary>
        internal IReadOnlyList<AbstractHealthDamageEvent> GetJustActorHitDamageEvents(AbstractSingleActor target, ParsedLog log, long start, long end, ParserHelper.DamageType damageType)
        {
            if (!_typedSelfHitDamageEvents.TryGetValue(damageType, out CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> hitDamageEventsPerPhasePerTarget))
            {
                hitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
                _typedSelfHitDamageEvents[damageType] = hitDamageEventsPerPhasePerTarget;
            }
            if (!hitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetHitDamageEvents(target, log, start, end, damageType).Where(x => x.From == AgentItem).ToList();
                hitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        public Point3D GetCurrentPosition(ParsedLog log, long time)
        {
            IReadOnlyList<Point3D> positions = GetCombatReplayPolledPositions(log);
            if (!positions.Any())
            {
                return null;
            }
            return positions.FirstOrDefault(x => x.Time >= time);
        }
    }
}
