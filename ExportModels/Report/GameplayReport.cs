using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.ExportModels.Report
{
    public class GameplayReport
    {
        public int CriticalHits { get; set; }
        public int CritableHits { get; set; }
        public int Flanking { get; set; }
        public int Glancing { get; set; }
        public int ConnectedHits { get; set; }
        public int Blined { get; set; }
        public int Interrupted { get; set; }
        public int Invulnerable { get; set; }
        public int Evaded { get; set; }
        public int Blocked { get; set; }
        public int Killed { get; set; }
        public int Downed { get; set; }
        public float Wasted { get; set; }
        public int WastedCount { get; set; }
        public float Saved { get; set; }
        public float SavedCount { get; set; }
        public int WeaponSwapped { get; set; }
        public double AvgDistanceToSquad { get; set; }
        public double AvgDistanceToTag { get; set; }

        public GameplayReport() { }
        public GameplayReport(GameplayReport other)
        {
            CriticalHits = other.CriticalHits;
            CritableHits = other.CritableHits;
            Flanking = other.Flanking;
            Glancing = other.Glancing;
            ConnectedHits = other.ConnectedHits;
            Blined = other.Blined;
            Interrupted = other.Interrupted;
            Invulnerable = other.Invulnerable;
            Evaded = other.Evaded;
            Blocked = other.Blocked;
            Downed = other.Downed;
            Killed = other.Killed;
            Wasted = other.Wasted;
            WastedCount = other.WastedCount;
            Saved = other.Saved;
            SavedCount = other.SavedCount;
            WeaponSwapped = other.WeaponSwapped;
            AvgDistanceToSquad = other.AvgDistanceToSquad;
            AvgDistanceToTag = other.AvgDistanceToTag;
        }
    }
}
