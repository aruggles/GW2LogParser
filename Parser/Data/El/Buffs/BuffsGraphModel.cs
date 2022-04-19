﻿using Gw2LogParser.Parser.Data.El.Statistics;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Buffs
{
    public class BuffsGraphModel
    {
        public Buff Buff { get; }

        public IReadOnlyList<Segment> BuffChart => _buffChart;
        private List<Segment> _buffChart { get; set; } = new List<Segment>();

        // Constructor
        internal BuffsGraphModel(Buff buff)
        {
            Buff = buff;
        }
        internal BuffsGraphModel(Buff buff, List<Segment> buffChartWithSource)
        {
            Buff = buff;
            _buffChart = buffChartWithSource;
            FuseSegments();
        }

        public int GetStackCount(long time)
        {
            foreach (Segment seg in BuffChart)
            {
                if (seg.Intersect(time, time))
                {
                    return (int)seg.Value;
                }
            }
            return 0;
        }


        public bool IsPresent(long time)
        {
            return GetStackCount(time) > 0;
        }

        /// <summary>
        /// Fuse consecutive segments with same value
        /// </summary>
        internal void FuseSegments()
        {
            _buffChart = Segment.FuseSegments(_buffChart);
        }

        /// <summary>
        /// This method will integrate the graph "from" to "to"
        /// It is going to add +1 to "to" when "from" has a value > 0
        /// </summary>
        /// <param name="from"></param> 
        /// <param name="to"></param>
        internal void MergePresenceInto(IReadOnlyList<Segment> from)
        {
            List<Segment> segmentsToFill = _buffChart;
            bool firstPass = segmentsToFill.Count == 0;
            foreach (Segment seg in from)
            {
                long start = seg.Start;
                long end = seg.End;
                int presence = seg.Value > 0 ? 1 : 0;
                if (firstPass)
                {
                    segmentsToFill.Add(new Segment(start, end, presence));
                }
                else
                {
                    for (int i = 0; i < segmentsToFill.Count; i++)
                    {
                        Segment curSeg = segmentsToFill[i];
                        long curEnd = curSeg.End;
                        long curStart = curSeg.Start;
                        int curVal = (int)curSeg.Value;
                        if (curStart > end)
                        {
                            break;
                        }
                        if (curEnd < start)
                        {
                            continue;
                        }
                        if (end <= curEnd)
                        {
                            curSeg.End = start;
                            segmentsToFill.Insert(i + 1, new Segment(start, end, curVal + presence));
                            segmentsToFill.Insert(i + 2, new Segment(end, curEnd, curVal));
                            break;
                        }
                        else
                        {
                            curSeg.End = start;
                            segmentsToFill.Insert(i + 1, new Segment(start, curEnd, curVal + presence));
                            start = curEnd;
                            i++;
                        }
                    }
                }
            }
            // Merge consecutive segments with same value, otherwise expect exponential growth
            FuseSegments();
        }
    }
}
