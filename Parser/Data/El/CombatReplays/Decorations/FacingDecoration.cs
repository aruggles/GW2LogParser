using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;
using Gw2LogParser.Parser.Data.El.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations
{
    internal class FacingDecoration : GenericAttachedDecoration
    {
        public List<int> Angles { get; } = new List<int>();

        public FacingDecoration((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings) : base(lifespan, connector)
        {
            foreach (Point3D facing in facings)
            {
                Angles.Add(-Point3D.GetRotationFromFacing(facing));
            }
        }

        //

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            return new FacingDecorationSerializable(log, this, map);
        }
    }
}
