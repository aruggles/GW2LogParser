using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using System.Linq;

namespace Gw2LogParser.Parser.Data
{
    public class FacingDecorationSerializable : GenericAttachedDecorationSerializable
    {
        public int[] FacingData { get; }

        internal FacingDecorationSerializable(ParsedLog log, FacingDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Facing";
            FacingData = decoration.Angles.ToArray();
        }

    }
}
