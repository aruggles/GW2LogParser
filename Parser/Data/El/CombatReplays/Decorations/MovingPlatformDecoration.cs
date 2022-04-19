using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations
{
    internal class MovingPlatformDecoration : BackgroundDecoration
    {
        public string Image { get; }
        public int Width { get; }
        public int Height { get; }

        public List<(float x, float y, float z, float angle, float opacity, int time)> Positions { get; } =
            new List<(float x, float y, float z, float angle, float opacity, int time)>();

        public MovingPlatformDecoration(string image, int width, int height, (int start, int end) lifespan) : base(lifespan)
        {
            Image = image;
            Width = width;
            Height = height;
        }

        public void AddPosition(float x, float y, float z, double angle, double opacity, int time)
        {
            Positions.Add((x, y, z, (float)angle, (float)opacity, time));
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedLog log)
        {
            return new MovingPlatformDecorationCombatReplayDescription(this, map);
        }
    }
}
