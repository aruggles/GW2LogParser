using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public abstract class GenericAttachedDecorationCombatReplayDescription : GenericDecorationCombatReplayDescription
    {
        public object ConnectedTo { get; }

        internal GenericAttachedDecorationCombatReplayDescription(ParsedLog log, GenericAttachedDecoration decoration, CombatReplayMap map) : base(decoration)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
        }
    }
}
