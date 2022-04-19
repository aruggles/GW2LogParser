using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using System.Collections.Generic;


namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class FacingDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {
        public IReadOnlyList<float> FacingData { get; }

        internal FacingDecorationCombatReplayDescription(ParsedLog log, FacingDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Facing";
            FacingData = decoration.Angles;
        }
    }
}
