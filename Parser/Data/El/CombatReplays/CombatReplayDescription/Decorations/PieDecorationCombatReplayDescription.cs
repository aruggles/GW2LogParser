using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class PieDecorationCombatReplayDescription : CircleDecorationCombatReplayDescription
    {
        public float Direction { get; set; }
        public float OpeningAngle { get; set; }

        internal PieDecorationCombatReplayDescription(ParsedLog log, PieDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Pie";
            Direction = decoration.Direction;
            OpeningAngle = decoration.OpeningAngle;
        }
    }
}
