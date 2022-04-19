﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Statistics;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.Actors
{
    public class Player : AbstractPlayer
    {
        // Fields

        // Constructors
        internal Player(Agent agent, bool noSquad) : base(agent)
        {
            if (agent.Type != Agent.AgentType.Player)
            {
                throw new InvalidDataException("Agent is not a Player");
            }
            string[] name = agent.Name.Split('\0');
            if (name.Length < 2)
            {
                throw new EvtcAgentException("Name problem on Player");
            }
            if (name[1].Length == 0 || name[2].Length == 0 || Character.Contains("-"))
            {
                throw new EvtcAgentException("Missing Group on Player");
            }
            Account = name[1].TrimStart(':');
            Group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
        }


        internal void MakeSquadless()
        {
            Group = 1;
        }

        internal void Anonymize(int index)
        {
            Character = "Player " + index;
            Account = "Account " + index;
        }

        internal override Dictionary<long, FinalActorBuffs>[] ComputeBuffs(ParsedLog log, long start, long end, BuffEnum type)
        {
            switch (type)
            {
                case BuffEnum.Group:
                    var otherPlayersInGroup = log.PlayerList
                        .Where(p => p.Group == Group && this != p)
                        .ToList();
                    return FinalActorBuffs.GetBuffsForPlayers(otherPlayersInGroup, log, AgentItem, start, end);
                case BuffEnum.OffGroup:
                    var offGroupPlayers = log.PlayerList.Where(p => p.Group != Group).ToList();
                    return FinalActorBuffs.GetBuffsForPlayers(offGroupPlayers, log, AgentItem, start, end);
                case BuffEnum.Squad:
                    var otherPlayers = log.PlayerList.Where(p => p != this).ToList();
                    return FinalActorBuffs.GetBuffsForPlayers(otherPlayers, log, AgentItem, start, end);
                case BuffEnum.Self:
                default:
                    return FinalActorBuffs.GetBuffsForSelf(log, this, start, end);
            }
        }

        public IReadOnlyList<Point3D> GetCombatReplayActivePositions(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            (IReadOnlyList<(long start, long end)> deads, _, IReadOnlyList<(long start, long end)> dcs) = GetStatus(log);
            var activePositions = new List<Point3D>(GetCombatReplayPolledPositions(log));
            for (int i = 0; i < activePositions.Count; i++)
            {
                Point3D cur = activePositions[i];
                foreach ((long start, long end) in deads)
                {
                    if (cur.Time >= start && cur.Time <= end)
                    {
                        activePositions[i] = null;
                    }
                }
                foreach ((long start, long end) in dcs)
                {
                    if (cur.Time >= start && cur.Time <= end)
                    {
                        activePositions[i] = null;
                    }
                }
            }
            return activePositions;
        }
    }
}
