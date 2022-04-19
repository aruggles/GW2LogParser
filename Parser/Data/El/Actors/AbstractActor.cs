﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Helper;
using Gw2LogParser.Parser.Helper.CachingCollections;

namespace Gw2LogParser.Parser.Data.El.Actors
{
    public abstract class AbstractActor
    {
        public Agent AgentItem { get; }
        public string Character { get; protected set; }

        public int UniqueID => AgentItem.UniqueID;
        public uint Toughness => AgentItem.Toughness;
        public uint Condition => AgentItem.Condition;
        public uint Concentration => AgentItem.Concentration;
        public uint Healing => AgentItem.Healing;
        public ParserHelper.Spec Spec => AgentItem.Spec;
        public ParserHelper.Spec BaseSpec => AgentItem.BaseSpec;
        public long LastAware => AgentItem.LastAware;
        public long FirstAware => AgentItem.FirstAware;
        public int ID => AgentItem.ID;
        public uint HitboxHeight => AgentItem.HitboxHeight;
        public uint HitboxWidth => AgentItem.HitboxWidth;
        public bool IsFakeActor => AgentItem.IsFake;
        // Damage
        protected List<AbstractHealthDamageEvent> DamageEvents { get; set; }
        protected Dictionary<Agent, List<AbstractHealthDamageEvent>> DamageEventByDst { get; set; }
        private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>> _typedHitDamageEvents = new Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>>();
        protected List<AbstractHealthDamageEvent> DamageTakenEvents { get; set; }
        protected Dictionary<Agent, List<AbstractHealthDamageEvent>> DamageTakenEventsBySrc { get; set; }
        // Breakbar Damage
        protected List<AbstractBreakbarDamageEvent> BreakbarDamageEvents { get; set; }
        protected Dictionary<Agent, List<AbstractBreakbarDamageEvent>> BreakbarDamageEventsByDst { get; set; }
        protected List<AbstractBreakbarDamageEvent> BreakbarDamageTakenEvents { get; set; }
        protected Dictionary<Agent, List<AbstractBreakbarDamageEvent>> BreakbarDamageTakenEventsBySrc { get; set; }
        // Cast
        protected List<AbstractCastEvent> CastEvents { get; set; }

        protected AbstractActor(Agent agent)
        {
            string[] name = agent.Name.Split('\0');
            Character = name[0];
            AgentItem = agent;
        }
        // Getters
        // Damage logs
        public abstract IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(AbstractSingleActor target, ParsedLog log, long start, long end);

        public abstract IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageEvents(AbstractSingleActor target, ParsedLog log, long start, long end);

        public IReadOnlyList<AbstractHealthDamageEvent> GetHitDamageEvents(AbstractSingleActor target, ParsedLog log, long start, long end, ParserHelper.DamageType damageType)
        {
            if (!_typedHitDamageEvents.TryGetValue(damageType, out CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> hitDamageEventsPerPhasePerTarget))
            {
                hitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
                _typedHitDamageEvents[damageType] = hitDamageEventsPerPhasePerTarget;
            }
            if (!hitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetDamageEvents(target, log, start, end).Where(x => x.HasHit).ToList();
                switch (damageType)
                {
                    case ParserHelper.DamageType.Power:
                        dls.RemoveAll(x => x.ConditionDamageBased(log));
                        break;
                    case ParserHelper.DamageType.Strike:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent);
                        break;
                    case ParserHelper.DamageType.Condition:
                        dls.RemoveAll(x => !x.ConditionDamageBased(log));
                        break;
                    case ParserHelper.DamageType.StrikeAndCondition:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent && !x.ConditionDamageBased(log));
                        break;
                    case ParserHelper.DamageType.StrikeAndConditionAndLifeLeech:
                        dls.RemoveAll(x => x is NonDirectHealthDamageEvent ndhd && !x.ConditionDamageBased(log) && !ndhd.IsLifeLeech);
                        break;
                    case ParserHelper.DamageType.All:
                        break;
                    default:
                        throw new NotImplementedException("Not implemented damage type " + damageType);
                }
                hitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        public abstract IReadOnlyList<AbstractHealthDamageEvent> GetDamageTakenEvents(AbstractSingleActor target, ParsedLog log, long start, long end);

        public abstract IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenEvents(AbstractSingleActor target, ParsedLog log, long start, long end);

        // Cast logs
        public abstract IReadOnlyList<AbstractCastEvent> GetCastEvents(ParsedLog log, long start, long end);
        public abstract IReadOnlyList<AbstractCastEvent> GetIntersectingCastEvents(ParsedLog log, long start, long end);
        // privates

        protected static bool KeepIntersectingCastLog(AbstractCastEvent evt, long start, long end)
        {
            return (evt.Time >= start && evt.Time <= end) || // start inside
                (evt.EndTime >= start && evt.EndTime <= end) || // end inside
                (evt.Time <= start && evt.EndTime >= end); // start before, end after
        }
    }
}
