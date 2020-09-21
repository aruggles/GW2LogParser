using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data
{
    public class RotatedRectangleDecorationSerializable : RectangleDecorationSerializable
    {
        public int Rotation { get; }
        public int RadialTranslation { get; }
        public int SpinAngle { get; }

        internal RotatedRectangleDecorationSerializable(ParsedLog log, RotatedRectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "RotatedRectangle";
            Rotation = decoration.Rotation;
            RadialTranslation = decoration.RadialTranslation;
            SpinAngle = decoration.SpinAngle;
        }

    }
}
