using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class RectangleDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public int Height { get; }
        public int Width { get; }

        internal RectangleDecorationCombatReplayDescription(ParsedLog log, RectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Rectangle";
            Width = decoration.Width;
            Height = decoration.Height;
        }
    }
}
