using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Skills;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Statistics
{
    public class FinalDefensesAll : FinalDefenses
    {
        public int DownCount { get; }
        public long DownDuration { get; }
        public int DeadCount { get; }
        public long DeadDuration { get; }
        public int DcCount { get; }
        public long DcDuration { get; }

        public FinalDefensesAll(ParsedLog log, long start, long end, AbstractSingleActor actor) : base(log, start, end, actor, null)
        {
            (IReadOnlyList<(long start, long end)> dead, IReadOnlyList<(long start, long end)> down, IReadOnlyList<(long start, long end)> dc) = actor.GetStatus(log);

            DownCount = log.MechanicData.GetMechanicLogs(log, Skill.DownId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DeadCount = log.MechanicData.GetMechanicLogs(log, Skill.DeathId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DcCount = log.MechanicData.GetMechanicLogs(log, Skill.DCId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);

            DownDuration = down.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            DeadDuration = dead.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            DcDuration = dc.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
        }
    }
}
