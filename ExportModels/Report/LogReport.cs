using System.Collections.Generic;

namespace Gw2LogParser.ExportModels.Report;

internal class LogReport
{
    public string Name { get; set; } = "";
    public Dictionary<string, PlayerReport> players { get; set; } = new Dictionary<string, PlayerReport>();
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
