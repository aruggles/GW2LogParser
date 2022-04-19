using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves
{
    public class BuffRemoveManualEvent : AbstractBuffRemoveEvent
    {
        internal BuffRemoveManualEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }

        internal BuffRemoveManualEvent(Agent by, Agent to, long time, int removedDuration, Skill buffSkill) : base(by, to, time, removedDuration, buffSkill)
        {
        }

        internal override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return false; // don't consider manual remove events
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
        }
    }
}
