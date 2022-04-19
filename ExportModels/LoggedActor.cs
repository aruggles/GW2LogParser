using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.ExportModels
{
    internal abstract class LoggedActor
    {
        public int UniqueID { get; set; }
        public string Name { get; set; }
        public uint Tough { get; set; }
        public uint Condi { get; set; }
        public uint Conc { get; set; }
        public uint Heal { get; set; }
        public string Icon { get; set; }
        public long Health { get; set; }
        public List<LoggedMinion> Minions { get; } = new List<LoggedMinion>();
        public LoggedActorDetails Details { get; internal set; }

        protected LoggedActor(AbstractSingleActor actor, ParsedLog log, LoggedActorDetails details)
        {
            Health = actor.GetHealth(log.CombatData);
            Condi = actor.Condition;
            Conc = actor.Concentration;
            Heal = actor.Healing;
            Icon = actor.GetIcon();
            Name = actor.Character;
            Tough = actor.Toughness;
            Details = details;
            UniqueID = actor.UniqueID;
            foreach (KeyValuePair<long, Minions> pair in actor.GetMinions(log))
            {
                Minions.Add(new LoggedMinion()
                {
                    Id = pair.Key,
                    Name = pair.Value.Character
                });
            }
        }
    }
}
