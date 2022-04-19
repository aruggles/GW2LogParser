using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.DamageModifiers.GainComputers
{
    internal class GainComputerByMultiPresence : GainComputerByStack
    {
        public GainComputerByMultiPresence()
        {
            Multiplier = true;
        }
    }
}
