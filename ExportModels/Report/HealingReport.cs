
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gw2LogParser.ExportModels.Report;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class HealingReport
{
    public int OutgoingTargetAll { get; set; }
    public int OutgoingTargetHealingPower { get; set; }
    public int OutgoingTargetConversion { get; set; }
    public int OutgoingTargetDowned { get; set; }
    public int OutgoingAll { get; set; }
    public int OutgoingAllHealingPower { get; set; }
    public int OutgoingAllConversion { get; set; }
    public int OutgoingAllDowned { get; set; }
    public int IncomingHealed { get; set; }
    public int IncomingHealingPower { get; set; }
    public int IncomingConversion { get; set; }
    public int IncomingDowned { get; set; }
    public int IncomingBarrier { get; set; }
    public int OutgoingBarrier { get; set; }
    public int OutgoingTargetBarrier { get; set; }

    public HealingReport() { }
    public HealingReport(HealingReport other)
    {
        OutgoingTargetAll = other.OutgoingTargetAll;
        OutgoingTargetHealingPower = other.OutgoingTargetHealingPower;
        OutgoingTargetConversion = other.OutgoingTargetConversion;
        OutgoingTargetDowned = other.OutgoingTargetDowned;
        OutgoingAll = other.OutgoingAll;
        OutgoingAllHealingPower = other.OutgoingAllHealingPower;
        OutgoingAllConversion = other.OutgoingAllConversion;
        OutgoingAllDowned = other.OutgoingAllDowned;
        IncomingHealed = other.IncomingHealed;
        IncomingHealingPower = other.IncomingHealingPower;
        IncomingConversion = other.IncomingConversion;
        IncomingDowned = other.IncomingDowned;
        IncomingBarrier = other.IncomingBarrier;
        OutgoingBarrier = other.OutgoingBarrier;
        OutgoingTargetBarrier = other.OutgoingTargetBarrier;
    }
}
