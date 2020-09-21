using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Simulator;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies
{
    public class BuffExtensionEvent : AbstractBuffApplyEvent
    {
        private readonly long _oldValue;
        private readonly long _durationChange;

        internal BuffExtensionEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            if (InternalBy == ParserHelper._unknownAgent)
            {
                InternalBy = null;
            }
            _oldValue = evtcItem.OverstackValue - evtcItem.Value;
            _durationChange = evtcItem.Value;
        }

        internal override void TryFindSrc(ParsedLog log)
        {
            if (InternalBy == null)
            {
                InternalBy = log.Buffs.TryFindSrc(To, Time, _durationChange, log, BuffID);
            }
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Extend(_durationChange, _oldValue, By, Time, BuffInstance);
        }

        internal override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffExtensionEvent)
            {
                return 0;
            }
            if (abe is BuffApplyEvent)
            {
                return 1;
            }
            return -1;
        }
    }
}
