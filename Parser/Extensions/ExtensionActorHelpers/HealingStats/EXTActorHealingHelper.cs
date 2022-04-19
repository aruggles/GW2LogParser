using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Helper.CachingCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Extensions.HealingStatsExtensionHandler;

namespace Gw2LogParser.Parser.Extensions
{
    public abstract class EXTActorHealingHelper
    {
        protected List<EXTAbstractHealingEvent> HealEvents { get; set; }
        protected Dictionary<Agent, List<EXTAbstractHealingEvent>> HealEventsByDst { get; set; }
        protected List<EXTAbstractHealingEvent> HealReceivedEvents { get; set; }
        protected Dictionary<Agent, List<EXTAbstractHealingEvent>> HealReceivedEventsBySrc { get; set; }


        private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>> _typedHealEvents = new Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>>();

        internal EXTActorHealingHelper()
        {
        }

        public abstract IReadOnlyList<EXTAbstractHealingEvent> GetOutgoingHealEvents(AbstractSingleActor target, ParsedLog log, long start, long end);

        public abstract IReadOnlyList<EXTAbstractHealingEvent> GetIncomingHealEvents(AbstractSingleActor target, ParsedLog log, long start, long end);

        public IReadOnlyList<EXTAbstractHealingEvent> GetTypedOutgoingHealEvents(AbstractSingleActor target, ParsedLog log, long start, long end, EXTHealingType healingType)
        {
            if (!_typedHealEvents.TryGetValue(healingType, out CachingCollectionWithTarget<List<EXTAbstractHealingEvent>> healEventsPerPhasePerTarget))
            {
                healEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>(log);
                _typedHealEvents[healingType] = healEventsPerPhasePerTarget;
            }
            if (!healEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<EXTAbstractHealingEvent> dls))
            {
                dls = GetOutgoingHealEvents(target, log, start, end).ToList();
                switch (healingType)
                {
                    case EXTHealingType.HealingPower:
                        dls.RemoveAll(x => x.GetHealingType(log) != EXTHealingType.HealingPower);
                        break;
                    case EXTHealingType.ConversionBased:
                        dls.RemoveAll(x => x.GetHealingType(log) != EXTHealingType.ConversionBased);
                        break;
                    case EXTHealingType.Hybrid:
                        dls.RemoveAll(x => x.GetHealingType(log) != EXTHealingType.Hybrid);
                        break;
                    case EXTHealingType.All:
                        break;
                    default:
                        throw new NotImplementedException("Not implemented healing type " + healingType);
                }
                healEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }
    }
}
