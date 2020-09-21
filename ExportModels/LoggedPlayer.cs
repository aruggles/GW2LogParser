using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.ExportModels
{
    public class LoggedPlayer
    {
        public int Group { get; set; }
        public int CombatReplayID { get; set; }
        public string Name { get; set; }
        public string Acc { get; set; }
        public string Profession { get; set; }
        public uint Condi { get; set; }
        public uint Conc { get; set; }
        public uint Heal { get; set; }
        public uint Tough { get; set; }
        public LoggedPlayerDetails Details { get; set; }

        public LoggedPlayer(Player player, LoggedPlayerDetails details)
        {
            Group = player.Group;
            Name = player.Character;
            Acc = player.Account;
            Profession = player.Prof;
            Condi = player.Condition;
            Conc = player.Concentration;
            Heal = player.Healing;
            Tough = player.Toughness;
            Details = details;
        }
    }
}
