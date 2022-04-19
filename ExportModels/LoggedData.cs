using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.ExportModels
{
    internal class LoggedData
    {
        public List<LoggedPlayer> Players { get; } = new List<LoggedPlayer>();
        public List<LoggedPhase> Phases { get; } = new List<LoggedPhase>();
        public List<long> Boons { get; } = new List<long>();
        public List<long> OffBuffs { get; } = new List<long>();
        public List<long> DefBuffs { get; } = new List<long>();
        public List<long> Conditions { get; } = new List<long>();
        public Dictionary<string, LoggedSkill> SkillMap { get; } = new Dictionary<string, LoggedSkill>();
        public Dictionary<string, LoggedBoon> BuffMap { get; } = new Dictionary<string, LoggedBoon>();
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public string PoVName { get; set; }
        public string EncounterDuration { get; set; }
    }
}
