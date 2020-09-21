using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Exceptions
{
    public class TooShortException : Exception
    {
        public TooShortException() : base("Fight is too short, aborted")
        {
        }
    }
}
