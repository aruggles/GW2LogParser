using System;
using System.Collections.Generic;

namespace Gw2LogParser.ExportModels.Report
{
    public class PlayerReport: IComparer<PlayerReport>
    {
        public string Name { get; set; }
        public int Group { get; set; }
        public string Account { get; set; }
        public string Profession { get; set; }
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
                    boonInfo.Value = Parse<double>(boon[0]);
                    boonInfo.Uptime = Parse<double>(boon[1]);
                } else if (boon.Count > 0)
                {
                    boonInfo.Value = Parse<double>(boon[0]);
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
                    boonInfo.Value = Parse<double>(boon[0]);
                    boonInfo.Wasted = Parse<double>(boon[2]);
                    boonInfo.Extended = Parse<double>(boon[5]);
                } else if (boon.Count > 2)
                {
                    boonInfo.Value = Parse<double>(boon[0]);
                    boonInfo.Wasted = Parse<double>(boon[2]);
                }
                else if (boon.Count > 0)
                {
                    boonInfo.Value = Parse<double>(boon[0]);
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
                    boonInfo.Value = Parse<double>(boon[0]);
                    boonInfo.Wasted = Parse<double>(boon[2]);
                    boonInfo.Extended = Parse<double>(boon[5]);
                }
                else if (boon.Count > 2)
                {
                    boonInfo.Value = Parse<double>(boon[0]);
                    boonInfo.Wasted = Parse<double>(boon[2]);
                }
                else if (boon.Count > 0)
                {
                    boonInfo.Value = Parse<double>(boon[0]);
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
                    boonInfo.Value = Parse<double>(boon[0]);
                    boonInfo.Wasted = Parse<double>(boon[2]);
                    boonInfo.Extended = Parse<double>(boon[5]);
                }
                else if (boon.Count > 2)
                {
                    boonInfo.Value = Parse<double>(boon[0]);
                    boonInfo.Wasted = Parse<double>(boon[2]);
                }
                else if (boon.Count > 0)
                {
                    boonInfo.Value = Parse<double>(boon[0]);
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
                    boonInfo.Value = Parse<double>(boon[0]);
                    boonInfo.Wasted = Parse<double>(boon[2]);
                    boonInfo.Extended = Parse<double>(boon[5]);
                }
                else if (boon.Count > 2)
                {
                    boonInfo.Value = Parse<double>(boon[0]);
                    boonInfo.Wasted = Parse<double>(boon[2]);
                }
                else if (boon.Count > 0)
                {
                    boonInfo.Value = Parse<double>(boon[0]);
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
