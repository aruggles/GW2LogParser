
namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations
{
    public abstract class BackgroundDecoration : GenericDecoration
    {
        public BackgroundDecoration((int start, int end) lifespan) : base(lifespan)
        {
        }

        public abstract override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedLog log);
    }
}
