using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class FacingRectangleDecorationCombatReplayDescription : FacingDecorationCombatReplayDescription
    {
        public int Width { get; }
        public int Height { get; }
        public string Color { get; }
        public int Translation { get; }

        internal FacingRectangleDecorationCombatReplayDescription(ParsedLog log, FacingRectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "FacingRectangle";
            Width = decoration.Width;
            Height = decoration.Height;
            Translation = decoration.Translation;
            Color = decoration.Color;
        }
    }
}
