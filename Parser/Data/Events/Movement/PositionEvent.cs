using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.Statistics;

namespace Gw2LogParser.Parser.Data.Events.Movement
{
    public class PositionEvent : AbstractMovementEvent
    {

        internal PositionEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        internal override void AddPoint3D(CombatReplay replay)
        {
            (float x, float y, float z) = Unpack();
            if (x == 0.0f && y == 0.0f && z == 0.0f)
            {
                return;
            }
            replay.Positions.Add(new Point3D(x, y, z, Time));

        }
    }
}
