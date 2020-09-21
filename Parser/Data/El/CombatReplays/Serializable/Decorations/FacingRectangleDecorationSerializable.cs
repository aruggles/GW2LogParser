﻿using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data
{
    public class FacingRectangleDecorationSerializable : FacingDecorationSerializable
    {
        public int Width { get; }
        public int Height { get; }
        public string Color { get; }

        internal FacingRectangleDecorationSerializable(ParsedLog log, FacingRectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "FacingRectangle";
            Width = decoration.Width;
            Height = decoration.Height;
            Color = decoration.Color;
        }
    }
}
