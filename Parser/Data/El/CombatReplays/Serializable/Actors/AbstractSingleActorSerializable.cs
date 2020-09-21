using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.Statistics;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data
{
    public class AbstractSingleActorSerializable
    {
        public string Img { get; }
        public string Type { get; }
        public int ID { get; }
        public List<double> Positions { get; }

        internal AbstractSingleActorSerializable(AbstractSingleActor actor, ParsedLog log, CombatReplayMap map, CombatReplay replay, string type)
        {
            Img = actor.GetIcon();
            ID = actor.CombatReplayID;
            Positions = new List<double>();
            Type = type;
            foreach (Point3D pos in replay.PolledPositions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                Positions.Add(x);
                Positions.Add(y);
            }
        }
    }
}
