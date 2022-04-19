﻿using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Status;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El
{
    public class PhaseData
    {
        public long Start { get; private set; }
        public long End { get; private set; }
        public long DurationInS { get; private set; }
        public long DurationInMS { get; private set; }
        public long DurationInM { get; private set; }
        public string Name { get; internal set; }
        public bool DrawStart { get; internal set; } = true;
        public bool DrawEnd { get; internal set; } = true;
        public bool DrawArea { get; internal set; } = true;
        public bool DrawLabel { get; internal set; } = true;
        public bool CanBeSubPhase { get; internal set; } = true;

        public bool BreakbarPhase { get; internal set; } = false;

        public bool Dummy { get; internal set; } = false;
        public IReadOnlyList<AbstractSingleActor> Targets => _targets;
        private readonly List<AbstractSingleActor> _targets = new List<AbstractSingleActor>();

        internal PhaseData(long start, long end)
        {
            Start = start;
            End = end;
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }

        internal PhaseData(long start, long end, string name) : this(start, end)
        {
            Name = name;
        }

        public bool InInterval(long time)
        {
            return Start <= time && time <= End;
        }

        internal void AddTarget(AbstractSingleActor target)
        {
            _targets.Add(target);
        }

        internal void RemoveTarget(AbstractSingleActor target)
        {
            _targets.Remove(target);
        }

        internal void AddTargets(IEnumerable<AbstractSingleActor> targets)
        {
            _targets.AddRange(targets);
        }

        internal void OverrideStart(long start)
        {
            Start = start;
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }

        internal void OverrideEnd(long end)
        {
            End = end;
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }

        /// <summary>
        /// Override times in a manner that the phase englobes the targets present in the phase (if possible)
        /// </summary>
        /// <param name="log"></param>
        internal void OverrideTimes(ParsedLog log)
        {
            if (Targets.Count > 0)
            {
                long end = long.MinValue;
                long start = long.MaxValue;
                foreach (AbstractSingleActor target in Targets)
                {
                    long startTime = target.FirstAware;
                    SpawnEvent spawned = log.CombatData.GetSpawnEvents(target.AgentItem).FirstOrDefault();
                    if (spawned != null)
                    {
                        startTime = spawned.Time;
                    }
                    EnterCombatEvent enterCombat = log.CombatData.GetEnterCombatEvents(target.AgentItem).FirstOrDefault();
                    if (enterCombat != null)
                    {
                        startTime = enterCombat.Time;
                    }
                    long deadTime = target.LastAware;
                    DeadEvent died = log.CombatData.GetDeadEvents(target.AgentItem).FirstOrDefault();
                    if (died != null)
                    {
                        deadTime = died.Time;
                    }
                    start = Math.Min(start, startTime);
                    end = Math.Max(end, deadTime);
                }
                Start = Math.Max(Math.Max(Start, start), 0);
                End = Math.Min(Math.Min(End, end), log.FightData.FightEnd);
            }
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }
    }
}
