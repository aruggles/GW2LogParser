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
        private bool _sourceFinderRan = false;

        internal BuffExtensionEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            _oldValue = evtcItem.OverstackValue - evtcItem.Value;
            _durationChange = evtcItem.Value;
        }

        internal override void TryFindSrc(ParsedLog log)
        {
            if (!_sourceFinderRan && By == ParserHelper._unknownAgent)
            {
                _sourceFinderRan = true;
                By = log.Buffs.TryFindSrc(To, Time, _durationChange, log, BuffID);
            }
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Extend(_durationChange, _oldValue, CreditedBy, Time, BuffInstance);
        }
    }
}
