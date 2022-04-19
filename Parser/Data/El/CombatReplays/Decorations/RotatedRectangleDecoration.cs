﻿using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations
{
    internal class RotatedRectangleDecoration : RectangleDecoration
    {
        public float Rotation { get; } // initial rotation angle
        public int RadialTranslation { get; } // translation of the triangle center in the direction of the current rotation
        public int SpinAngle { get; } // rotation the rectangle is supposed to go through over the course of its lifespan, 0 for no rotation

        // Rectangles with fixed rotation and no translation
        public RotatedRectangleDecoration(bool fill, int growing, int width, int height, float rotation, (int start, int end) lifespan, string color, Connector connector)
            : this(fill, growing, width, height, rotation, 0, 0, lifespan, color, connector) { }


        // Rectangles with a fixed rotation and translation

        public RotatedRectangleDecoration(bool fill, int growing, int width, int height, float rotation, int translation, (int start, int end) lifespan, string color, Connector connector)
            : this(fill, growing, width, height, rotation, translation, 0, lifespan, color, connector) { }

        // Rectangles rotating over time

        public RotatedRectangleDecoration(bool fill, int growing, int width, int height, float rotation, int translation, int spinAngle, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, width, height, lifespan, color, connector)
        {
            Rotation = rotation;
            RadialTranslation = translation;
            SpinAngle = spinAngle;
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedLog log)
        {
            return new RotatedRectangleDecorationCombatReplayDescription(log, this, map);
        }
    }
}
