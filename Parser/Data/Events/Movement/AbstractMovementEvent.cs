using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using System;

namespace Gw2LogParser.Parser.Data.Events.Movement
{
    public abstract class AbstractMovementEvent : AbstractTimeCombatEvent
    {
        public Agent Agent { get; }
        private readonly ulong _dstAgent;
        private readonly int _value;

        protected AbstractMovementEvent(Combat evtcItem, AgentData agentData) : base(evtcItem.Time)
        {
            Agent = agentData.GetAgent(evtcItem.SrcAgent);
            _dstAgent = evtcItem.DstAgent;
            _value = evtcItem.Value;
        }

        internal static (float x, float y, float z) UnpackMovementData(ulong packedXY, int intZ)
        {
            byte[] xyBytes = BitConverter.GetBytes(packedXY);
            byte[] zBytes = BitConverter.GetBytes(intZ);
            float x = BitConverter.ToSingle(xyBytes, 0);
            float y = BitConverter.ToSingle(xyBytes, 4);
            float z = BitConverter.ToSingle(zBytes, 0);

            return (x, y, z);
        }

        protected (float x, float y, float z) Unpack()
        {
            return UnpackMovementData(_dstAgent, _value);
        }

        internal abstract void AddPoint3D(CombatReplay replay);
    }
}
