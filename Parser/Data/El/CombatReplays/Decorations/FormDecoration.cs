using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations
{
    internal abstract class FormDecoration : GenericAttachedDecoration
    {

        public bool Filled { get; }
        public string Color { get; }
        public int Growing { get; }

        protected FormDecoration(bool fill, int growing, (int start, int end) lifespan, string color, Connector connector) : base(lifespan, connector)
        {
            Color = color;
            Filled = fill;
            Growing = growing;
        }

    }
}
