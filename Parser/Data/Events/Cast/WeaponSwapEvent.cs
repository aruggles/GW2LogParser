using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Cast
{
    public class WeaponSwapEvent : AbstractCastEvent
    {
        // Swaps
        public int SwappedTo { get; protected set; }

        internal WeaponSwapEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Status = AnimationStatus.Instant;
            SwappedTo = (int)evtcItem.DstAgent;
            Skill = skillData.Get(Skill.WeaponSwapId);
            ActualDuration = 0;
            ExpectedDuration = 0;
        }
    }
}
