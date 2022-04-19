using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class CircleDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public int Radius { get; }
        public int MinRadius { get; }

        internal CircleDecorationCombatReplayDescription(ParsedLog log, CircleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Circle";
            Radius = decoration.Radius;
            MinRadius = decoration.MinRadius;
        }
    }
}
