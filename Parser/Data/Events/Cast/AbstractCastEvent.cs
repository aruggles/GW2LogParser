using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Cast
{
    public abstract class AbstractCastEvent : AbstractTimeCombatEvent
    {
        public enum AnimationStatus { Unknown, Reduced, Interrupted, Full, Instant };

        // start item
        public Skill Skill { get; protected set; }
        public long SkillId => Skill.ID;
        public Agent Caster { get; }

        public AnimationStatus Status { get; protected set; } = AnimationStatus.Unknown;
        public int SavedDuration { get; protected set; }

        public int ExpectedDuration { get; protected set; }

        public int ActualDuration { get; protected set; }

        public long EndTime => Time + ActualDuration;

        public double Acceleration { get; protected set; } = 0;

        internal AbstractCastEvent(Combat baseItem, AgentData agentData, SkillData skillData) : base(baseItem.Time)
        {
            Skill = skillData.Get(baseItem.SkillID);
            Caster = agentData.GetAgent(baseItem.SrcAgent, baseItem.Time);
        }

        internal AbstractCastEvent(long time, Skill skill, Agent caster) : base(time)
        {
            Skill = skill;
            Caster = caster;
        }

    }
}
