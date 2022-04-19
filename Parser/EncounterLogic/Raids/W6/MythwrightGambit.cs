using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class MythwrightGambit : RaidLogic
    {
        public MythwrightGambit(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.MythwrightGambit;
        }
    }
}
