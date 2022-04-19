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

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Reset(BuffInstance, _resetToDuration);
        }
    }
}
