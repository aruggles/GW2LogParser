﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffStacks;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Events.MetaData;
using Gw2LogParser.Parser.Data.Events.Movement;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.Events
{
    public static class EventFactory
    {
        private static void Add<TKey, TValue>(Dictionary<TKey, List<TValue>> dict, TKey key, TValue evt)
        {
            if (dict.TryGetValue(key, out List<TValue> list))
            {
                list.Add(evt);
            }
            else
            {
                dict[key] = new List<TValue>()
                {
                    evt
                };
            }
        }

        public static void AddStateChangeEvent(Combat stateChangeEvent, AgentData agentData, SkillData skillData, MetaEventsContainer metaDataEvents, StatusEventsContainer statusEvents, List<RewardEvent> rewardEvents, List<WeaponSwapEvent> wepSwaps, List<AbstractBuffEvent> buffEvents)
        {
            switch (stateChangeEvent.IsStateChange)
            {
                case ArcDPSEnums.StateChange.EnterCombat:
                    var enterCombatEvt = new EnterCombatEvent(stateChangeEvent, agentData);
                    Add(statusEvents.EnterCombatEvents, enterCombatEvt.Src, enterCombatEvt);
                    break;
                case ArcDPSEnums.StateChange.ExitCombat:
                    var exitCombatEvt = new ExitCombatEvent(stateChangeEvent, agentData);
                    Add(statusEvents.ExitCombatEvents, exitCombatEvt.Src, exitCombatEvt);
                    break;
                case ArcDPSEnums.StateChange.ChangeUp:
                    var aliveEvt = new AliveEvent(stateChangeEvent, agentData);
                    Add(statusEvents.AliveEvents, aliveEvt.Src, aliveEvt);
                    break;
                case ArcDPSEnums.StateChange.ChangeDead:
                    var deadEvt = new DeadEvent(stateChangeEvent, agentData);
                    Add(statusEvents.DeadEvents, deadEvt.Src, deadEvt);
                    break;
                case ArcDPSEnums.StateChange.ChangeDown:
                    var downEvt = new DownEvent(stateChangeEvent, agentData);
                    Add(statusEvents.DownEvents, downEvt.Src, downEvt);
                    break;
                case ArcDPSEnums.StateChange.Spawn:
                    var spawnEvt = new SpawnEvent(stateChangeEvent, agentData);
                    Add(statusEvents.SpawnEvents, spawnEvt.Src, spawnEvt);
                    break;
                case ArcDPSEnums.StateChange.Despawn:
                    var despawnEvt = new DespawnEvent(stateChangeEvent, agentData);
                    Add(statusEvents.DespawnEvents, despawnEvt.Src, despawnEvt);
                    break;
                case ArcDPSEnums.StateChange.HealthUpdate:
                    var healthEvt = new HealthUpdateEvent(stateChangeEvent, agentData);
                    Add(statusEvents.HealthUpdateEvents, healthEvt.Src, healthEvt);
                    break;
                case ArcDPSEnums.StateChange.BarrierUpdate:
                    var barrierEvt = new BarrierUpdateEvent(stateChangeEvent, agentData);
                    Add(statusEvents.BarrierUpdateEvents, barrierEvt.Src, barrierEvt);
                    break;
                case ArcDPSEnums.StateChange.LogStart:
                    if (stateChangeEvent.Value == 0 || stateChangeEvent.BuffDmg == 0)
                    {
                        return;
                    }
                    metaDataEvents.LogStartEvent = new LogStartEvent(stateChangeEvent);
                    break;
                case ArcDPSEnums.StateChange.LogEnd:
                    if (stateChangeEvent.Value == 0 || stateChangeEvent.BuffDmg == 0)
                    {
                        return;
                    }
                    metaDataEvents.LogEndEvent = new LogEndEvent(stateChangeEvent);
                    break;
                case ArcDPSEnums.StateChange.MaxHealthUpdate:
                    var maxHealthEvt = new MaxHealthUpdateEvent(stateChangeEvent, agentData);
                    Add(statusEvents.MaxHealthUpdateEvents, maxHealthEvt.Src, maxHealthEvt);
                    break;
                case ArcDPSEnums.StateChange.PointOfView:
                    if (stateChangeEvent.SrcAgent == 0)
                    {
                        return;
                    }
                    metaDataEvents.PointOfViewEvent = new PointOfViewEvent(stateChangeEvent, agentData);
                    break;
                case ArcDPSEnums.StateChange.Language:
                    metaDataEvents.LanguageEvent = new LanguageEvent(stateChangeEvent);
                    break;
                case ArcDPSEnums.StateChange.GWBuild:
                    if (stateChangeEvent.SrcAgent == 0)
                    {
                        return;
                    }
                    metaDataEvents.BuildEvent = new BuildEvent(stateChangeEvent);
                    break;
                case ArcDPSEnums.StateChange.ShardId:
                    metaDataEvents.ShardEvents.Add(new ShardEvent(stateChangeEvent));
                    break;
                case ArcDPSEnums.StateChange.Reward:
#if !NO_REWARDS
                    rewardEvents.Add(new RewardEvent(stateChangeEvent));
#endif
                    break;
                case ArcDPSEnums.StateChange.TeamChange:
                    var tcEvt = new TeamChangeEvent(stateChangeEvent, agentData);
                    Add(statusEvents.TeamChangeEvents, tcEvt.Src, tcEvt);
                    break;
                case ArcDPSEnums.StateChange.AttackTarget:
                    var aTEvt = new AttackTargetEvent(stateChangeEvent, agentData);
                    Add(statusEvents.AttackTargetEvents, aTEvt.Src, aTEvt);
                    break;
                case ArcDPSEnums.StateChange.Targetable:
                    var tarEvt = new TargetableEvent(stateChangeEvent, agentData);
                    Add(statusEvents.TargetableEvents, tarEvt.Src, tarEvt);
                    break;
                case ArcDPSEnums.StateChange.MapID:
                    metaDataEvents.MapIDEvents.Add(new MapIDEvent(stateChangeEvent));
                    break;
                case ArcDPSEnums.StateChange.Guild:
                    var gEvt = new GuildEvent(stateChangeEvent, agentData);
                    Add(metaDataEvents.GuildEvents, gEvt.Src, gEvt);
                    break;
                case ArcDPSEnums.StateChange.BuffInfo:
                case ArcDPSEnums.StateChange.BuffFormula:
                    if (metaDataEvents.BuffInfoEvents.TryGetValue(stateChangeEvent.SkillID, out BuffInfoEvent buffInfoEvent))
                    {
                        buffInfoEvent.CompleteBuffInfoEvent(stateChangeEvent);
                    }
                    else
                    {
                        buffInfoEvent = new BuffInfoEvent(stateChangeEvent);
                        metaDataEvents.BuffInfoEvents[stateChangeEvent.SkillID] = buffInfoEvent;
                    }
                    if (stateChangeEvent.IsStateChange == ArcDPSEnums.StateChange.BuffInfo)
                    {
                        if (metaDataEvents.BuffInfoEventsByCategory.TryGetValue(buffInfoEvent.Category, out List<BuffInfoEvent> bdEvtList))
                        {
                            bdEvtList.Add(buffInfoEvent);
                        }
                        else
                        {
                            metaDataEvents.BuffInfoEventsByCategory[buffInfoEvent.Category] = new List<BuffInfoEvent> { buffInfoEvent };
                        }
                    }
                    break;
                case ArcDPSEnums.StateChange.SkillInfo:
                case ArcDPSEnums.StateChange.SkillTiming:
                    if (metaDataEvents.SkillInfoEvents.TryGetValue(stateChangeEvent.SkillID, out SkillInfoEvent skillInfoEvent))
                    {
                        skillInfoEvent.CompleteSkillInfoEvent(stateChangeEvent);
                    }
                    else
                    {
                        skillInfoEvent = new SkillInfoEvent(stateChangeEvent);
                        metaDataEvents.SkillInfoEvents[stateChangeEvent.SkillID] = skillInfoEvent;
                    }
                    break;
                case ArcDPSEnums.StateChange.BreakbarState:
                    var bSEvt = new BreakbarStateEvent(stateChangeEvent, agentData);
                    Add(statusEvents.BreakbarStateEvents, bSEvt.Src, bSEvt);
                    break;
                case ArcDPSEnums.StateChange.BreakbarPercent:
                    var bPEvt = new BreakbarPercentEvent(stateChangeEvent, agentData);
                    Add(statusEvents.BreakbarPercentEvents, bPEvt.Src, bPEvt);
                    break;
                case ArcDPSEnums.StateChange.Error:
                    metaDataEvents.ErrorEvents.Add(new ErrorEvent(stateChangeEvent));
                    break;
                case ArcDPSEnums.StateChange.Tag:
                    // Getting ready in case this becomes an actual state event
                    _ = new TagEvent(stateChangeEvent, agentData);
                    //Add(metaDataEvents.TagEvents, tagEvt.Src, tagEvt);
                    break;
                case ArcDPSEnums.StateChange.Velocity:
                    var velEvt = new VelocityEvent(stateChangeEvent, agentData);
                    Add(statusEvents.MovementEvents, velEvt.Src, velEvt);
                    break;
                case ArcDPSEnums.StateChange.Rotation:
                    var rotEvt = new RotationEvent(stateChangeEvent, agentData);
                    Add(statusEvents.MovementEvents, rotEvt.Src, rotEvt);
                    break;
                case ArcDPSEnums.StateChange.Position:
                    var posEvt = new PositionEvent(stateChangeEvent, agentData);
                    Add(statusEvents.MovementEvents, posEvt.Src, posEvt);
                    break;
                case ArcDPSEnums.StateChange.WeaponSwap:
                    wepSwaps.Add(new WeaponSwapEvent(stateChangeEvent, agentData, skillData));
                    break;
                case ArcDPSEnums.StateChange.StackActive:
                    buffEvents.Add(new BuffStackActiveEvent(stateChangeEvent, agentData, skillData));
                    break;
                case ArcDPSEnums.StateChange.StackReset:
                    buffEvents.Add(new BuffStackResetEvent(stateChangeEvent, agentData, skillData));
                    break;
                case ArcDPSEnums.StateChange.BuffInitial:
                    buffEvents.Add(new BuffApplyEvent(stateChangeEvent, agentData, skillData));
                    break;
                default:
                    break;
            }
        }

        public static void AddBuffApplyEvent(Combat buffEvent, List<AbstractBuffEvent> buffEvents, AgentData agentData, SkillData skillData)
        {
            if (buffEvent.IsOffcycle > 0)
            {
                buffEvents.Add(new BuffExtensionEvent(buffEvent, agentData, skillData));
            }
            else
            {
                buffEvents.Add(new BuffApplyEvent(buffEvent, agentData, skillData));
            }
        }

        public static void AddBuffRemoveEvent(Combat buffEvent, List<AbstractBuffEvent> buffEvents, AgentData agentData, SkillData skillData)
        {
            switch (buffEvent.IsBuffRemove)
            {
                case ArcDPSEnums.BuffRemove.Single:
                    buffEvents.Add(new BuffRemoveSingleEvent(buffEvent, agentData, skillData));
                    break;
                case ArcDPSEnums.BuffRemove.All:
                    buffEvents.Add(new BuffRemoveAllEvent(buffEvent, agentData, skillData));
                    break;
                case ArcDPSEnums.BuffRemove.Manual:
                    buffEvents.Add(new BuffRemoveManualEvent(buffEvent, agentData, skillData));
                    break;
            }
        }

        public static List<AnimatedCastEvent> CreateCastEvents(Dictionary<ulong, List<Combat>> castEventsBySrcAgent, AgentData agentData, SkillData skillData, FightData fightData)
        {
            var res = new List<AnimatedCastEvent>();
            foreach (KeyValuePair<ulong, List<Combat>> pairBySrcAgent in castEventsBySrcAgent)
            {
                var resBySrcAgent = new List<AnimatedCastEvent>();
                var castEventsBySrcAgentBySkillID = pairBySrcAgent.Value.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
                foreach (KeyValuePair<uint, List<Combat>> pairBySrcAgentBySkillID in castEventsBySrcAgentBySkillID)
                {
                    var resBySrcAgentBySkillID = new List<AnimatedCastEvent>();
                    Combat startItem = null;
                    foreach (Combat c in pairBySrcAgentBySkillID.Value)
                    {
                        if (c.StartCasting())
                        {
                            // missing end
                            if (startItem != null)
                            {
                                resBySrcAgentBySkillID.Add(new AnimatedCastEvent(startItem, agentData, skillData, fightData.LogEnd));
                            }
                            startItem = c;
                        }
                        else if (c.EndCasting())
                        {
                            if (startItem != null && startItem.SkillID == c.SkillID)
                            {
                                resBySrcAgentBySkillID.Add(new AnimatedCastEvent(startItem, agentData, skillData, c));
                                startItem = null;
                            }
                            // missing start
                            else
                            {
                                var toCheck = new AnimatedCastEvent(agentData, skillData, c);
                                // we are only interested in animations started before log starts
                                if (toCheck.Time < fightData.LogStart)
                                {
                                    resBySrcAgentBySkillID.Add(toCheck);
                                }
                            }
                        }
                    }
                    // missing end
                    if (startItem != null)
                    {
                        resBySrcAgentBySkillID.Add(new AnimatedCastEvent(startItem, agentData, skillData, fightData.LogEnd));
                    }
                    resBySrcAgentBySkillID.RemoveAll(x => x.ActualDuration <= 1);
                    resBySrcAgent.AddRange(resBySrcAgentBySkillID);
                }
                resBySrcAgent = resBySrcAgent.OrderBy(x => x.Time).ToList();
                // sanitize 
                for (int i = 0; i < resBySrcAgent.Count - 1; i++)
                {
                    resBySrcAgent[i].CutAt(resBySrcAgent[i + 1].Time + ParserHelper.ServerDelayConstant);
                }
                res.AddRange(resBySrcAgent);
            }
            res = res.OrderBy(x => x.Time).ToList();
            return res;
        }

        public static void AddDirectDamageEvent(Combat damageEvent, List<AbstractHealthDamageEvent> hpDamage, List<AbstractBreakbarDamageEvent> brkBarDamage, AgentData agentData, SkillData skillData)
        {
            ArcDPSEnums.PhysicalResult result = ArcDPSEnums.GetPhysicalResult(damageEvent.Result);
            switch (result)
            {
                case ArcDPSEnums.PhysicalResult.BreakbarDamage:
                    brkBarDamage.Add(new DirectBreakbarDamageEvent(damageEvent, agentData, skillData));
                    break;
                case ArcDPSEnums.PhysicalResult.Activation:
                case ArcDPSEnums.PhysicalResult.Unknown:
                    break;
                default:
                    hpDamage.Add(new DirectHealthDamageEvent(damageEvent, agentData, skillData, result));
                    break;
            }
        }

        public static void AddIndirectDamageEvent(Combat damageEvent, List<AbstractHealthDamageEvent> hpDamage, List<AbstractBreakbarDamageEvent> brkBarDamage, AgentData agentData, SkillData skillData)
        {
            ArcDPSEnums.ConditionResult result = ArcDPSEnums.GetConditionResult(damageEvent.Result);
            switch (result)
            {
                /*case ArcDPSEnums.ConditionResult.BreakbarDamage:
                    brkBarDamage.Add(new NonDirectBreakbarDamageEvent(c, agentData, skillData));
                    break;*/
                case ArcDPSEnums.ConditionResult.Unknown:
                    break;
                default:
                    hpDamage.Add(new NonDirectHealthDamageEvent(damageEvent, agentData, skillData, result));
                    break;
            }
        }
    }
}
