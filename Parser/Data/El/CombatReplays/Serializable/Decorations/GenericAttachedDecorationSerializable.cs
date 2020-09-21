using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;

namespace Gw2LogParser.Parser.Data
{
    public abstract class GenericAttachedDecorationSerializable : GenericDecorationSerializable
    {
        public object ConnectedTo { get; }

        internal GenericAttachedDecorationSerializable(ParsedLog log, GenericAttachedDecoration decoration, CombatReplayMap map) : base(decoration)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
        }
    }
}
