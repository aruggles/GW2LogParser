﻿using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data
{
    public class LineDecorationSerializable : FormDecorationSerializable
    {
        public object ConnectedFrom { get; }

        internal LineDecorationSerializable(ParsedLog log, LineDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Line";
            ConnectedFrom = decoration.ConnectedFrom.GetConnectedTo(map, log);
        }
    }
}
