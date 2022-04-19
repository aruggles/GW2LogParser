using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies
{
    public class BuffApplyEvent : AbstractBuffApplyEvent
    {
        public bool Initial { get; }
        public int AppliedDuration { get; }

        private readonly uint _overstackDuration;
        private readonly bool _addedActive;

        internal BuffApplyEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Initial = evtcItem.IsStateChange == ArcDPSEnums.StateChange.BuffInitial;
            AppliedDuration = evtcItem.Value;
            _addedActive = evtcItem.IsShields > 0;
            _overstackDuration = evtcItem.OverstackValue;
        }

        internal BuffApplyEvent(Agent by, Agent to, long time, int duration, Skill buffSkill, uint id, bool addedActive) : base(by, to, time, buffSkill, id)
        {
            AppliedDuration = duration;
            _addedActive = addedActive;
        }

        internal override void TryFindSrc(ParsedLog log)
        {
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Add(AppliedDuration, CreditedBy, Time, BuffInstance, _addedActive || simulator.Buff.StackType == ArcDPSEnums.BuffStackType.StackingConditionalLoss, _overstackDuration);
        }
    }
}
