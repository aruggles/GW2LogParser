
namespace Gw2LogParser.ExportModels.Report
{
    public class SummaryItem
    {
        public string Icon { get; set; }
        public string Skill { get; set; }
        public float Percent { get; set; }
        public long Damage { get; set; }
        public long BarrierDamage { get; set; }
        public int Min { get; set; }
        public int Avg { get; set; }
        public int Max { get; set; }
        public int Casts { get; set; }
        public int Hits { get; set; }
        public float HitsPerCast { get; set; }
        public int Crit { get; set; }
        public int Flank { get; set; }
        public int Glance { get; set; }
        public float Wasted { get; set; }
        public float Saved { get; set; }
        public double CritPercent
        {
            get
            {
                return (Hits == 0) ? 0 : ((double)Crit / (double)Hits) * 100.0;
            }
        }
        public double FlankPercent
        {
            get
            {
                return (Hits == 0) ? 0 : ((double)Flank / (double)Hits) * 100.0;
            }
        }
        public double GlancePercent
        {
            get
            {
                return (Hits == 0) ? 0 : ((double)Glance / (double)Hits) * 100.0;
            }
        }

        public SummaryItem() { }
        public SummaryItem(SummaryItem other)
        {
            Icon = other.Icon;
            Skill = other.Skill;
            Percent = other.Percent;
            Damage = other.Damage;
            BarrierDamage = other.BarrierDamage;
            Min = other.Min;
            Avg = other.Avg;
            Max = other.Max;
            Casts = other.Casts;
            Hits = other.Hits;
            HitsPerCast = other.HitsPerCast;
            Crit = other.Crit;
            Flank = other.Flank;
            Glance = other.Glance;
            Wasted = other.Wasted;
            Saved = other.Saved;
        }
    }
}
