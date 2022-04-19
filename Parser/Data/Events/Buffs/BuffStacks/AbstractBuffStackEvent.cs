using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.Events.Buffs.BuffStacks
{
    public abstract class AbstractBuffStackEvent : AbstractBuffEvent
    {
        protected uint BuffInstance { get; set; }

        internal AbstractBuffStackEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            To = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        }

        internal override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != Buff.NoBuff && hasStackIDs && BuffInstance != 0 && Time <= fightEnd - ParserHelper.BuffSimulatorDelayConstant;
        }

        internal override void TryFindSrc(ParsedLog log)
        {
        }
    }
}
