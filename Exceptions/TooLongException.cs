using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Exceptions
{
    public class TooLongException : Exception
    {
        internal TooLongException() : base("Fight is took longer than 24h - may be a broken evtc")
        {
        }
    }
}
