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
            // A player with no active time (dead/disconnected for the whole fight) would
            // otherwise produce NaN or Infinity, which Newtonsoft serialises as a string.
            if (TimeInCombat <= 0) { return 0; }
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
    // Skill Usage report: casts per skill ID (see Report.Skills for the skill metadata),
    // summed across fights. Only the player's own casts: no minions, no interrupted casts,
    // no downed-state skills. Weapon swaps and EI-inferred instant casts (trait/gear procs)
    // are included.
    public Dictionary<long, int> SkillCasts { get; set; } = [];
    public int numberOfFights { get; set; } = 1;

    public PlayerReport()
    {

    }

    // Deep copy. Per-fight reports are retained (ProgramHelper.CompletedLogs) so the summary can
    // be regenerated as more fights arrive; the merge must therefore start from a copy rather
    // than accumulate into the first fight's own object.
    public PlayerReport(PlayerReport other)
    {
        Name = other.Name;
        Group = other.Group;
        GroupCounts = new Dictionary<int, int>(other.GroupCounts);
        Account = other.Account;
        Profession = other.Profession;
        Icon = other.Icon;
        TimeInCombat = other.TimeInCombat;
        Damage = new DamageReport(other.Damage);
        Defense = new DefenseReport(other.Defense);
        Support = new SupportReport(other.Support);
        Gameplay = new GameplayReport(other.Gameplay);
        DamageSummary = other.DamageSummary.ConvertAll(s => new SummaryItem(s));
        TakenSummary = other.TakenSummary.ConvertAll(s => new SummaryItem(s));
        BoonStats = other.BoonStats.ConvertAll(b => new BoonReport(b));
        BoonGenSelfStats = other.BoonGenSelfStats.ConvertAll(b => new BoonReport(b));
        BoonGenGroupStats = other.BoonGenGroupStats.ConvertAll(b => new BoonReport(b));
        BoonGenOGroupStats = other.BoonGenOGroupStats.ConvertAll(b => new BoonReport(b));
        BoonGenSquadStats = other.BoonGenSquadStats.ConvertAll(b => new BoonReport(b));
        healing = other.healing == null ? null : new HealingReport(other.healing);
        SkillCasts = new Dictionary<long, int>(other.SkillCasts);
        numberOfFights = other.numberOfFights;
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

