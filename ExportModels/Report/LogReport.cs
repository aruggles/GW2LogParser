using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.ExportModels.Report
{
    public class LogReport
    {
        public string Name { get; set; }
        public Dictionary<string, PlayerReport> players { get; set; } = new Dictionary<string, PlayerReport>();
        public string LogsStart { get; set; }
        public string LogsEnd { get; set; }
        public double lengthInSeconds { get; set; }
        public string PointOfView { get; set; }
    }
}
