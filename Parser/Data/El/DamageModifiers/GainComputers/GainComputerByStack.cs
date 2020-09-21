
namespace Gw2LogParser.Parser.Data.El.DamageModifiers.GainComputers
{
    internal class GainComputerByStack : GainComputer
    {
        public GainComputerByStack()
        {
            Multiplier = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            return gainPerStack * stack / (100 + stack * gainPerStack);
        }
    }
}
