using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class TheKeyOfAhdashim : RaidLogic
    {
        public TheKeyOfAhdashim(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.TheKeyOfAhdashim;
        }
    }
}
