using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Skills;
using static Gw2LogParser.Parser.Extensions.HealingStatsExtensionHandler;

namespace Gw2LogParser.Parser.Extensions
{
    public abstract class EXTAbstractHealingEvent : AbstractDamageEvent
    {
        public int HealingDone { get; protected set; }

        public bool SrcIsPeer { get; }
        public bool DstIsPeer { get; }

        protected const byte SrcPeerMask = 64;
        protected const byte DstPeerMask = 128;

        internal EXTAbstractHealingEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            SrcIsPeer = (evtcItem.IsOffcycle & SrcPeerMask) > 0;
            DstIsPeer = (evtcItem.IsOffcycle & DstPeerMask) > 0;
            if (!SrcIsPeer && !DstIsPeer)
            {
                SrcIsPeer = true;
            }
        }

        public EXTHealingType GetHealingType(ParsedLog log)
        {
            return log.CombatData.EXTHealingCombatData.GetHealingType(Skill, log);
        }

        public override bool ConditionDamageBased(ParsedLog log)
        {
            return false;
        }
    }
}
