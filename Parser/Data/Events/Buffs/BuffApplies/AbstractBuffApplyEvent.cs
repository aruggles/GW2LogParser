using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies
{
    public abstract class AbstractBuffApplyEvent : AbstractBuffEvent
    {
        public uint BuffInstance { get; }

        internal AbstractBuffApplyEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            By = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
            To = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
            BuffInstance = evtcItem.Pad;
        }

        internal AbstractBuffApplyEvent(Agent by, Agent to, long time, Skill buffSkill, uint id) : base(buffSkill, time)
        {
            By = by;
            To = to;
            BuffInstance = id;
        }

        internal override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != Buff.NoBuff && Time <= fightEnd - ParserHelper.BuffSimulatorDelayConstant;
        }
    }
}
