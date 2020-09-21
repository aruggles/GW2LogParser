using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.DamageModifiers.GainComputers
{
    internal class GainComputerByPresence : GainComputer
    {
        public GainComputerByPresence()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            return stack > 0 ? gainPerStack / (100 + gainPerStack) : 0;
        }
    }
}
