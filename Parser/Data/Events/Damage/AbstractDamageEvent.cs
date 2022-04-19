using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;
using System;

namespace Gw2LogParser.Parser.Data.Events.Damage
{
    public abstract class AbstractDamageEvent : AbstractTimeCombatEvent
    {
        public Agent From { get; }
        public Agent CreditedFrom => From.GetFinalMaster();
        public Agent To { get; }

        public Skill Skill { get; }
        public long SkillId => Skill.ID;
        private readonly ArcDPSEnums.IFF _iff;

        public bool ToFriendly => _iff == ArcDPSEnums.IFF.Friend;
        public bool ToFoe => _iff == ArcDPSEnums.IFF.Foe;
        public bool ToUnknown => _iff == ArcDPSEnums.IFF.Unknown;

        //private int _damage;
        public bool IsOverNinety { get; }
        public bool AgainstUnderFifty { get; }
        public bool IsMoving { get; }
        public bool AgainstMoving { get; }
        public bool IsFlanking { get; }
        public bool AgainstDowned { get; protected set; }

        protected AbstractDamageEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem.Time)
        {
            From = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
            To = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
            Skill = skillData.Get(evtcItem.SkillID);
            IsOverNinety = evtcItem.IsNinety > 0;
            AgainstUnderFifty = evtcItem.IsFifty > 0;
            IsMoving = (evtcItem.IsMoving & 1) > 0;
            AgainstMoving = (evtcItem.IsMoving & 2) > 0;
            IsFlanking = evtcItem.IsFlanking > 0;
            _iff = evtcItem.IFF;
        }

        /*public bool AgainstDowned(ParsedEvtcLog log)
        {
            if (AgainstDownedInternal == -1)
            {
                AgainstDownedInternal = To.IsDowned(log, Time) ? 1 : 0;
            }        
            return AgainstDownedInternal == 1;
        }*/

        public abstract bool ConditionDamageBased(ParsedLog log);
    }
}
