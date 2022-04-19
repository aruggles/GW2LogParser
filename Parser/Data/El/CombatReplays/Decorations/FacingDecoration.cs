using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;
using Gw2LogParser.Parser.Data.El.Statistics;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations
{
    internal class FacingDecoration : GenericAttachedDecoration
    {
        public List<float> Angles { get; } = new List<float>();

        public FacingDecoration((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings) : base(lifespan, connector)
        {
            foreach (Point3D facing in facings)
            {
                if (facing.Time >= lifespan.start && facing.Time <= lifespan.end)
                {
                    Angles.Add(-Point3D.GetRotationFromFacing(facing));
                }
            }
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedLog log)
        {
            return new FacingDecorationCombatReplayDescription(log, this, map);
        }
    }
}
