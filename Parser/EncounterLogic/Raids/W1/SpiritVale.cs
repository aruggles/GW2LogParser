using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class SpiritVale : RaidLogic
    {
        public SpiritVale(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.SpiritVale;
        }
    }
}
