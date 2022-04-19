using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Damage
{
    public abstract class AbstractBreakbarDamageEvent : AbstractDamageEvent
    {
        public double BreakbarDamage { get; protected set; }
        internal AbstractBreakbarDamageEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }
    }
}
