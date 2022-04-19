using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class RotatedRectangleDecorationCombatReplayDescription : RectangleDecorationCombatReplayDescription
    {
        public float Rotation { get; }
        public int RadialTranslation { get; }
        public int SpinAngle { get; }

        internal RotatedRectangleDecorationCombatReplayDescription(ParsedLog log, RotatedRectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "RotatedRectangle";
            Rotation = decoration.Rotation;
            RadialTranslation = decoration.RadialTranslation;
            SpinAngle = decoration.SpinAngle;
        }
    }
}
