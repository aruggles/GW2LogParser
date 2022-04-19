using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
