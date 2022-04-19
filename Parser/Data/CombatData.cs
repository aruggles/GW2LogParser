﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Professions;
using Gw2LogParser.Parser.Data.Events;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Events.MetaData;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data
{
    public class CombatData
    {
        public bool HasMovementData { get; }

        //private List<CombatItem> _healingData;
        //private List<CombatItem> _healingReceivedData;
        private readonly StatusEventsContainer _statusEvents = new StatusEventsContainer();
        private readonly MetaEventsContainer _metaDataEvents = new MetaEventsContainer();
        private readonly HashSet<long> _skillIds;
        private readonly Dictionary<long, List<AbstractBuffEvent>> _buffData;
        private Dictionary<long, List<BuffRemoveAllEvent>> _buffRemoveAllData;
        private readonly Dictionary<Agent, List<AbstractBuffEvent>> _buffDataByDst;
        private readonly Dictionary<Agent, List<AbstractHealthDamageEvent>> _damageData;
        private readonly Dictionary<Agent, List<AbstractBreakbarDamageEvent>> _breakbarDamageData;
        private readonly Dictionary<long, List<AbstractHealthDamageEvent>> _damageDataById;
        private readonly Dictionary<Agent, List<AnimatedCastEvent>> _animatedCastData;
        private readonly Dictionary<Agent, List<InstantCastEvent>> _instantCastData;
        private readonly Dictionary<Agent, List<WeaponSwapEvent>> _weaponSwapData;
        private readonly Dictionary<long, List<AnimatedCastEvent>> _animatedCastDataById;
        private readonly Dictionary<Agent, List<AbstractHealthDamageEvent>> _damageTakenData;
        private readonly Dictionary<Agent, List<AbstractBreakbarDamageEvent>> _breakbarDamageTakenData;
        private readonly List<RewardEvent> _rewardEvents = new List<RewardEvent>();
        // EXTENSIONS
        public EXTHealingCombatData EXTHealingCombatData { get; internal set; }
        public bool HasEXTHealing => EXTHealingCombatData != null;

        internal bool HasStackIDs { get; } = false;

        public bool HasBreakbarDamageData { get; } = false;

        private void EIBuffParse(List<Player> players, SkillData skillData, FightData fightData)
        {
            var toAdd = new List<AbstractBuffEvent>();
            foreach (Player p in players)
            {
                if (p.Spec == ParserHelper.Spec.Weaver)
                {
                    toAdd.AddRange(WeaverHelper.TransformWeaverAttunements(GetBuffData(p.AgentItem), _buffData, p.AgentItem, skillData));
                }
                if (p.BaseSpec == ParserHelper.Spec.Elementalist && p.Spec != ParserHelper.Spec.Weaver)
                {
                    ElementalistHelper.RemoveDualBuffs(GetBuffData(p.AgentItem), _buffData, skillData);
                }
            }
            toAdd.AddRange(fightData.Logic.SpecialBuffEventProcess(this, skillData));
            var buffIDsToSort = new HashSet<long>();
            var buffAgentsToSort = new HashSet<Agent>();
            foreach (AbstractBuffEvent bf in toAdd)
            {
                if (_buffDataByDst.TryGetValue(bf.To, out List<AbstractBuffEvent> buffByDstList))
                {
                    buffByDstList.Add(bf);
                }
                else
                {
                    _buffDataByDst[bf.To] = new List<AbstractBuffEvent>()
                    {
                        bf
                    };
                }
                buffAgentsToSort.Add(bf.To);
                if (_buffData.TryGetValue(bf.BuffID, out List<AbstractBuffEvent> buffByIDList))
                {
                    buffByIDList.Add(bf);
                }
                else
                {
                    _buffData[bf.BuffID] = new List<AbstractBuffEvent>()
                    {
                        bf
                    };
                }
                buffIDsToSort.Add(bf.BuffID);
            }
            foreach (long buffID in buffIDsToSort)
            {
                _buffData[buffID] = _buffData[buffID].OrderBy(x => x.Time).ToList();
            }
            foreach (Agent a in buffAgentsToSort)
            {
                _buffDataByDst[a] = _buffDataByDst[a].OrderBy(x => x.Time).ToList();
            }
            if (toAdd.Any())
            {
                _buffRemoveAllData = _buffData.ToDictionary(x => x.Key, x => x.Value.OfType<BuffRemoveAllEvent>().ToList());
            }
        }

        private void EIDamageParse(SkillData skillData, FightData fightData)
        {
            var toAdd = new List<AbstractHealthDamageEvent>();
            toAdd.AddRange(fightData.Logic.SpecialDamageEventProcess(this, skillData));
            var idsToSort = new HashSet<long>();
            var dstToSort = new HashSet<Agent>();
            var srcToSort = new HashSet<Agent>();
            foreach (AbstractHealthDamageEvent de in toAdd)
            {
                if (_damageTakenData.TryGetValue(de.To, out List<AbstractHealthDamageEvent> damageTakenList))
                {
                    damageTakenList.Add(de);
                }
                else
                {
                    _damageTakenData[de.To] = new List<AbstractHealthDamageEvent>()
                    {
                        de
                    };
                }
                dstToSort.Add(de.To);
                if (_damageData.TryGetValue(de.From, out List<AbstractHealthDamageEvent> damageDoneList))
                {
                    damageDoneList.Add(de);
                }
                else
                {
                    _damageData[de.From] = new List<AbstractHealthDamageEvent>()
                    {
                        de
                    };
                }
                srcToSort.Add(de.From);
                if (_damageDataById.TryGetValue(de.SkillId, out List<AbstractHealthDamageEvent> damageDoneByIDList))
                {
                    damageDoneByIDList.Add(de);
                }
                else
                {
                    _damageDataById[de.SkillId] = new List<AbstractHealthDamageEvent>()
                    {
                        de
                    };
                }
                idsToSort.Add(de.SkillId);
            }
            foreach (long buffID in idsToSort)
            {
                _damageDataById[buffID] = _damageDataById[buffID].OrderBy(x => x.Time).ToList();
            }
            foreach (Agent a in dstToSort)
            {
                _damageTakenData[a] = _damageTakenData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (Agent a in srcToSort)
            {
                _damageData[a] = _damageData[a].OrderBy(x => x.Time).ToList();
            }
        }
        private void EICastParse(List<Player> players, SkillData skillData, FightData fightData, AgentData agentData)
        {
            List<AbstractCastEvent> toAdd = fightData.Logic.SpecialCastEventProcess(this, skillData);
            toAdd.AddRange(ProfHelper.ComputeInstantCastEvents(players, this, skillData, agentData, fightData.Logic));
            //
            var castIDsToSort = new HashSet<long>();
            var castAgentsToSort = new HashSet<Agent>();
            var wepSwapAgentsToSort = new HashSet<Agent>();
            var instantAgentsToSort = new HashSet<Agent>();
            foreach (AbstractCastEvent cast in toAdd)
            {
                if (cast is AnimatedCastEvent ace)
                {
                    if (_animatedCastData.TryGetValue(ace.Caster, out List<AnimatedCastEvent> animatedCastList))
                    {
                        animatedCastList.Add(ace);
                    }
                    else
                    {
                        _animatedCastData[ace.Caster] = new List<AnimatedCastEvent>()
                        {
                            ace
                        };
                    }
                    castAgentsToSort.Add(ace.Caster);
                    if (_animatedCastDataById.TryGetValue(ace.SkillId, out List<AnimatedCastEvent> animatedCastByIDList))
                    {
                        animatedCastByIDList.Add(ace);
                    }
                    else
                    {
                        _animatedCastDataById[cast.SkillId] = new List<AnimatedCastEvent>()
                    {
                        ace
                    };
                    }
                    castIDsToSort.Add(cast.SkillId);
                }
                if (cast is WeaponSwapEvent wse)
                {
                    if (_weaponSwapData.TryGetValue(wse.Caster, out List<WeaponSwapEvent> weaponSwapList))
                    {
                        weaponSwapList.Add(wse);
                    }
                    else
                    {
                        _weaponSwapData[wse.Caster] = new List<WeaponSwapEvent>()
                        {
                            wse
                        };
                    }
                    wepSwapAgentsToSort.Add(wse.Caster);
                }
                if (cast is InstantCastEvent ice)
                {
                    if (_instantCastData.TryGetValue(ice.Caster, out List<InstantCastEvent> instantCastList))
                    {
                        instantCastList.Add(ice);
                    }
                    else
                    {
                        _instantCastData[ice.Caster] = new List<InstantCastEvent>()
                        {
                            ice
                        };
                    }
                    instantAgentsToSort.Add(ice.Caster);
                }
            }
            foreach (long castID in castIDsToSort)
            {
                _animatedCastDataById[castID] = _animatedCastDataById[castID].OrderBy(x => x.Time).ToList();
            }
            foreach (Agent a in castAgentsToSort)
            {
                _animatedCastData[a] = _animatedCastData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (Agent a in wepSwapAgentsToSort)
            {
                _weaponSwapData[a] = _weaponSwapData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (Agent a in instantAgentsToSort)
            {
                _instantCastData[a] = _instantCastData[a].OrderBy(x => x.Time).ToList();
            }
        }

        private void EIMetaAndStatusParse(FightData fightData, int arcdpsVersion)
        {
            foreach (KeyValuePair<Agent, List<AbstractHealthDamageEvent>> pair in _damageTakenData)
            {
                if (pair.Key.ID == (int)ArcDPSEnums.TargetID.WorldVersusWorld)
                {
                    continue;
                }
                bool setDeads = false;
                if (!_statusEvents.DeadEvents.TryGetValue(pair.Key, out List<DeadEvent> agentDeaths))
                {
                    agentDeaths = new List<DeadEvent>();
                    setDeads = true;
                }
                bool setDowns = false;
                if (!_statusEvents.DownEvents.TryGetValue(pair.Key, out List<DownEvent> agentDowns))
                {
                    agentDowns = new List<DownEvent>();
                    setDowns = true;
                }
                foreach (AbstractHealthDamageEvent evt in pair.Value)
                {
                    if (evt.HasKilled)
                    {
                        if (!agentDeaths.Exists(x => Math.Abs(x.Time - evt.Time) < 500))
                        {
                            agentDeaths.Add(new DeadEvent(pair.Key, evt.Time));
                        }
                    }
                    if (evt.HasDowned)
                    {
                        if (!agentDowns.Exists(x => Math.Abs(x.Time - evt.Time) < 500))
                        {
                            agentDowns.Add(new DownEvent(pair.Key, evt.Time));
                        }
                    }
                }
                if (setDeads && agentDeaths.Count > 0)
                {
                    _statusEvents.DeadEvents[pair.Key] = agentDeaths.OrderBy(x => x.Time).ToList();
                }
                if (setDowns && agentDowns.Count > 0)
                {
                    _statusEvents.DownEvents[pair.Key] = agentDowns.OrderBy(x => x.Time).ToList();
                }
            }
            _metaDataEvents.ErrorEvents.AddRange(fightData.Logic.GetCustomWarningMessages(fightData, arcdpsVersion));
        }

        private void EIExtraEventProcess(List<Player> players, SkillData skillData, AgentData agentData, FightData fightData, ParserController operation, int arcdpsVersion)
        {
            operation.UpdateProgressWithCancellationCheck("Creating Custom Buff Events");
            EIBuffParse(players, skillData, fightData);
            operation.UpdateProgressWithCancellationCheck("Creating Custom Damage Events");
            EIDamageParse(skillData, fightData);
            operation.UpdateProgressWithCancellationCheck("Creating Custom Cast Events");
            EICastParse(players, skillData, fightData, agentData);
            operation.UpdateProgressWithCancellationCheck("Creating Custom Status Events");
            EIMetaAndStatusParse(fightData, arcdpsVersion);
            // master attachements
            operation.UpdateProgressWithCancellationCheck("Attaching Banners to Warriors");
            WarriorHelper.AttachMasterToWarriorBanners(players, this);
            operation.UpdateProgressWithCancellationCheck("Attaching Turrets to Engineers");
            EngineerHelper.AttachMasterToEngineerTurrets(players, this);
            operation.UpdateProgressWithCancellationCheck("Attaching Ranger Gadgets to Rangers");
            RangerHelper.AttachMasterToRangerGadgets(players, this);
            operation.UpdateProgressWithCancellationCheck("Attaching Racial Gadgets to Players");
            ProfHelper.AttachMasterToRacialGadgets(players, this);
        }

        internal CombatData(List<Combat> allCombatItems, FightData fightData, AgentData agentData, SkillData skillData, List<Player> players, ParserController operation, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, int evtcVersion)
        {
            var combatEvents = allCombatItems.OrderBy(x => x.Time).ToList();
            _skillIds = new HashSet<long>();
            var castCombatEvents = new Dictionary<ulong, List<Combat>>();
            var buffEvents = new List<AbstractBuffEvent>();
            var wepSwaps = new List<WeaponSwapEvent>();
            var brkDamageData = new List<AbstractBreakbarDamageEvent>();
            var damageData = new List<AbstractHealthDamageEvent>();
            operation.UpdateProgressWithCancellationCheck("Creating EI Combat Data");
            foreach (Combat combatItem in combatEvents)
            {
                _skillIds.Add(combatItem.SkillID);
                if (combatItem.IsStateChange != ArcDPSEnums.StateChange.None)
                {
                    if (combatItem.IsExtension)
                    {
                        if (extensions.TryGetValue(combatItem.Pad, out AbstractExtensionHandler handler))
                        {
                            handler.InsertEIExtensionEvent(combatItem, agentData, skillData);
                        }
                    }
                    else
                    {
                        EventFactory.AddStateChangeEvent(combatItem, agentData, skillData, _metaDataEvents, _statusEvents, _rewardEvents, wepSwaps, buffEvents);
                    }

                }
                else if (combatItem.IsActivation != ArcDPSEnums.Activation.None)
                {
                    if (castCombatEvents.TryGetValue(combatItem.SrcAgent, out List<Combat> list))
                    {
                        list.Add(combatItem);
                    }
                    else
                    {
                        castCombatEvents[combatItem.SrcAgent] = new List<Combat>() { combatItem };
                    }
                }
                else if (combatItem.IsBuffRemove != ArcDPSEnums.BuffRemove.None)
                {
                    EventFactory.AddBuffRemoveEvent(combatItem, buffEvents, agentData, skillData);
                }
                else
                {
                    if (combatItem.IsBuff != 0 && combatItem.BuffDmg == 0 && combatItem.Value > 0)
                    {
                        EventFactory.AddBuffApplyEvent(combatItem, buffEvents, agentData, skillData);
                    }
                    else if (combatItem.IsBuff == 0)
                    {
                        EventFactory.AddDirectDamageEvent(combatItem, damageData, brkDamageData, agentData, skillData);
                    }
                    else if (combatItem.IsBuff != 0 && combatItem.Value == 0)
                    {
                        EventFactory.AddIndirectDamageEvent(combatItem, damageData, brkDamageData, agentData, skillData);
                    }
                }
            }
            HasStackIDs = false;// arcdpsVersion > 20210529 && buffEvents.Any(x => x is BuffStackActiveEvent || x is BuffStackResetEvent) && (fightData.Logic.Mode == EncounterLogic.FightLogic.ParseMode.Instanced10 || fightData.Logic.Mode == EncounterLogic.FightLogic.ParseMode.Instanced5 || fightData.Logic.Mode == EncounterLogic.FightLogic.ParseMode.Benchmark);
            HasMovementData = _statusEvents.MovementEvents.Count > 1;
            HasBreakbarDamageData = brkDamageData.Any();
            //
            operation.UpdateProgressWithCancellationCheck("Combining SkillInfo with SkillData");
            skillData.CombineWithSkillInfo(_metaDataEvents.SkillInfoEvents);
            //
            operation.UpdateProgressWithCancellationCheck("Creating Cast Events");
            List<AnimatedCastEvent> animatedCastData = EventFactory.CreateCastEvents(castCombatEvents, agentData, skillData, fightData);
            _weaponSwapData = wepSwaps.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            _animatedCastData = animatedCastData.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            _instantCastData = new Dictionary<Agent, List<InstantCastEvent>>();
            _animatedCastDataById = animatedCastData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            //
            operation.UpdateProgressWithCancellationCheck("Creating Buff Events");
            _buffDataByDst = buffEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _buffData = buffEvents.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            // damage events
            operation.UpdateProgressWithCancellationCheck("Creating Damage Events");
            _damageData = damageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _damageTakenData = damageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _damageDataById = damageData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            _breakbarDamageData = brkDamageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _breakbarDamageTakenData = brkDamageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _buffRemoveAllData = _buffData.ToDictionary(x => x.Key, x => x.Value.OfType<BuffRemoveAllEvent>().ToList());
            //
            /*healing_data = allCombatItems.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = allCombatItems.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
            operation.UpdateProgressWithCancellationCheck("Checking CM");
            fightData.SetCM(this, agentData, fightData);
            foreach (AbstractExtensionHandler handler in extensions.Values)
            {
                handler.AttachToCombatData(this, operation, GetBuildEvent().Build);
            }
            EIExtraEventProcess(players, skillData, agentData, fightData, operation, evtcVersion);
        }

        // getters

        public IReadOnlyCollection<long> GetSkills()
        {
            return _skillIds;
        }

        public IReadOnlyList<AliveEvent> GetAliveEvents(Agent key)
        {
            if (_statusEvents.AliveEvents.TryGetValue(key, out List<AliveEvent> list))
            {
                return list;
            }
            return new List<AliveEvent>();
        }

        public IReadOnlyList<AttackTargetEvent> GetAttackTargetEvents(Agent key)
        {
            if (_statusEvents.AttackTargetEvents.TryGetValue(key, out List<AttackTargetEvent> list))
            {
                return list;
            }
            return new List<AttackTargetEvent>();
        }

        public IReadOnlyList<DeadEvent> GetDeadEvents(Agent key)
        {
            if (_statusEvents.DeadEvents.TryGetValue(key, out List<DeadEvent> list))
            {
                return list;
            }
            return new List<DeadEvent>();
        }

        public IReadOnlyList<DespawnEvent> GetDespawnEvents(Agent key)
        {
            if (_statusEvents.DespawnEvents.TryGetValue(key, out List<DespawnEvent> list))
            {
                return list;
            }
            return new List<DespawnEvent>();
        }

        public IReadOnlyList<DownEvent> GetDownEvents(Agent key)
        {
            if (_statusEvents.DownEvents.TryGetValue(key, out List<DownEvent> list))
            {
                return list;
            }
            return new List<DownEvent>();
        }

        public IReadOnlyList<EnterCombatEvent> GetEnterCombatEvents(Agent key)
        {
            if (_statusEvents.EnterCombatEvents.TryGetValue(key, out List<EnterCombatEvent> list))
            {
                return list;
            }
            return new List<EnterCombatEvent>();
        }

        public IReadOnlyList<ExitCombatEvent> GetExitCombatEvents(Agent key)
        {
            if (_statusEvents.ExitCombatEvents.TryGetValue(key, out List<ExitCombatEvent> list))
            {
                return list;
            }
            return new List<ExitCombatEvent>();
        }

        public IReadOnlyList<GuildEvent> GetGuildEvents(Agent key)
        {
            if (_metaDataEvents.GuildEvents.TryGetValue(key, out List<GuildEvent> list))
            {
                return list;
            }
            return new List<GuildEvent>();
        }

        public IReadOnlyList<HealthUpdateEvent> GetHealthUpdateEvents(Agent key)
        {
            if (_statusEvents.HealthUpdateEvents.TryGetValue(key, out List<HealthUpdateEvent> list))
            {
                return list;
            }
            return new List<HealthUpdateEvent>();
        }

        public IReadOnlyList<BarrierUpdateEvent> GetBarrierUpdateEvents(Agent key)
        {
            if (_statusEvents.BarrierUpdateEvents.TryGetValue(key, out List<BarrierUpdateEvent> list))
            {
                return list;
            }
            return new List<BarrierUpdateEvent>();
        }

        public IReadOnlyList<MaxHealthUpdateEvent> GetMaxHealthUpdateEvents(Agent key)
        {
            if (_statusEvents.MaxHealthUpdateEvents.TryGetValue(key, out List<MaxHealthUpdateEvent> list))
            {
                return list;
            }
            return new List<MaxHealthUpdateEvent>();
        }

        public PointOfViewEvent GetPointOfViewEvent()
        {
            return _metaDataEvents.PointOfViewEvent;
        }

        public IReadOnlyList<SpawnEvent> GetSpawnEvents(Agent key)
        {
            if (_statusEvents.SpawnEvents.TryGetValue(key, out List<SpawnEvent> list))
            {
                return list;
            }
            return new List<SpawnEvent>();
        }

        public IReadOnlyList<TargetableEvent> GetTargetableEvents(Agent key)
        {
            if (_statusEvents.TargetableEvents.TryGetValue(key, out List<TargetableEvent> list))
            {
                return list;
            }
            return new List<TargetableEvent>();
        }

        /*public IReadOnlyList<TagEvent> GetTagEvents(AgentItem key)
        {
            if (_statusEvents.TagEvents.TryGetValue(key, out List<TagEvent> list))
            {
                return list;
            }
            return new List<TagEvent>();
        }*/

        public IReadOnlyList<TeamChangeEvent> GetTeamChangeEvents(Agent key)
        {
            if (_statusEvents.TeamChangeEvents.TryGetValue(key, out List<TeamChangeEvent> list))
            {
                return list;
            }
            return new List<TeamChangeEvent>();
        }

        public IReadOnlyList<BreakbarStateEvent> GetBreakbarStateEvents(Agent key)
        {
            if (_statusEvents.BreakbarStateEvents.TryGetValue(key, out List<BreakbarStateEvent> list))
            {
                return list;
            }
            return new List<BreakbarStateEvent>();
        }

        public IReadOnlyList<BreakbarPercentEvent> GetBreakbarPercentEvents(Agent key)
        {
            if (_statusEvents.BreakbarPercentEvents.TryGetValue(key, out List<BreakbarPercentEvent> list))
            {
                return list;
            }
            return new List<BreakbarPercentEvent>();
        }

        public BuildEvent GetBuildEvent()
        {
            return _metaDataEvents.BuildEvent;
        }

        public LanguageEvent GetLanguageEvent()
        {
            return _metaDataEvents.LanguageEvent;
        }

        public LogStartEvent GetLogStartEvent()
        {
            return _metaDataEvents.LogStartEvent;
        }

        public LogEndEvent GetLogEndEvent()
        {
            return _metaDataEvents.LogEndEvent;
        }

        public IReadOnlyList<MapIDEvent> GetMapIDEvents()
        {
            return _metaDataEvents.MapIDEvents;
        }

        public IReadOnlyList<RewardEvent> GetRewardEvents()
        {
            return _rewardEvents;
        }

        public IReadOnlyList<ErrorEvent> GetErrorEvents()
        {
            return _metaDataEvents.ErrorEvents;
        }

        public IReadOnlyList<ShardEvent> GetShardEvents()
        {
            return _metaDataEvents.ShardEvents;
        }

        public BuffInfoEvent GetBuffInfoEvent(long buffID)
        {
            if (_metaDataEvents.BuffInfoEvents.TryGetValue(buffID, out BuffInfoEvent evt))
            {
                return evt;
            }
            return null;
        }

        public IReadOnlyList<BuffInfoEvent> GetBuffInfoEvent(ArcDPSEnums.BuffCategory category)
        {
            if (_metaDataEvents.BuffInfoEventsByCategory.TryGetValue(category, out List<BuffInfoEvent> evts))
            {
                return evts;
            }
            return new List<BuffInfoEvent>();
        }

        public SkillInfoEvent GetSkillInfoEvent(long skillID)
        {
            if (_metaDataEvents.SkillInfoEvents.TryGetValue(skillID, out SkillInfoEvent evt))
            {
                return evt;
            }
            return null;
        }

        public IReadOnlyList<AbstractBuffEvent> GetBuffData(long key)
        {
            if (_buffData.TryGetValue(key, out List<AbstractBuffEvent> res))
            {
                return res;
            }
            return new List<AbstractBuffEvent>();
        }

        public IReadOnlyList<BuffRemoveAllEvent> GetBuffRemoveAllData(long key)
        {
            if (_buffRemoveAllData.TryGetValue(key, out List<BuffRemoveAllEvent> res))
            {
                return res;
            }
            return new List<BuffRemoveAllEvent>();
        }

        /// <summary>
        /// Returns list of buff events applied on agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractBuffEvent> GetBuffData(Agent key)
        {
            if (_buffDataByDst.TryGetValue(key, out List<AbstractBuffEvent> res))
            {
                return res;
            }
            return new List<AbstractBuffEvent>();
        }

        /// <summary>
        /// Returns list of damage events done by agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageData(Agent key)
        {
            if (_damageData.TryGetValue(key, out List<AbstractHealthDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractHealthDamageEvent>();
        }

        /// <summary>
        /// Returns list of breakbar damage events done by agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageData(Agent key)
        {
            if (_breakbarDamageData.TryGetValue(key, out List<AbstractBreakbarDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractBreakbarDamageEvent>();
        }

        /// <summary>
        /// Returns list of damage events applied by a skill
        /// </summary>
        /// <param name="key"></param> Id of the skill
        /// <returns></returns>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageData(long key)
        {
            if (_damageDataById.TryGetValue(key, out List<AbstractHealthDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractHealthDamageEvent>();
        }

        /// <summary>
        /// Returns list of animated cast events done by Agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastData(Agent key)
        {
            if (_animatedCastData.TryGetValue(key, out List<AnimatedCastEvent> res))
            {
                return res;
            }
            return new List<AnimatedCastEvent>();
        }

        /// <summary>
        /// Returns list of instant cast events done by Agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<InstantCastEvent> GetInstantCastData(Agent key)
        {
            if (_instantCastData.TryGetValue(key, out List<InstantCastEvent> res))
            {
                return res;
            }
            return new List<InstantCastEvent>();
        }

        /// <summary>
        /// Returns list of weapon swap events done by Agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<WeaponSwapEvent> GetWeaponSwapData(Agent key)
        {
            if (_weaponSwapData.TryGetValue(key, out List<WeaponSwapEvent> res))
            {
                return res;
            }
            return new List<WeaponSwapEvent>();
        }

        /// <summary>
        /// Returns list of cast events from skill
        /// </summary>
        /// <param name="key"></param> ID of the skill
        /// <returns></returns>
        public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastData(long key)
        {
            if (_animatedCastDataById.TryGetValue(key, out List<AnimatedCastEvent> res))
            {
                return res;
            }
            return new List<AnimatedCastEvent>();
        }

        /// <summary>
        /// Returns list of damage taken events by Agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageTakenData(Agent key)
        {
            if (_damageTakenData.TryGetValue(key, out List<AbstractHealthDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractHealthDamageEvent>();
        }

        /// <summary>
        /// Returns list of breakbar damage taken events by Agent
        /// </summary>
        /// <param name="key"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenData(Agent key)
        {
            if (_breakbarDamageTakenData.TryGetValue(key, out List<AbstractBreakbarDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractBreakbarDamageEvent>();
        }

        /*public IReadOnlyList<CombatItem> getHealingData()
        {
            return _healingData;
        }

        public IReadOnlyList<CombatItem> getHealingReceivedData()
        {
            return _healingReceivedData;
        }*/


        public IReadOnlyList<AbstractMovementEvent> GetMovementData(Agent key)
        {
            if (_statusEvents.MovementEvents.TryGetValue(key, out List<AbstractMovementEvent> res))
            {
                return res;
            }
            return new List<AbstractMovementEvent>();
        }
    }
}
