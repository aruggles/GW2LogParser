using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using Gw2LogParser.EvtcParserExtensions;

namespace Gw2LogParser.GW2EIBuilders
{
    internal abstract class AbstractSkillDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int HealingMode { get; set; }

        protected AbstractSkillDto(Buff buff, ParsedLog log)
        {
            Id = buff.ID;
            Name = buff.Name;
            Icon = buff.Link;
            HealingMode = 3;
            if (log.CombatData.HasEXTHealing)
            {
                switch (log.CombatData.EXTHealingCombatData.GetHealingType(buff, log))
                {
                    case HealingStatsExtensionHandler.EXTHealingType.HealingPower:
                        HealingMode = 0;
                        break;
                    case HealingStatsExtensionHandler.EXTHealingType.ConversionBased:
                        HealingMode = 1;
                        break;
                    case HealingStatsExtensionHandler.EXTHealingType.Hybrid:
                        HealingMode = 2;
                        break;
                    default:
                        HealingMode = 3;
                        break;
                }
            }
        }

        protected AbstractSkillDto(SkillItem skill, ParsedLog log)
        {
            Id = skill.ID;
            Name = skill.Name;
            Icon = skill.Icon;
            HealingMode = 3;
            if (log.CombatData.HasEXTHealing)
            {
                switch (log.CombatData.EXTHealingCombatData.GetHealingType(skill, log))
                {
                    case HealingStatsExtensionHandler.EXTHealingType.HealingPower:
                        HealingMode = 0;
                        break;
                    case HealingStatsExtensionHandler.EXTHealingType.ConversionBased:
                        HealingMode = 1;
                        break;
                    case HealingStatsExtensionHandler.EXTHealingType.Hybrid:
                        HealingMode = 2;
                        break;
                    default:
                        HealingMode = 3;
                        break;
                }
            }
        }
    }
}
