﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Actors;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{

    internal abstract class ActorDto
    {
        public int UniqueID { get; set; }
        public string Name { get; set; }
        public uint Tough { get; set; }
        public uint Condi { get; set; }
        public uint Conc { get; set; }
        public uint Heal { get; set; }
        public string Icon { get; set; }
        public long Health { get; set; }
        public List<MinionDto> Minions { get; } = new List<MinionDto>();
        public ActorDetailsDto Details { get; set; }

        protected ActorDto(AbstractSingleActor actor, ParsedLog log, ActorDetailsDto details)
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
                Minions.Add(new MinionDto()
                {
                    Id = pair.Key,
                    Name = pair.Value.Character
                });
            }
        }
    }
}
