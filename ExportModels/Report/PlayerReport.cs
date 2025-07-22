using GW2EIBuilders.HtmlModels.HTMLStats;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gw2LogParser.ExportModels.Report;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
internal class PlayerReport : IComparer<PlayerReport>
{
    public string? Identifier => Name.Replace(' ', '_').ToLower();
    public string? Name { get; set; }
    public int? Group { get; set; }
    public string? Account { get; set; }
    public string? Profession { get; set; }
    public string? Icon { get; set; }
    public double DPS
    {
        get
        {
            return Math.Round(Damage.AllDamage / TimeSpan.FromMilliseconds(TimeInCombat).TotalSeconds, 0);
        }
    }
    public long TimeInCombat { get; set; }
    public DamageReport Damage { get; set; } = new DamageReport();
    public DefenseReport Defense { get; set; } = new DefenseReport();
    public SupportReport Support { get; set; } = new SupportReport();
    public GameplayReport Gameplay { get; set; } = new GameplayReport();
    public List<SummaryItem> DamageSummary { get; set; } = [];
    public List<SummaryItem> TakenSummary { get; set; } = [];
    public List<BoonReport> BoonStats { get; set; } = [];
    public List<BoonReport> BoonGenSelfStats { get; set; } = [];
    public List<BoonReport> BoonGenGroupStats { get; set; } = [];
    public List<BoonReport> BoonGenOGroupStats { get; set; } = [];
    public List<BoonReport> BoonGenSquadStats { get; set; } = [];
    public HealingReport? healing;
    public int numberOfFights { get; set; } = 1;

    public PlayerReport()
    {

    }

    public int Compare(PlayerReport x, PlayerReport y)
    {
        if (x == null || y == null || (x.Name == null && y.Name == null))
        {
            return 0;
        }
        if (x.Name == null && y.Name != null)
        {
            return -1;
        }
        if (x.Name != null && y.Name != null)
        {
            return 1;
        }
        if (x.Name == null) { return -1; }
        return x.Name.CompareTo(y.Name);
    }

    private T Parse<T>(object value)
    {
        return (T)Convert.ChangeType(value, typeof(T));
    }
}

