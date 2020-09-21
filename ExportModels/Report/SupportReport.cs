using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.ExportModels.Report
{
    public class SupportReport
    {
        public int CleanseOnOther { get; set; }
        public int CleanseOnSelf { get; set; }
        public int BoonStrips { get; set; }
        public int Resurrects { get; set; }

        public SupportReport() {  }
        public SupportReport(SupportReport other)
        {
            CleanseOnOther = other.CleanseOnOther;
            CleanseOnSelf = other.CleanseOnSelf;
            BoonStrips = other.BoonStrips;
            Resurrects = other.Resurrects;
        }
    }
}
