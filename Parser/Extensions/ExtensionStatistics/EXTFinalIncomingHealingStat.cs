using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Actors;
using static Gw2LogParser.Parser.Extensions.HealingStatsExtensionHandler;

namespace Gw2LogParser.Parser.Extensions
{
    public class EXTFinalIncomingHealingStat
    {
        public int Healed { get; internal set; }
        public int HealingPowerHealed { get; internal set; }
        public int ConversionHealed { get; internal set; }
        public int HybridHealed { get; internal set; }
        public int DownedHealed { get; internal set; }

        internal EXTFinalIncomingHealingStat(ParsedLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            foreach (EXTAbstractHealingEvent healingEvent in actor.EXTHealing.GetIncomingHealEvents(target, log, start, end))
            {
                Healed += healingEvent.HealingDone;
                switch (healingEvent.GetHealingType(log))
                {
                    case EXTHealingType.ConversionBased:
                        ConversionHealed += healingEvent.HealingDone;
                        break;
                    case EXTHealingType.Hybrid:
                        HybridHealed += healingEvent.HealingDone;
                        break;
                    case EXTHealingType.HealingPower:
                        HealingPowerHealed += healingEvent.HealingDone;
                        break;
                    default:
                        break;
                }
                if (healingEvent.AgainstDowned)
                {
                    DownedHealed += healingEvent.HealingDone;
                }
            }
        }
    }
}
