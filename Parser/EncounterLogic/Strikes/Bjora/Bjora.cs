using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal abstract class Bjora : StrikeMissionLogic
    {
        public Bjora(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.Bjora;
        }
    }
}
