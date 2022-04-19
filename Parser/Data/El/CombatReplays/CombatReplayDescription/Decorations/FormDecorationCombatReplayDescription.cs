using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public abstract class FormDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {
        public bool Fill { get; }
        public int Growing { get; }
        public string Color { get; }

        internal FormDecorationCombatReplayDescription(ParsedLog log, FormDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Fill = decoration.Filled;
            Color = decoration.Color;
            Growing = decoration.Growing;
        }
    }
}
