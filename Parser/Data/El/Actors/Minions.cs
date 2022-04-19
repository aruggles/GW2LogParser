﻿using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Actors
{
    public class Minions : AbstractActor
    {
        private List<NPC> _minionList { get; }

        public IReadOnlyList<NPC> MinionList => _minionList;
        public AbstractSingleActor Master { get; }
        public EXTMinionsHealingHelper EXTHealing { get; }

        internal Minions(AbstractSingleActor master, NPC firstMinion) : base(firstMinion.AgentItem)
        {
            _minionList = new List<NPC> { firstMinion };
            Master = master;
            EXTHealing = new EXTMinionsHealingHelper(this);
            Character = firstMinion.Character;
        }

        internal void AddMinion(NPC minion)
        {
            _minionList.Add(minion);
        }

        public override IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (DamageEvents == null)
            {
                DamageEvents = new List<AbstractHealthDamageEvent>();
                foreach (NPC minion in _minionList)
                {
                    DamageEvents.AddRange(minion.GetDamageEvents(null, log, 0, log.FightData.FightEnd));
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

        public override IReadOnlyList<AbstractBreakbarDamageEvent> GetBreakbarDamageEvents(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (BreakbarDamageEvents == null)
            {
                BreakbarDamageEvents = new List<AbstractBreakbarDamageEvent>();
                foreach (NPC minion in _minionList)
                {
                    BreakbarDamageEvents.AddRange(minion.GetBreakbarDamageEvents(null, log, 0, log.FightData.FightEnd));
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

        /*public List<DamageLog> getHealingLogs(ParsedEvtcLog log, long start, long end)
        {
            List<DamageLog> res = new List<DamageLog>();
            foreach (Minion minion in this)
            {
                res.AddRange(minion.getHealingLogs(log, start, end));
            }
            return res;
        }*/

        public override IReadOnlyList<AbstractHealthDamageEvent> GetDamageTakenEvents(AbstractSingleActor target, ParsedLog log, long start, long end)
        {
            if (DamageTakenEvents == null)
            {
                DamageTakenEvents = new List<AbstractHealthDamageEvent>();
                foreach (NPC minion in _minionList)
                {
                    DamageTakenEvents.AddRange(minion.GetDamageTakenEvents(null, log, 0, log.FightData.FightEnd));
                }
                DamageTakenEvents = DamageTakenEvents.OrderBy(x => x.Time).ToList();
                DamageTakenEventsBySrc = DamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageTakenEventsBySrc.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
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
                foreach (NPC minion in _minionList)
                {
                    BreakbarDamageTakenEvents.AddRange(minion.GetBreakbarDamageTakenEvents(null, log, 0, log.FightData.FightEnd));
                }
                BreakbarDamageTakenEvents = BreakbarDamageTakenEvents.OrderBy(x => x.Time).ToList();
                BreakbarDamageTakenEventsBySrc = BreakbarDamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageTakenEventsBySrc.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractBreakbarDamageEvent>();
                }
            }
            return BreakbarDamageTakenEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        private void InitCastEvents(ParsedLog log)
        {
            CastEvents = new List<AbstractCastEvent>();
            foreach (NPC minion in _minionList)
            {
                CastEvents.AddRange(minion.GetCastEvents(log, 0, log.FightData.FightEnd));
            }
            CastEvents = CastEvents.OrderBy(x => x.Time).ThenBy(x => x.Skill.IsSwap).ToList();
        }

        public override IReadOnlyList<AbstractCastEvent> GetCastEvents(ParsedLog log, long start, long end)
        {
            if (CastEvents == null)
            {
                InitCastEvents(log);
            }
            return CastEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<AbstractCastEvent> GetIntersectingCastEvents(ParsedLog log, long start, long end)
        {
            if (CastEvents == null)
            {
                InitCastEvents(log);
            }
            return CastEvents.Where(x => KeepIntersectingCastLog(x, start, end)).ToList();
        }

        internal IReadOnlyList<IReadOnlyList<Segment>> GetLifeSpanSegments(ParsedLog log)
        {
            var minionsSegments = new List<List<Segment>>();
            long fightDur = log.FightData.FightEnd;
            foreach (NPC minion in _minionList)
            {
                var minionSegments = new List<Segment>();
                long start = Math.Max(minion.FirstAware, 0);
                // Find end
                long end = minion.LastAware;
                DeadEvent dead = log.CombatData.GetDeadEvents(minion.AgentItem).LastOrDefault();
                if (dead != null)
                {
                    end = Math.Min(dead.Time, end);
                }
                DespawnEvent despawn = log.CombatData.GetDespawnEvents(minion.AgentItem).LastOrDefault();
                if (despawn != null)
                {
                    end = Math.Min(despawn.Time, end);
                }
                //
                end = Math.Min(end, fightDur);
                minionSegments.Add(new Segment(0, start, 0));
                minionSegments.Add(new Segment(start, end, 1));
                minionSegments.Add(new Segment(end, fightDur, 0));
                minionsSegments.Add(minionSegments);
            }
            return minionsSegments;
        }
    }
}
