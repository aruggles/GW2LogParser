
namespace Gw2LogParser.ExportModels.Report
{
    public class DamageReport
    {
        public long TargetDamage { get; set; }
        public long TargetPower { get; set; }
        public long TargetCondi { get; set; }
        public long AllDamage { get; set; }
        public long Power { get; set; }
        public long Condi { get; set; }

        public DamageReport()
        {

        }

        public DamageReport(DamageReport other)
        {
            TargetDamage = other.TargetDamage;
            TargetPower = other.TargetPower;
            TargetCondi = other.TargetCondi;
            AllDamage = other.AllDamage;
            Power = other.Power;
            Condi = other.Condi;
        }
    }
}
