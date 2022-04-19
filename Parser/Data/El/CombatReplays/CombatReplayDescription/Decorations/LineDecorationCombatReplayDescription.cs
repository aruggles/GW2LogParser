using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class LineDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public object ConnectedFrom { get; }

        internal LineDecorationCombatReplayDescription(ParsedLog log, LineDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Line";
            ConnectedFrom = decoration.ConnectedFrom.GetConnectedTo(map, log);
        }
    }
}
