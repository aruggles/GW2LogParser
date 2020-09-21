using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.Statistics
{
    public class FinalPlayerSupport
    {
        //public long allHeal;
        public int Resurrects { get; internal set; }
        public double ResurrectTime { get; internal set; }
        public int CondiCleanse { get; internal set; }
        public double CondiCleanseTime { get; internal set; }
        public int CondiCleanseSelf { get; internal set; }
        public double CondiCleanseTimeSelf { get; internal set; }
        public int BoonStrips { get; internal set; }
        public double BoonStripsTime { get; internal set; }
    }
}
