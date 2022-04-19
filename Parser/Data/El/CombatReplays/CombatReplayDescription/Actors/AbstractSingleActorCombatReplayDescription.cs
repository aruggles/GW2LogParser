using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Statistics;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public abstract class AbstractSingleActorCombatReplayDescription
    {
        public string Img { get; }
        public string Type { get; }
        public int ID { get; }
        public IReadOnlyList<float> Positions { get; }
        public IReadOnlyList<long> Dead { get; private set; }
        public IReadOnlyList<long> Down { get; private set; }
        public IReadOnlyList<long> Dc { get; private set; }
        public long Start { get; }
        public long End { get; }

        internal AbstractSingleActorCombatReplayDescription(AbstractSingleActor actor, ParsedLog log, CombatReplayMap map, CombatReplay replay, string type)
        {
            Start = replay.TimeOffsets.start;
            End = replay.TimeOffsets.end;
            Img = actor.GetIcon();
            ID = actor.UniqueID;
            var positions = new List<float>();
            Positions = positions;
            Type = type;
            foreach (Point3D pos in replay.PolledPositions)
            {
                (float x, float y) = map.GetMapCoord(pos.X, pos.Y);
                positions.Add(x);
                positions.Add(y);
            }
        }
        protected void SetStatus(ParsedLog log, AbstractSingleActor a)
        {
            var dead = new List<long>();
            Dead = dead;
            var down = new List<long>();
            Down = down;
            var dc = new List<long>();
            Dc = dc;
            (IReadOnlyList<(long start, long end)> deads, IReadOnlyList<(long start, long end)> downs, IReadOnlyList<(long start, long end)> dcs) = a.GetStatus(log);

            foreach ((long start, long end) in deads)
            {
                dead.Add(start);
                dead.Add(end);
            }
            foreach ((long start, long end) in downs)
            {
                down.Add(start);
                down.Add(end);
            }
            foreach ((long start, long end) in dcs)
            {
                dc.Add(start);
                dc.Add(end);
            }
        }
    }
}
