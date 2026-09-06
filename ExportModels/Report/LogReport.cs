using System.Collections.Generic;

namespace Gw2LogParser.ExportModels.Report;

internal class LogReport
{
    public string Name { get; set; } = "";
    // File name of this fight's own report (fight_<n>.html) inside the report folder. The
    // number is the grid row's stable index, NOT the fight's chronological position in the
    // summary, so the summary must link through this rather than its own row index.
    public string FileName { get; set; } = "";
    public Dictionary<string, PlayerReport> players { get; set; } = new Dictionary<string, PlayerReport>();
    // Skills referenced by any player's SkillCasts in this fight, keyed by skill ID.
    public Dictionary<long, SkillUsageReport> Skills { get; set; } = new Dictionary<long, SkillUsageReport>();
    public long LogsStart { get; set; }
    public long LogsEnd { get; set; }
    public long Duration { get; set; }
    public string DurationString { get; set; } = "";
    public string PointOfView { get; set; } = "";

    // Per-fight aggregates surfaced in the summary's "Fights In This Report" table.
    public string StartTime { get; set; } = "";   // wall-clock start (LogMetadata.DateStart)
    public string MapName { get; set; } = "";      // WvW map/borderland name; empty for non-WvW
    public bool Success { get; set; }              // Win/Loss
    // Counts
    public int SquadSize { get; set; }
    public int AlliesOutsideSquad { get; set; }
    public int TotalEnemies { get; set; }
    // Outcome
    public int AlliesDowned { get; set; }
    public int AlliesDead { get; set; }
    public int AlliesRevived { get; set; }
    public int EnemyDowns { get; set; }
    public int EnemyDeaths { get; set; }
    // Damage
    public long OutgoingDamage { get; set; }
    public long IncomingDamage { get; set; }
    public long DamageDelta { get; set; }            // Outgoing - Incoming
    public long SquadBarrierAbsorbed { get; set; }
    public long EnemyBarrierAbsorbed { get; set; }
    public long BarrierDelta { get; set; }           // Squad - Enemy
}
