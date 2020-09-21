using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using System;

namespace Gw2LogParser.Parser.Data
{
    public class CircleDecorationSerializable : FormDecorationSerializable
    {
        public int Radius { get; }
        public int MinRadius { get; }

        internal CircleDecorationSerializable(ParsedLog log, CircleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Circle";
            Radius = decoration.Radius;
            MinRadius = decoration.MinRadius;
        }
    }
}
