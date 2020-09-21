
namespace Gw2LogParser.ExportModels.Report
{
    public class DefenseReport
    {
        public long DamageTaken { get; set; }
        public long DamageBarrier { get; set; }
        public int Blocked { get; set; }
        public int Invulned { get; set; }
        public int Interrupted { get; set; }
        public int Evaded { get; set; }
        public int Dodges { get; set; }
        public int Missed { get; set; }
        public int Downed { get; set; }
        public int Dead { get; set; }

        public DefenseReport()
        {

        }
        public DefenseReport(DefenseReport other)
        {
            DamageTaken = other.DamageTaken;
            DamageBarrier = other.DamageBarrier;
            Blocked = other.Blocked;
            Invulned = other.Invulned;
            Interrupted = other.Interrupted;
            Evaded = other.Evaded;
            Dodges = other.Dodges;
            Missed = other.Missed;
            Downed = other.Downed;
            Dead = other.Dead;
        }
    }
}
