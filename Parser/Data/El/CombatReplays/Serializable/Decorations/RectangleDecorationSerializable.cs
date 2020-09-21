
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data
{
    public class RectangleDecorationSerializable : FormDecorationSerializable
    {
        public int Height { get; }
        public int Width { get; }

        internal RectangleDecorationSerializable(ParsedLog log, RectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Rectangle";
            Width = decoration.Width;
            Height = decoration.Height;
        }
    }
}
