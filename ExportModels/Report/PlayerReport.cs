using System;
using System.Collections.Generic;

namespace Gw2LogParser.ExportModels.Report
{
    public class PlayerReport: IComparer<PlayerReport>
    {
        public string Identifier
        {
            get
            {
                return Name.Replace(' ', '_').ToLower();
            }
        }
        public string Name { get; set; }
        public int Group { get; set; }
        public string Account { get; set; }
        public string Profession { get; set; }
        public string Icon { get; set; }
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
        public List<SummaryItem> DamageSummary { get; set; } = new List<SummaryItem>();
        public List<SummaryItem> TakenSummary { get; set; } = new List<SummaryItem>();
        public List<BoonInfo> BoonStats { get; internal set; } = new List<BoonInfo>();
        public List<BoonInfo> BoonGenSelfStats { get; internal set; } = new List<BoonInfo>();
        public List<BoonInfo> BoonGenGroupStats { get; internal set; } = new List<BoonInfo>();
        public List<BoonInfo> BoonGenOGroupStats { get; internal set; } = new List<BoonInfo>();
        public List<BoonInfo> BoonGenSquadStats { get; internal set; } = new List<BoonInfo>();
        public HealingReport healing = null;
        public int numberOfFights { get; set; } = 1;

        public PlayerReport()
        {

        }

        public int Compare(PlayerReport x, PlayerReport y)
        {
            if (x == null || y == null)
            {
                return 0;
            }
            return x.Name.CompareTo(y.Name);
        }

        public void setBoonStats(List<List<object>> boonStats)
        {
            foreach (var boon in boonStats)
            {
                var boonInfo = new BoonInfo();
                if (boon.Count > 1)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                    boonInfo.Uptime = (Parse<double>(boon[1]) * TimeInCombat);
                } else if (boon.Count > 0)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                }
                BoonStats.Add(boonInfo);
            }
        }

        public void setBoonGenSelfStats(List<List<object>> boonStats)
        {
            foreach (var boon in boonStats)
            {
                var boonInfo = new BoonInfo();
                if (boon.Count > 5)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                    boonInfo.Wasted = (Parse<double>(boon[2]) * TimeInCombat);
                    boonInfo.Extended = (Parse<double>(boon[5]) * TimeInCombat);
                } else if (boon.Count > 2)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                    boonInfo.Wasted = (Parse<double>(boon[2]) *  TimeInCombat);
                }
                else if (boon.Count > 0)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                }
                BoonGenSelfStats.Add(boonInfo);
            }
        }

        public void setBoonGenGroupStats(List<List<object>> boonStats)
        {
            foreach (var boon in boonStats)
            {
                var boonInfo = new BoonInfo();
                if (boon.Count > 5)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                    boonInfo.Wasted = (Parse<double>(boon[2]) * TimeInCombat);
                    boonInfo.Extended = (Parse<double>(boon[5]) * TimeInCombat);
                }
                else if (boon.Count > 2)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                    boonInfo.Wasted = (Parse<double>(boon[2]) * TimeInCombat);
                }
                else if (boon.Count > 0)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                }
                BoonGenGroupStats.Add(boonInfo);
            }
        }

        public void setBoonGenOGroupStats(List<List<object>> boonStats)
        {
            foreach (var boon in boonStats)
            {
                var boonInfo = new BoonInfo();
                if (boon.Count > 5)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                    boonInfo.Wasted = (Parse<double>(boon[2]) * TimeInCombat);
                    boonInfo.Extended = (Parse<double>(boon[5]) * TimeInCombat);
                }
                else if (boon.Count > 2)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                    boonInfo.Wasted = (Parse<double>(boon[2]) * TimeInCombat);
                }
                else if (boon.Count > 0)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                }
                BoonGenOGroupStats.Add(boonInfo);
            }
        }

        public void setBoonGenSquadStats(List<List<object>> boonStats)
        {
            foreach (var boon in boonStats)
            {
                var boonInfo = new BoonInfo();
                if (boon.Count > 5)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                    boonInfo.Wasted = (Parse<double>(boon[2]) * TimeInCombat);
                    boonInfo.Extended = (Parse<double>(boon[5]) * TimeInCombat);
                }
                else if (boon.Count > 2)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                    boonInfo.Wasted = (Parse<double>(boon[2]) * TimeInCombat);
                }
                else if (boon.Count > 0)
                {
                    boonInfo.Value = (Parse<double>(boon[0]) * TimeInCombat);
                }
                BoonGenSquadStats.Add(boonInfo);
            }
        }

        private T Parse<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
