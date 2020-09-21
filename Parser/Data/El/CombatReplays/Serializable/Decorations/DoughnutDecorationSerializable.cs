using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data
{
    public class DoughnutDecorationSerializable : FormDecorationSerializable
    {
        public int InnerRadius { get; }
        public int OuterRadius { get; }

        internal DoughnutDecorationSerializable(ParsedLog log, DoughnutDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Doughnut";
            OuterRadius = decoration.OuterRadius;
            InnerRadius = decoration.InnerRadius;
        }

    }
}
