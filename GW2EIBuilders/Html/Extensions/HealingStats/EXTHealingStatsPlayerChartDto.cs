using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class EXTHealingStatsPlayerChartDto
    {
        public PlayerDamageChartDto<int> Healing { get; }
        public PlayerDamageChartDto<int> HealingPowerHealing { get; }
        public PlayerDamageChartDto<int> ConversionBasedHealing { get; }
        //public PlayerDamageChartDto<int> HybridHealing { get; }

        private EXTHealingStatsPlayerChartDto(ParsedLog log, PhaseData phase, AbstractSingleActor p)
        {
            Healing = new PlayerDamageChartDto<int>()
            {
                Total = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.All),
                Targets = new List<IReadOnlyList<int>>()
            };
            //
            var hybridHealingPower = new List<int>(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.HealingPower));
            IReadOnlyList<int> hybrid = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.Hybrid);
            for (int i = 0; i < hybrid.Count; i++)
            {
                hybridHealingPower[i] += hybrid[i];
            }
            HealingPowerHealing = new PlayerDamageChartDto<int>()
            {
                Total = hybridHealingPower,
                Targets = new List<IReadOnlyList<int>>()
            };
            //
            ConversionBasedHealing = new PlayerDamageChartDto<int>()
            {
                Total = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.ConversionBased),
                Targets = new List<IReadOnlyList<int>>()
            };
            //
            /*HybridHealing = new PlayerDamageChartDto<int>()
            {
                Total = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.Hybrid),
                Targets = new List<IReadOnlyList<int>>()
            };*/
            //
            foreach (AbstractSingleActor target in log.Friendlies)
            {
                Healing.Targets.Add(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.All));
                //
                hybridHealingPower = new List<int>(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.HealingPower));
                hybrid = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.Hybrid);
                for (int i = 0; i < hybrid.Count; i++)
                {
                    hybridHealingPower[i] += hybrid[i];
                }
                HealingPowerHealing.Targets.Add(hybridHealingPower);
                //
                ConversionBasedHealing.Targets.Add(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.ConversionBased));
                //
                //HybridHealing.Targets.Add(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, HealingStatsExtensionHandler.EXTHealingType.Hybrid));
            }
        }

        public static List<EXTHealingStatsPlayerChartDto> BuildPlayersHealingGraphData(ParsedLog log, PhaseData phase)
        {
            var list = new List<EXTHealingStatsPlayerChartDto>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new EXTHealingStatsPlayerChartDto(log, phase, actor));
            }
            return list;
        }
    }
}
