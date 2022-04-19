using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class StrongholdOfTheFaithful : RaidLogic
    {
        public StrongholdOfTheFaithful(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.StrongholdOfTheFaithful;
        }
    }
}
