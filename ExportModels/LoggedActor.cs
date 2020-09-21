using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.ExportModels
{
    public abstract class LoggedActor
    {
        public int CombatReplayID { get; internal set; }
        public string Name { get; internal set; }
        public uint Tough { get; internal set; }
        public List<LoggedMinion> Minions { get; } = new List<LoggedMinion>();
        public LoggedActorDetails Details { get; internal set; }

        protected LoggedActor(AbstractSingleActor actor, ParsedLog log, bool cr, LoggedActorDetails details)
        {
            Name = actor.Character;
            Tough = actor.Toughness;
            Details = details;
            if (cr)
            {
                CombatReplayID = actor.CombatReplayID;
            }
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
