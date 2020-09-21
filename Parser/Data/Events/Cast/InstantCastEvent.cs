using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Cast
{
    public class InstantCastEvent : AbstractCastEvent
    {
        internal InstantCastEvent(long time, Skill skill, Agent caster) : base(time, skill, caster)
        {
            Status = AnimationStatus.Instant;
            ActualDuration = 0;
            ExpectedDuration = 0;
        }

        internal InstantCastEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Status = AnimationStatus.Instant;
            ActualDuration = 0;
            ExpectedDuration = 0;
        }
    }
}
