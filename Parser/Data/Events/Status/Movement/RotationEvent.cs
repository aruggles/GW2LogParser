using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.Statistics;

namespace Gw2LogParser.Parser.Data.Events.Movement
{
    public class RotationEvent : AbstractMovementEvent
    {

        internal RotationEvent(Combat evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        internal override void AddPoint3D(CombatReplay replay)
        {
            (float x, float y, float z) = Unpack();
            replay.Rotations.Add(new Point3D(x, y, z, Time));
        }
    }
}
