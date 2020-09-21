using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data
{
    public class PieDecorationSerializable : CircleDecorationSerializable
    {
        public int Direction { get; set; }
        public int OpeningAngle { get; set; }

        internal PieDecorationSerializable(ParsedLog log, PieDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Pie";
            Direction = decoration.Direction;
            OpeningAngle = decoration.OpeningAngle;
        }

    }
}
