﻿
namespace Gw2LogParser.Parser.Data.El.DamageModifiers.GainComputers
{
    internal class GainComputerBySkill : GainComputer
    {
        public GainComputerBySkill()
        {
            Multiplier = true;
            SkillBased = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            return stack > 0 ? 1.0 : 0.0;
        }
    }
}
