using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data
{
    public abstract class FormDecorationSerializable : GenericAttachedDecorationSerializable
    {
        public bool Fill { get; }
        public int Growing { get; }
        public string Color { get; }

        internal FormDecorationSerializable(ParsedLog log, FormDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Fill = decoration.Filled;
            Color = decoration.Color;
            Growing = decoration.Growing;
        }

    }
}
