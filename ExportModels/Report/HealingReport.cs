
namespace Gw2LogParser.ExportModels.Report
{
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
    }
}
