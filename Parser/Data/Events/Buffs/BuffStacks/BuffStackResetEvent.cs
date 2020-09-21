using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Buffs.BuffStacks
{
    class BuffStackResetEvent : AbstractBuffStackEvent
    {
        private readonly int _resetToDuration;
        internal BuffStackResetEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BuffInstance = evtcItem.Pad;
            _resetToDuration = evtcItem.Value;
        }

        internal override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return false; // ignore reset event
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Reset(BuffInstance, _resetToDuration);
        }
        internal override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffStackActiveEvent || abe is BuffApplyEvent)
            {
                return 1;
            }
            if (abe is BuffStackResetEvent)
            {
                return 0;
            }
            return -1;
        }
    }
}
