using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator.BuffSimulationItems;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Simulator
{
    internal abstract class AbstractBuffSimulator
    {
        internal class BuffStackItem
        {
            public long Start { get; private set; }
            public long Duration { get; private set; }
            public Agent Src { get; private set; }
            public Agent SeedSrc { get; }
            public bool IsExtension { get; private set; }

            public long StackID { get; protected set; } = 0;

            public List<(Agent src, long value)> Extensions { get; } = new List<(Agent src, long value)>();

            public BuffStackItem(long start, long boonDuration, Agent src, Agent seedSrc, long stackID, bool isExtension)
            {
                Start = start;
                SeedSrc = seedSrc;
                Duration = boonDuration;
                Src = src;
                IsExtension = isExtension;
                StackID = stackID;
            }

            public BuffStackItem(long start, long boonDuration, Agent src, long stackID)
            {
                StackID = stackID;
                Start = start;
                SeedSrc = src;
                Duration = boonDuration;
                Src = src;
                IsExtension = false;
            }

            public void Shift(long startShift, long durationShift)
            {
                Start += startShift;
                Duration -= durationShift;
                if (Duration == 0 && Extensions.Count > 0)
                {
                    (Agent src, long value) = Extensions.First();
                    Extensions.RemoveAt(0);
                    Src = src;
                    Duration = value;
                    IsExtension = true;
                }
            }

            public long TotalBoonDuration()
            {
                long res = Duration;
                foreach ((Agent src, long value) in Extensions)
                {
                    res += value;
                }
                return res;
            }

            public void Extend(long value, Agent src)
            {
                Extensions.Add((src, value));
            }
        }
        // Fields
        protected List<BuffStackItem> BuffStack { get; set; }
        public List<BuffSimulationItem> GenerationSimulation { get; } = new List<BuffSimulationItem>();
        public List<BuffSimulationItemOverstack> OverstackSimulationResult { get; } = new List<BuffSimulationItemOverstack>();
        public List<BuffSimulationItemWasted> WasteSimulationResult { get; } = new List<BuffSimulationItemWasted>();

        protected ParsedLog Log { get; }

        // Constructor
        protected AbstractBuffSimulator(ParsedLog log)
        {
            BuffStack = new List<BuffStackItem>();
            Log = log;
        }


        // Abstract Methods
        /// <summary>
        /// Make sure the last element does not overflow the fight
        /// </summary>
        /// <param name="fightDuration">Duration of the fight</param>
        public void Trim(long fightDuration)
        {
            for (int i = GenerationSimulation.Count - 1; i >= 0; i--)
            {
                BuffSimulationItem data = GenerationSimulation[i];
                if (data.End > fightDuration)
                {
                    data.OverrideEnd(fightDuration);
                }
                else
                {
                    break;
                }
            }
            GenerationSimulation.RemoveAll(x => x.Duration <= 0);
        }

        public void Simulate(List<AbstractBuffEvent> logs, long fightDuration)
        {
            long firstTimeValue = logs.Count > 0 ? Math.Min(logs.First().Time, 0) : 0;
            long timeCur = firstTimeValue;
            long timePrev = firstTimeValue;
            foreach (AbstractBuffEvent log in logs)
            {
                timeCur = log.Time;
                if (timeCur - timePrev < 0)
                {
                    throw new InvalidOperationException("Negative passed time in boon simulation");
                }
                Update(timeCur - timePrev);
                log.UpdateSimulator(this);
                timePrev = timeCur;
            }
            Update(fightDuration - timePrev);
            GenerationSimulation.RemoveAll(x => x.Duration <= 0);
            BuffStack.Clear();
        }

        protected abstract void Update(long timePassed);

        public abstract void Add(long duration, Agent src, long time, uint stackID, bool addedActive, uint overstackDuration);

        public abstract void Remove(Agent by, long removedDuration, int removedStacks, long time, ArcDPSEnums.BuffRemove removeType, uint stackID);

        public abstract void Extend(long extension, long oldValue, Agent src, long time, uint stackID);

        public abstract void Activate(uint stackID);
        public abstract void Reset(uint stackID, long toDuration);
    }
}
