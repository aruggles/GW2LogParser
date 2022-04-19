using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class SalvationPass : RaidLogic
    {
        public SalvationPass(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.SalvationPass;
        }
    }
}
