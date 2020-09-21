using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations
{
    public abstract class GenericDecoration
    {
        public (int start, int end) Lifespan { get; }

        protected GenericDecoration((int start, int end) lifespan)
        {
            Lifespan = lifespan;
        }
        //

        public abstract GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);

    }
}
