﻿using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations
{
    internal class RectangleDecoration : FormDecoration
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleDecoration(bool fill, int growing, int width, int height, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            Height = height;
            Width = width;
        }

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            return new RectangleDecorationSerializable(log, this, map);
        }
    }
}
