using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using System;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class DoughnutDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public int InnerRadius { get; }
        public int OuterRadius { get; }

        internal DoughnutDecorationCombatReplayDescription(ParsedLog log, DoughnutDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Doughnut";
            OuterRadius = decoration.OuterRadius;
            InnerRadius = decoration.InnerRadius;
        }
    }
}
