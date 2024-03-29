﻿using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;
using Gw2LogParser.Parser.Data.El.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations
{
    internal class FacingRectangleDecoration : FacingDecoration
    {
        public int Width { get; }
        public int Height { get; }
        public string Color { get; }
        public int Translation { get; }
        public FacingRectangleDecoration((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings, int width, int height, string color) : base(lifespan, connector, facings)
        {
            Width = width;
            Height = height;
            Color = color;
        }

        public FacingRectangleDecoration((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings, int width, int height, int translation, string color) : this(lifespan, connector, facings, width, height, color)
        {
            Translation = translation;
        }
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedLog log)
        {
            return new FacingRectangleDecorationCombatReplayDescription(log, this, map);
        }
    }
}
