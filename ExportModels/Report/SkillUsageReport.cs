using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gw2LogParser.ExportModels.Report;

// Static description of one skill that appeared in the Skill Usage report. Kept once per
// report (Report.Skills, keyed by skill ID) rather than per player so the payload doesn't
// repeat names/icons for every row. Per-player cast counts live in PlayerReport.SkillCasts.
[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
internal class SkillUsageReport
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    // EI's auto-attack classification (weapon slot 1, excluding stealth/ambush attacks).
    // Drives the "APM (no AA)" figure and the column ordering.
    public bool AutoAttack { get; set; }
    public bool Swap { get; set; }
    public bool Dodge { get; set; }
}
