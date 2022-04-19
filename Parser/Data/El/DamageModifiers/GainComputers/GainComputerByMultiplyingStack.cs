﻿using System;

namespace Gw2LogParser.Parser.Data.El.DamageModifiers.GainComputers
{
    internal class GainComputerByMultiplyingStack : GainComputer
    {
        public GainComputerByMultiplyingStack()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            var pow = 100.0 * Math.Pow(1.0 + gainPerStack / 100.0, stack) - 100.0;
            return pow / (100 + pow);
        }
    }
}
