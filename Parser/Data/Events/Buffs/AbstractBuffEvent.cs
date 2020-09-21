using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Simulator;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Data.Events.Buffs
{
    public abstract class AbstractBuffEvent : AbstractTimeCombatEvent
    {
        public Skill BuffSkill { get; private set; }
        public long BuffID => BuffSkill.ID;
        //private long _originalBuffID;

        protected Agent InternalBy { get; set; }
        public Agent By => InternalBy?.GetFinalMaster();

        public Agent ByMinion => InternalBy != null && InternalBy.Master != null ? InternalBy : null;

        public Agent To { get; protected set; }

        internal AbstractBuffEvent(Combat evtcItem, SkillData skillData) : base(evtcItem.Time)
        {
            BuffSkill = skillData.Get(evtcItem.SkillID);
        }

        internal AbstractBuffEvent(Skill buffSkill, long time) : base(time)
        {
            BuffSkill = buffSkill;
        }

        internal void Invalidate(SkillData skillData)
        {
            if (BuffID != Buff.NoBuff)
            {
                //_originalBuffID = BuffID;
                BuffSkill = skillData.Get(Buff.NoBuff);
            }
        }

        internal abstract void UpdateSimulator(AbstractBuffSimulator simulator);

        internal abstract void TryFindSrc(ParsedLog log);

        internal abstract bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs);

        internal abstract int CompareTo(AbstractBuffEvent abe);
    }
}
