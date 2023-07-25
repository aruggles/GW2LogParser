﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;

namespace GW2EIEvtcParser.ParsedData
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
        private readonly Dictionary<AgentItem, List<AbstractBuffEvent>> _buffDataByDst;
        private readonly Dictionary<AgentItem, List<AbstractHealthDamageEvent>> _damageData;
        private readonly Dictionary<AgentItem, List<AbstractBreakbarDamageEvent>> _breakbarDamageData;
        private readonly Dictionary<long, List<AbstractBreakbarDamageEvent>> _breakbarDamageDataById;
        private readonly Dictionary<long, List<AbstractHealthDamageEvent>> _damageDataById;
        private readonly Dictionary<AgentItem, List<AnimatedCastEvent>> _animatedCastData;
        private readonly Dictionary<AgentItem, List<InstantCastEvent>> _instantCastData;
        private readonly Dictionary<AgentItem, List<WeaponSwapEvent>> _weaponSwapData;
        private readonly Dictionary<long, List<AnimatedCastEvent>> _animatedCastDataById;
        private readonly Dictionary<long, List<InstantCastEvent>> _instantCastDataById;
        private readonly Dictionary<AgentItem, List<AbstractHealthDamageEvent>> _damageTakenData;
        private readonly Dictionary<AgentItem, List<AbstractBreakbarDamageEvent>> _breakbarDamageTakenData;
        private readonly List<RewardEvent> _rewardEvents = new List<RewardEvent>();
        // EXTENSIONS
        public EXTHealingCombatData EXTHealingCombatData { get; internal set; }
        public EXTBarrierCombatData EXTBarrierCombatData { get; internal set; }
        public bool HasEXTHealing => EXTHealingCombatData != null;
        public bool HasEXTBarrier => EXTBarrierCombatData != null;

        internal bool UseBuffInstanceSimulator { get; } = false;

        internal bool HasStackIDs { get; }

        public bool HasBreakbarDamageData { get; } = false;
        public bool HasEffectData { get; } = false;

        private void EIBuffParse(IReadOnlyList<Player> players, SkillData skillData, FightData fightData)
        {
            var toAdd = new List<AbstractBuffEvent>();
            foreach (Player p in players)
            {
                if (p.Spec == ParserHelper.Spec.Weaver)
                {
                    toAdd.AddRange(WeaverHelper.TransformWeaverAttunements(GetBuffData(p.AgentItem), _buffData, p.AgentItem, skillData));
                }
                if (p.Spec == ParserHelper.Spec.Virtuoso)
                {
                    toAdd.AddRange(VirtuosoHelper.TransformVirtuosoBladeStorage(GetBuffData(p.AgentItem), p.AgentItem, skillData));
                }
                if (p.BaseSpec == ParserHelper.Spec.Elementalist && p.Spec != ParserHelper.Spec.Weaver)
                {
                    ElementalistHelper.RemoveDualBuffs(GetBuffData(p.AgentItem), _buffData, skillData);
                }
            }
            toAdd.AddRange(fightData.Logic.SpecialBuffEventProcess(this, skillData));
            var buffIDsToSort = new HashSet<long>();
            var buffAgentsToSort = new HashSet<AgentItem>();
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
            foreach (AgentItem a in buffAgentsToSort)
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
            var dstToSort = new HashSet<AgentItem>();
            var srcToSort = new HashSet<AgentItem>();
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
            foreach (AgentItem a in dstToSort)
            {
                _damageTakenData[a] = _damageTakenData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (AgentItem a in srcToSort)
            {
                _damageData[a] = _damageData[a].OrderBy(x => x.Time).ToList();
            }
        }

        private IReadOnlyList<InstantCastEvent> ComputeInstantCastEventsFromFinders(AgentData agentData, SkillData skillData, IReadOnlyList<InstantCastFinder> instantCastFinders)
        {
            var res = new List<InstantCastEvent>();
            foreach (InstantCastFinder icf in instantCastFinders)
            {
                if (icf.Available(this))
                {
                    if (icf.NotAccurate)
                    {
                        skillData.NotAccurate.Add(icf.SkillID);
                    }
                    switch(icf.CastOrigin)
                    {
                        case InstantCastFinder.InstantCastOrigin.Trait:
                            skillData.TraitProc.Add(icf.SkillID);
                            break;
                        case InstantCastFinder.InstantCastOrigin.Gear:
                            skillData.GearProc.Add(icf.SkillID);
                            break;
                        case InstantCastFinder.InstantCastOrigin.Skill:
                        default:
                            break;
                    }
                    res.AddRange(icf.ComputeInstantCast(this, skillData, agentData));
                }
            }
            return res;
        }
        private void EICastParse(IReadOnlyList<Player> players, SkillData skillData, FightData fightData, AgentData agentData)
        {
            List<AbstractCastEvent> toAdd = fightData.Logic.SpecialCastEventProcess(this, skillData);
            foreach (Player p in players) {
                switch(p.Spec)
                {
                    case ParserHelper.Spec.Willbender:
                        toAdd.AddRange(WillbenderHelper.ComputeFlowingResolveCastEvents(p, this, skillData, agentData));
                        break;
                    default:
                        break;
                }
                switch(p.BaseSpec)
                {
                    case ParserHelper.Spec.Ranger:
                        toAdd.AddRange(RangerHelper.ComputeAncestralGraceCastEvents(p, this, skillData, agentData));
                        break;
                    default:
                        break;
                }
            }
            //
            var castIDsToSort = new HashSet<long>();
            var castAgentsToSort = new HashSet<AgentItem>();
            var wepSwapAgentsToSort = new HashSet<AgentItem>();
            var instantAgentsToSort = new HashSet<AgentItem>();
            var instantIDsToSort = new HashSet<long>();
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
                        _animatedCastDataById[ace.SkillId] = new List<AnimatedCastEvent>()
                        {
                            ace
                        };
                    }
                    castIDsToSort.Add(ace.SkillId);
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
            }
            var instantCastsFinder = new HashSet<InstantCastFinder>(ProfHelper.GetProfessionInstantCastFinders(players));
            fightData.Logic.GetInstantCastFinders().ForEach(x => instantCastsFinder.Add(x));
            //
            foreach (InstantCastEvent ice in ComputeInstantCastEventsFromFinders(agentData, skillData, instantCastsFinder.ToList()))
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
                if (_instantCastDataById.TryGetValue(ice.SkillId, out List<InstantCastEvent> instantCastListByID))
                {
                    instantCastListByID.Add(ice);
                }
                else
                {
                    _instantCastDataById[ice.SkillId] = new List<InstantCastEvent>()
                        {
                            ice
                        };
                }
                instantIDsToSort.Add(ice.SkillId);
            }
            foreach (long castID in castIDsToSort)
            {
                _animatedCastDataById[castID] = _animatedCastDataById[castID].OrderBy(x => x.Time).ToList();
            }
            foreach (AgentItem a in castAgentsToSort)
            {
                _animatedCastData[a] = _animatedCastData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (AgentItem a in wepSwapAgentsToSort)
            {
                _weaponSwapData[a] = _weaponSwapData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (AgentItem a in instantAgentsToSort)
            {
                _instantCastData[a] = _instantCastData[a].OrderBy(x => x.Time).ToList();
            }
            foreach (long instantID in instantIDsToSort)
            {
                _instantCastDataById[instantID] = _instantCastDataById[instantID].OrderBy(x => x.Time).ToList();
            }
        }

        private void EIMetaAndStatusParse(FightData fightData, int arcdpsVersion)
        {
            foreach (KeyValuePair<AgentItem, List<AbstractHealthDamageEvent>> pair in _damageTakenData)
            {
                if (pair.Key.IsSpecies(ArcDPSEnums.TargetID.WorldVersusWorld))
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

        private void EIExtraEventProcess(IReadOnlyList<Player> players, SkillData skillData, AgentData agentData, FightData fightData, ParserController operation, int arcdpsVersion)
        {
            // Add missing breakbar active state
            foreach (KeyValuePair<AgentItem, List<BreakbarStateEvent>> pair in _statusEvents.BreakbarStateEvents)
            {
                BreakbarStateEvent first = pair.Value.FirstOrDefault();
                if (first != null && first.State != ArcDPSEnums.BreakbarState.Active && first.Time > pair.Key.FirstAware + 500)
                {
                    pair.Value.Insert(0, new BreakbarStateEvent(pair.Key, pair.Key.FirstAware, ArcDPSEnums.BreakbarState.Active));
                }
            }
            // master attachements
            operation.UpdateProgressWithCancellationCheck("Processing Warrior Gadgets");
            WarriorHelper.ProcessGadgets(players, this);
            operation.UpdateProgressWithCancellationCheck("Processing Engineer Gadgets");
            EngineerHelper.ProcessGadgets(players, this);
            operation.UpdateProgressWithCancellationCheck("Attaching Ranger Gadgets");
            RangerHelper.ProcessGadgets(players, this);
            operation.UpdateProgressWithCancellationCheck("Processing Revenant Gadgets");
            RevenantHelper.ProcessGadgets(players, this, agentData);
            operation.UpdateProgressWithCancellationCheck("Processing Racial Gadget");
            ProfHelper.ProcessRacialGadgets(players, this);
            // Custom events
            operation.UpdateProgressWithCancellationCheck("Creating Custom Buff Events");
            EIBuffParse(players, skillData, fightData);
            operation.UpdateProgressWithCancellationCheck("Creating Custom Damage Events");
            EIDamageParse(skillData, fightData);
            operation.UpdateProgressWithCancellationCheck("Creating Custom Cast Events");
            EICastParse(players, skillData, fightData, agentData);
            operation.UpdateProgressWithCancellationCheck("Creating Custom Status Events");
            EIMetaAndStatusParse(fightData, arcdpsVersion);
        }

        internal CombatData(List<CombatItem> allCombatItems, FightData fightData, AgentData agentData, SkillData skillData, IReadOnlyList<Player> players, ParserController operation, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, int evtcVersion)
        {
            var combatEvents = allCombatItems.OrderBy(x => x.Time).ToList();
            _skillIds = new HashSet<long>();
            var castCombatEvents = new Dictionary<ulong, List<CombatItem>>();
            var buffEvents = new List<AbstractBuffEvent>();
            var wepSwaps = new List<WeaponSwapEvent>();
            var brkDamageData = new List<AbstractBreakbarDamageEvent>();
            var damageData = new List<AbstractHealthDamageEvent>();
            operation.UpdateProgressWithCancellationCheck("Creating EI Combat Data");
            foreach (CombatItem combatItem in combatEvents)
            {
                bool insertToSkillIDs = false;
                if (combatItem.IsStateChange != ArcDPSEnums.StateChange.None)
                {
                    if (combatItem.IsExtension)
                    {
                        if (extensions.TryGetValue(combatItem.Pad, out AbstractExtensionHandler handler))
                        {
                            insertToSkillIDs = handler.IsSkillID(combatItem);
                            handler.InsertEIExtensionEvent(combatItem, agentData, skillData);
                        }
                    } 
                    else
                    {
                        insertToSkillIDs = combatItem.IsStateChange == ArcDPSEnums.StateChange.BuffInitial;
                        CombatEventFactory.AddStateChangeEvent(combatItem, agentData, skillData, _metaDataEvents, _statusEvents, _rewardEvents, wepSwaps, buffEvents, evtcVersion);
                    }
                    
                }
                else if (combatItem.IsActivation != ArcDPSEnums.Activation.None)
                {
                    insertToSkillIDs = true;
                    if (castCombatEvents.TryGetValue(combatItem.SrcAgent, out List<CombatItem> list))
                    {
                        list.Add(combatItem);
                    }
                    else
                    {
                        castCombatEvents[combatItem.SrcAgent] = new List<CombatItem>() { combatItem };
                    }
                }
                else if (combatItem.IsBuffRemove != ArcDPSEnums.BuffRemove.None)
                {
                    insertToSkillIDs = true;
                    CombatEventFactory.AddBuffRemoveEvent(combatItem, buffEvents, agentData, skillData);
                }
                else
                {
                    insertToSkillIDs = true;
                    if (combatItem.IsBuff != 0 && combatItem.BuffDmg == 0 && combatItem.Value > 0)
                    {
                        CombatEventFactory.AddBuffApplyEvent(combatItem, buffEvents, agentData, skillData);
                    }
                    else if (combatItem.IsBuff == 0)
                    {
                        CombatEventFactory.AddDirectDamageEvent(combatItem, damageData, brkDamageData, agentData, skillData);
                    }
                    else if (combatItem.IsBuff != 0 && combatItem.Value == 0)
                    {
                        CombatEventFactory.AddIndirectDamageEvent(combatItem, damageData, brkDamageData, agentData, skillData);
                    }
                }
                if (insertToSkillIDs)
                {
                    _skillIds.Add(combatItem.SkillID);
                }
            }
            HasStackIDs = evtcVersion > ArcDPSEnums.ArcDPSBuilds.ProperConfusionDamageSimulation && buffEvents.Any(x => x is BuffStackActiveEvent || x is BuffStackResetEvent);
            UseBuffInstanceSimulator = HasStackIDs && false;// (fightData.Logic.Mode == EncounterLogic.FightLogic.ParseMode.Instanced10 || fightData.Logic.Mode == EncounterLogic.FightLogic.ParseMode.Instanced5 || fightData.Logic.Mode == EncounterLogic.FightLogic.ParseMode.Benchmark);
            HasMovementData = _statusEvents.MovementEvents.Count > 1;
            HasBreakbarDamageData = brkDamageData.Any();
            HasEffectData = _statusEvents.EffectEvents.Any();
            //
            operation.UpdateProgressWithCancellationCheck("Combining SkillInfo with SkillData");
            skillData.CombineWithSkillInfo(_metaDataEvents.SkillInfoEvents);
            //
            operation.UpdateProgressWithCancellationCheck("Creating Cast Events");
            List<AnimatedCastEvent> animatedCastData = CombatEventFactory.CreateCastEvents(castCombatEvents, agentData, skillData, fightData);
            _weaponSwapData = wepSwaps.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            _animatedCastData = animatedCastData.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            _instantCastData = new Dictionary<AgentItem, List<InstantCastEvent>>();
            _instantCastDataById = new Dictionary<long, List<InstantCastEvent>>();
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
            _breakbarDamageDataById = brkDamageData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            _breakbarDamageTakenData = brkDamageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _buffRemoveAllData = _buffData.ToDictionary(x => x.Key, x => x.Value.OfType<BuffRemoveAllEvent>().ToList());
            //
            /*healing_data = allCombatItems.Where(x => x.getDstInstid() != 0 && x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                         ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                         (x.isBuff() == 0 && x.getValue() > 0))).ToList();

            healing_received_data = allCombatItems.Where(x => x.isStateChange() == ParseEnum.StateChange.Normal && x.getIFF() == ParseEnum.IFF.Friend && x.isBuffremove() == ParseEnum.BuffRemove.None &&
                                            ((x.isBuff() == 1 && x.getBuffDmg() > 0 && x.getValue() == 0) ||
                                                (x.isBuff() == 0 && x.getValue() >= 0))).ToList();*/
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

        public IReadOnlyList<AliveEvent> GetAliveEvents(AgentItem src)
        {
            if (_statusEvents.AliveEvents.TryGetValue(src, out List<AliveEvent> list))
            {
                return list;
            }
            return new List<AliveEvent>();
        }

        public IReadOnlyList<AttackTargetEvent> GetAttackTargetEvents(AgentItem src)
        {
            if (_statusEvents.AttackTargetEvents.TryGetValue(src, out List<AttackTargetEvent> list))
            {
                return list;
            }
            return new List<AttackTargetEvent>();
        }

        public IReadOnlyList<AttackTargetEvent> GetAttackTargetEventsByAttackTarget(AgentItem attackTarget)
        {
            if (_statusEvents.AttackTargetEventsByAttackTarget.TryGetValue(attackTarget, out List<AttackTargetEvent> list))
            {
                return list;
            }
            return new List<AttackTargetEvent>();
        }

        public IReadOnlyList<DeadEvent> GetDeadEvents(AgentItem src)
        {
            if (_statusEvents.DeadEvents.TryGetValue(src, out List<DeadEvent> list))
            {
                return list;
            }
            return new List<DeadEvent>();
        }

        public IReadOnlyList<DespawnEvent> GetDespawnEvents(AgentItem src)
        {
            if (_statusEvents.DespawnEvents.TryGetValue(src, out List<DespawnEvent> list))
            {
                return list;
            }
            return new List<DespawnEvent>();
        }

        public IReadOnlyList<DownEvent> GetDownEvents(AgentItem src)
        {
            if (_statusEvents.DownEvents.TryGetValue(src, out List<DownEvent> list))
            {
                return list;
            }
            return new List<DownEvent>();
        }

        public IReadOnlyList<EnterCombatEvent> GetEnterCombatEvents(AgentItem src)
        {
            if (_statusEvents.EnterCombatEvents.TryGetValue(src, out List<EnterCombatEvent> list))
            {
                return list;
            }
            return new List<EnterCombatEvent>();
        }

        public IReadOnlyList<ExitCombatEvent> GetExitCombatEvents(AgentItem src)
        {
            if (_statusEvents.ExitCombatEvents.TryGetValue(src, out List<ExitCombatEvent> list))
            {
                return list;
            }
            return new List<ExitCombatEvent>();
        }

        public IReadOnlyList<GuildEvent> GetGuildEvents(AgentItem src)
        {
            if (_metaDataEvents.GuildEvents.TryGetValue(src, out List<GuildEvent> list))
            {
                return list;
            }
            return new List<GuildEvent>();
        }

        public IReadOnlyList<HealthUpdateEvent> GetHealthUpdateEvents(AgentItem src)
        {
            if (_statusEvents.HealthUpdateEvents.TryGetValue(src, out List<HealthUpdateEvent> list))
            {
                return list;
            }
            return new List<HealthUpdateEvent>();
        }

        public IReadOnlyList<BarrierUpdateEvent> GetBarrierUpdateEvents(AgentItem src)
        {
            if (_statusEvents.BarrierUpdateEvents.TryGetValue(src, out List<BarrierUpdateEvent> list))
            {
                return list;
            }
            return new List<BarrierUpdateEvent>();
        }

        public IReadOnlyList<MaxHealthUpdateEvent> GetMaxHealthUpdateEvents(AgentItem src)
        {
            if (_statusEvents.MaxHealthUpdateEvents.TryGetValue(src, out List<MaxHealthUpdateEvent> list))
            {
                return list;
            }
            return new List<MaxHealthUpdateEvent>();
        }

        public PointOfViewEvent GetPointOfViewEvent()
        {
            return _metaDataEvents.PointOfViewEvent;
        }

        public FractalScaleEvent GetFractalScaleEvent()
        {
            return _metaDataEvents.FractalScaleEvent;
        }

        public IReadOnlyList<SpawnEvent> GetSpawnEvents(AgentItem src)
        {
            if (_statusEvents.SpawnEvents.TryGetValue(src, out List<SpawnEvent> list))
            {
                return list;
            }
            return new List<SpawnEvent>();
        }

        public IReadOnlyList<TargetableEvent> GetTargetableEvents(AgentItem attackTarget)
        {
            if (_statusEvents.TargetableEvents.TryGetValue(attackTarget, out List<TargetableEvent> list))
            {
                return list;
            }
            return new List<TargetableEvent>();
        }

        public IReadOnlyList<TagEvent> GetTagEvents(AgentItem key)
        {
            if (_statusEvents.TagEvents.TryGetValue(key, out List<TagEvent> list))
            {
                return list;
            }
            return new List<TagEvent>();
        }

        public IReadOnlyList<TeamChangeEvent> GetTeamChangeEvents(AgentItem src)
        {
            if (_statusEvents.TeamChangeEvents.TryGetValue(src, out List<TeamChangeEvent> list))
            {
                return list;
            }
            return new List<TeamChangeEvent>();
        }

        public IReadOnlyList<BreakbarStateEvent> GetBreakbarStateEvents(AgentItem src)
        {
            if (_statusEvents.BreakbarStateEvents.TryGetValue(src, out List<BreakbarStateEvent> list))
            {
                return list;
            }
            return new List<BreakbarStateEvent>();
        }

        public IReadOnlyList<BreakbarPercentEvent> GetBreakbarPercentEvents(AgentItem src)
        {
            if (_statusEvents.BreakbarPercentEvents.TryGetValue(src, out List<BreakbarPercentEvent> list))
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

        public InstanceStartEvent GetInstanceStartEvent()
        {
            return _metaDataEvents.InstanceStartEvent;
        }

        public LogStartEvent GetLogStartEvent()
        {
            return _metaDataEvents.LogStartEvent;
        }

        public IReadOnlyList<LogStartNPCUpdateEvent> GetLogStartNPCUpdateEvents()
        {
            return _metaDataEvents.LogStartNPCUpdateEvents;
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

        public IReadOnlyList<TickRateEvent> GetTickRateEvents()
        {
            return _metaDataEvents.TickRateEvents;
        }

        public BuffInfoEvent GetBuffInfoEvent(long buffID)
        {
            if (_metaDataEvents.BuffInfoEvents.TryGetValue(buffID, out BuffInfoEvent evt))
            {
                return evt;
            }
            return null;
        }

        public IReadOnlyList<BuffInfoEvent> GetBuffInfoEvent(byte category)
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

        public IReadOnlyList<Last90BeforeDownEvent> GetLast90BeforeDownEvents()
        {
            return _statusEvents.Last90BeforeDownEvents;
        }

        public IReadOnlyList<Last90BeforeDownEvent> GetLast90BeforeDownEvents(AgentItem src)
        {
            if (_statusEvents.Last90BeforeDownEventsBySrc.TryGetValue(src, out List<Last90BeforeDownEvent> res))
            {
                return res;
            }
            return new List<Last90BeforeDownEvent>();
        }

        public IReadOnlyList<AbstractBuffEvent> GetBuffData(long buffID)
        {
            if (_buffData.TryGetValue(buffID, out List<AbstractBuffEvent> res))
            {
                return res;
            }
            return new List<AbstractBuffEvent>();
        }

        public IReadOnlyList<BuffRemoveAllEvent> GetBuffRemoveAllData(long buffID)
        {
            if (_buffRemoveAllData.TryGetValue(buffID, out List<BuffRemoveAllEvent> res))
            {
                return res;
            }
            return new List<BuffRemoveAllEvent>();
        }

        /// <summary>
        /// Returns list of buff events applied on agent
        /// </summary>
        /// <param name="dst"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractBuffEvent> GetBuffData(AgentItem dst)
        {
            if (_buffDataByDst.TryGetValue(dst, out List<AbstractBuffEvent> res))
            {
                return res;
            }
            return new List<AbstractBuffEvent>();
        }

        /// <summary>
        /// Returns list of damage events done by agent
        /// </summary>
        /// <param name="src"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageData(AgentItem src)
        {
            if (_damageData.TryGetValue(src, out List<AbstractHealthDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractHealthDamageEvent>();
        }

        /// <summary>
        /// Returns list of breakbar damage events done by agent
        /// </summary>
        /// <param name="src"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageData(AgentItem src)
        {
            if (_breakbarDamageData.TryGetValue(src, out List<AbstractBreakbarDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractBreakbarDamageEvent>();
        }

        /// <summary>
        /// Returns list of breakbar damage events done by skill id
        /// </summary>
        /// <param name="long"></param> ID
        /// <returns></returns>
        public IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageData(long skillID)
        {
            if (_breakbarDamageDataById.TryGetValue(skillID, out List<AbstractBreakbarDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractBreakbarDamageEvent>();
        }

        /// <summary>
        /// Returns list of damage events applied by a skill
        /// </summary>
        /// <param name="skillID"></param> Id of the skill
        /// <returns></returns>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageData(long skillID)
        {
            if (_damageDataById.TryGetValue(skillID, out List<AbstractHealthDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractHealthDamageEvent>();
        }

        /// <summary>
        /// Returns list of animated cast events done by Agent
        /// </summary>
        /// <param name="caster"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastData(AgentItem caster)
        {
            if (_animatedCastData.TryGetValue(caster, out List<AnimatedCastEvent> res))
            {
                return res;
            }
            return new List<AnimatedCastEvent>();
        }

        /// <summary>
        /// Returns list of instant cast events done by Agent
        /// </summary>
        /// <param name="caster"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<InstantCastEvent> GetInstantCastData(AgentItem caster)
        {
            if (_instantCastData.TryGetValue(caster, out List<InstantCastEvent> res))
            {
                return res;
            }
            return new List<InstantCastEvent>();
        }

        /// <summary>
        /// Returns list of instant cast events done by Agent
        /// </summary>
        /// <param name="caster"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<InstantCastEvent> GetInstantCastData(long skillID)
        {
            if (_instantCastDataById.TryGetValue(skillID, out List<InstantCastEvent> res))
            {
                return res;
            }
            return new List<InstantCastEvent>();
        }

        /// <summary>
        /// Returns list of weapon swap events done by Agent
        /// </summary>
        /// <param name="caster"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<WeaponSwapEvent> GetWeaponSwapData(AgentItem caster)
        {
            if (_weaponSwapData.TryGetValue(caster, out List<WeaponSwapEvent> res))
            {
                return res;
            }
            return new List<WeaponSwapEvent>();
        }

        /// <summary>
        /// Returns list of cast events from skill
        /// </summary>
        /// <param name="skillID"></param> ID of the skill
        /// <returns></returns>
        public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastData(long skillID)
        {
            if (_animatedCastDataById.TryGetValue(skillID, out List<AnimatedCastEvent> res))
            {
                return res;
            }
            return new List<AnimatedCastEvent>();
        }

        /// <summary>
        /// Returns list of damage taken events by Agent
        /// </summary>
        /// <param name="dst"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageTakenData(AgentItem dst)
        {
            if (_damageTakenData.TryGetValue(dst, out List<AbstractHealthDamageEvent> res))
            {
                return res;
            }
            return new List<AbstractHealthDamageEvent>();
        }

        /// <summary>
        /// Returns list of breakbar damage taken events by Agent
        /// </summary>
        /// <param name="dst"></param> Agent
        /// <returns></returns>
        public IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenData(AgentItem dst)
        {
            if (_breakbarDamageTakenData.TryGetValue(dst, out List<AbstractBreakbarDamageEvent> res))
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


        public IReadOnlyList<AbstractMovementEvent> GetMovementData(AgentItem src)
        {
            if (_statusEvents.MovementEvents.TryGetValue(src, out List<AbstractMovementEvent> res))
            {
                return res;
            }
            return new List<AbstractMovementEvent>();
        }

        public IReadOnlyList<EffectEvent> GetEffectEvents(AgentItem src)
        {
            if (_statusEvents.EffectEventsBySrc.TryGetValue(src, out List<EffectEvent> list))
            {
                return list;
            }
            return new List<EffectEvent>();
        }

        public IReadOnlyList<EffectEvent> GetEffectEventsByDst(AgentItem dst)
        {
            if (_statusEvents.EffectEventsByDst.TryGetValue(dst, out List<EffectEvent> list))
            {
                return list;
            }
            return new List<EffectEvent>();
        }

        public IReadOnlyList<EffectEvent> GetEffectEventsByEffectID(long effectID)
        {
            if (_statusEvents.EffectEventsByEffectID.TryGetValue(effectID, out List<EffectEvent> list))
            {
                return list;
            }
            return new List<EffectEvent>();
        }

        public IReadOnlyList<EffectEvent> GetEffectEventsByTrackingID(long trackingID)
        {
            if (_statusEvents.EffectEventsByTrackingID.TryGetValue(trackingID, out List<EffectEvent> list))
            {
                return list;
            }
            return new List<EffectEvent>();
        }

        public bool TryGetEffectEventsByGUID(string effectGUID, out IReadOnlyList<EffectEvent> effectEvents)
        {
            EffectGUIDEvent effectGUIDEvent = GetEffectGUIDEvent(effectGUID);
            effectEvents = null;
            if (effectGUIDEvent != null)
            {
                effectEvents = GetEffectEventsByEffectID(effectGUIDEvent.ContentID);
                return true;
            }
            return false;
        }


        public IReadOnlyList<EffectEvent> GetEffectEvents()
        {
            return _statusEvents.EffectEvents;
        }

        public EffectGUIDEvent GetEffectGUIDEvent(string effectGUID)
        {
            if (_metaDataEvents.EffectGUIDEventsByGUID.TryGetValue(effectGUID, out EffectGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }

        public EffectGUIDEvent GetEffectGUIDEvent(long effectID)
        {
            if (_metaDataEvents.EffectGUIDEventsByEffectID.TryGetValue(effectID, out EffectGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }


        public MarkerGUIDEvent GetMarkerGUIDEvent(string markerGUID)
        {
            if (_metaDataEvents.MarkerGUIDEventsByGUID.TryGetValue(markerGUID, out MarkerGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }

        public MarkerGUIDEvent GetMarkerGUIDEvent(long markerID)
        {
            if (_metaDataEvents.MarkerGUIDEventsByMarkerID.TryGetValue(markerID, out MarkerGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }

    }
}
