﻿
using Gw2LogParser.Parser.Data.El.Statistics;

namespace Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors
{
    internal class InterpolatedPositionConnector : PositionConnector
    {
        public InterpolatedPositionConnector(Point3D prev, Point3D next, int time) : base(prev)
        {
            if (prev != null && next != null)
            {
                long denom = next.Time - prev.Time;
                if (denom == 0)
                {
                    Position = prev;
                }
                else
                {
                    float ratio = (float)(time - prev.Time) / denom;
                    Position = new Point3D(prev, next, ratio, time);
                }
            }
            else
            {
                Position = prev ?? next;
            }
        }
    }
}
