using GW2EIBuilders.HtmlModels.HTMLStats;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gw2LogParser.ExportModels.Report;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
internal class PlayerReport : IComparer<PlayerReport>
{
    // Used as an HTML id and inside jQuery/Bootstrap selectors (e.g. #table-<identifier>-damage),
    // so it must be a valid CSS identifier. Names can contain characters jQuery can't parse
    // ($, ., accents); slug everything outside [a-z0-9_-] to '_'.
    // Built from the full identity (account + character + profession), not just Name, so two
    // entries that share a character name but differ in account or class get distinct ids.
    public string? Identifier => Name == null ? null : Slug($"{Account}_{Name}_{Profession}");

    // Identity used to aggregate stats across fights. A single account can swap characters
    // and swap elite specialisation ("class") between fights; the user wants each distinct
    // account + character + profession tracked as its own row rather than collapsed together.
    [JsonIgnore]
    public string Key => $"{Account}|{Name}|{Profession}";

    private static string Slug(string name)
    {
        char[] chars = name.ToLowerInvariant().ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];
            bool ok = (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '-' || c == '_';
            if (!ok) { chars[i] = '_'; }
        }
        return new string(chars);
    }
    public string? Name { get; set; }
    public int? Group { get; set; }

    // Players can swap subgroups between fights, so a single fight's group isn't
    // representative for the cross-fight summary. Tally how many fights this account
    // spent in each subgroup and surface the most frequent one as Group.
    [JsonIgnore]
    public Dictionary<int, int> GroupCounts { get; set; } = [];

    // Record one fight's subgroup for this account and update Group to the most
    // frequent so far. Ties break toward the lower group number for determinism.
    public void RecordGroup(int group, int count = 1)
    {
        GroupCounts.TryGetValue(group, out int existing);
        GroupCounts[group] = existing + count;

        int bestGroup = group;
        int bestCount = -1;
        foreach (var kv in GroupCounts)
        {
            if (kv.Value > bestCount || (kv.Value == bestCount && kv.Key < bestGroup))
            {
                bestCount = kv.Value;
                bestGroup = kv.Key;
            }
        }
        Group = bestGroup;
    }

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

