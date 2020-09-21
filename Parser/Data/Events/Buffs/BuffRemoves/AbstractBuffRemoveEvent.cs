using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves
{
    public abstract class AbstractBuffRemoveEvent : AbstractBuffEvent
    {
        public int RemovedDuration { get; }

        internal AbstractBuffRemoveEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            RemovedDuration = evtcItem.Value;
            InternalBy = agentData.GetAgent(evtcItem.DstAgent);
            To = agentData.GetAgent(evtcItem.SrcAgent);
        }

        internal AbstractBuffRemoveEvent(Agent by, Agent to, long time, int removedDuration, Skill buffSkill) : base(buffSkill, time)
        {
            RemovedDuration = removedDuration;
            InternalBy = by;
            To = to;
        }

        internal override void TryFindSrc(ParsedLog log)
        {
        }
    }
}
