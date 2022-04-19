using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Buffs.BuffStacks
{
    public class BuffStackActiveEvent : AbstractBuffStackEvent
    {
        internal BuffStackActiveEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BuffInstance = (uint)evtcItem.DstAgent;
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Activate(BuffInstance);
        }
    }
}
