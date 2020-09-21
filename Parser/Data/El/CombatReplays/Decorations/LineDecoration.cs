using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations
{
    internal class LineDecoration : FormDecoration
    {
        public Connector ConnectedFrom { get; }
        public int Width { get; }

        public LineDecoration(int growing, (int start, int end) lifespan, string color, Connector connector, Connector targetConnector) : base(false, growing, lifespan, color, connector)
        {
            ConnectedFrom = targetConnector;
        }

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            return new LineDecorationSerializable(log, this, map);
        }
    }
}
