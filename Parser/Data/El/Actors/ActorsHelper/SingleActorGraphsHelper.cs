﻿using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Helper;
using Gw2LogParser.Parser.Helper.CachingCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.Actors.ActorsHelper
{
    class SingleActorGraphsHelper : AbstractSingleActorHelper
    {
        private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<int[]>> _damageList1S = new Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<int[]>>();

        private CachingCollectionWithTarget<double[]> _breakbarDamageList1S;
        private List<Segment> _healthUpdates { get; set; }
        private List<Segment> _breakbarPercentUpdates { get; set; }
        private List<Segment> _barrierUpdates { get; set; }

        public SingleActorGraphsHelper(AbstractSingleActor actor) : base(actor)
        {
        }



        public IReadOnlyList<Segment> GetHealthUpdates(ParsedLog log)
        {
            if (_healthUpdates == null)
            {
                _healthUpdates = Segment.FromStates(log.CombatData.GetHealthUpdateEvents(AgentItem).Select(x => x.ToState()).ToList(), 0, log.FightData.FightEnd);
            }
            return _healthUpdates;
        }

        public IReadOnlyList<Segment> GetBreakbarPercentUpdates(ParsedLog log)
        {
            if (_breakbarPercentUpdates == null)
            {
                _breakbarPercentUpdates = Segment.FromStates(log.CombatData.GetBreakbarPercentEvents(AgentItem).Select(x => x.ToState()).ToList(), 0, log.FightData.FightEnd);
            }
            return _breakbarPercentUpdates;
        }

        public IReadOnlyList<Segment> GetBarrierUpdates(ParsedLog log)
        {
            if (_barrierUpdates == null)
            {
                _barrierUpdates = Segment.FromStates(log.CombatData.GetBarrierUpdateEvents(AgentItem).Select(x => x.ToState()).ToList(), 0, log.FightData.FightEnd);
            }
            return _barrierUpdates;
        }



        public IReadOnlyList<int> Get1SDamageList(ParsedLog log, long start, long end, AbstractSingleActor target, ParserHelper.DamageType damageType = ParserHelper.DamageType.All)
        {
            if (!_damageList1S.TryGetValue(damageType, out CachingCollectionWithTarget<int[]> graphs))
            {
                graphs = new CachingCollectionWithTarget<int[]>(log);
                _damageList1S[damageType] = graphs;
            }
            if (!graphs.TryGetValue(start, end, target, out int[] graph))
            {
                int durationInMS = (int)(end - start);
                int durationInS = durationInMS / 1000;
                graph = durationInS * 1000 != durationInMS ? new int[durationInS + 2] : new int[durationInS + 1];
                // fill the graph
                int previousTime = 0;
                foreach (AbstractHealthDamageEvent dl in Actor.GetHitDamageEvents(target, log, start, end, damageType))
                {
                    int time = (int)Math.Ceiling((dl.Time - start) / 1000.0);
                    if (time != previousTime)
                    {
                        for (int i = previousTime + 1; i <= time; i++)
                        {
                            graph[i] = graph[previousTime];
                        }
                    }
                    previousTime = time;
                    graph[time] += dl.HealthDamage;
                }
                for (int i = previousTime + 1; i < graph.Length; i++)
                {
                    graph[i] = graph[previousTime];
                }
                //
                graphs.Set(start, end, target, graph);
            }
            return graph;
        }

        public IReadOnlyList<double> Get1SBreakbarDamageList(ParsedLog log, long start, long end, AbstractSingleActor target)
        {
            if (!log.CombatData.HasBreakbarDamageData)
            {
                return null;
            }
            if (_breakbarDamageList1S == null)
            {
                _breakbarDamageList1S = new CachingCollectionWithTarget<double[]>(log);
            }
            if (_breakbarDamageList1S.TryGetValue(start, end, target, out double[] res))
            {
                return res;
            }
            int durationInMS = (int)(end - start);
            int durationInS = durationInMS / 1000;
            var brkDmgList = durationInS * 1000 != durationInMS ? new double[durationInS + 2] : new double[durationInS + 1];
            IReadOnlyList<AbstractBreakbarDamageEvent> breakbarDamageEvents = Actor.GetBreakbarDamageEvents(target, log, start, end);
            // fill the graph
            int previousTime = 0;
            foreach (AbstractBreakbarDamageEvent dl in breakbarDamageEvents)
            {
                int time = (int)Math.Ceiling((dl.Time - start) / 1000.0);
                if (time != previousTime)
                {
                    for (int i = previousTime + 1; i <= time; i++)
                    {
                        brkDmgList[i] = brkDmgList[previousTime];
                    }
                }
                previousTime = time;
                brkDmgList[time] += dl.BreakbarDamage;
            }
            for (int i = previousTime + 1; i < brkDmgList.Length; i++)
            {
                brkDmgList[i] = brkDmgList[previousTime];
            }
            _breakbarDamageList1S.Set(start, end, target, brkDmgList);
            return brkDmgList;
        }

        public double GetCurrentHealthPercent(ParsedLog log, long time)
        {
            IReadOnlyList<Segment> hps = GetHealthUpdates(log);
            if (!hps.Any())
            {
                return -1.0;
            }
            foreach (Segment seg in hps)
            {
                if (seg.Intersect(time - ParserHelper.ServerDelayConstant, time + ParserHelper.ServerDelayConstant))
                {
                    return seg.Value;
                }
            }
            return -1.0;
        }

        public double GetCurrentBarrierPercent(ParsedLog log, long time)
        {
            IReadOnlyList<Segment> hps = GetBarrierUpdates(log);
            if (!hps.Any())
            {
                return -1.0;
            }
            foreach (Segment seg in hps)
            {
                if (seg.Intersect(time - ParserHelper.ServerDelayConstant, time + ParserHelper.ServerDelayConstant))
                {
                    return seg.Value;
                }
            }
            return -1.0;
        }
    }
}
