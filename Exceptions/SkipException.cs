using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Exceptions
{
    public class SkipException : Exception
    {
        public SkipException() : base("Option enabled: Failed logs are skipped")
        {
        }
    }
}
