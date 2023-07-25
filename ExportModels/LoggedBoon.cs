
namespace Gw2LogParser.ExportModels
{
    public class LoggedBoon
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Stacking { get; set; }
        public bool Consumable { get; set; }
        public bool Enemy { get; set; }

    }
}
