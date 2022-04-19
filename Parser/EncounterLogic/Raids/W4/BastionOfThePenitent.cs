using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class BastionOfThePenitent : RaidLogic
    {
        public BastionOfThePenitent(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.BastionOfThePenitent;
        }
    }
}
