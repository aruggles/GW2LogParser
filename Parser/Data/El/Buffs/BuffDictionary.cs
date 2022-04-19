﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Buffs
{
    internal class BuffDictionary
    {
        private readonly Dictionary<long, List<AbstractBuffEvent>> _dict = new Dictionary<long, List<AbstractBuffEvent>>();
        // Constructors
        public BuffDictionary()
        {
        }

        public bool TryGetValue(long buffID, out List<AbstractBuffEvent> list)
        {
            if (_dict.TryGetValue(buffID, out list))
            {
                return true;
            }
            return false;
        }

        public void Add(ParsedLog log, Buff buff, AbstractBuffEvent buffEvent)
        {
            if (!buffEvent.IsBuffSimulatorCompliant(log.FightData.FightEnd, log.CombatData.HasStackIDs))
            {
                return;
            }
            buffEvent.TryFindSrc(log);
            if (_dict.TryGetValue(buff.ID, out List<AbstractBuffEvent> list))
            {
                list.Add(buffEvent);
                return;
            }
            _dict[buff.ID] = new List<AbstractBuffEvent>() { buffEvent };
        }

        /*private static int CompareBuffEventType(AbstractBuffEvent x, AbstractBuffEvent y)
        {
            if (x.Time < y.Time)
            {
                return -1;
            }
            else if (x.Time > y.Time)
            {
                return 1;
            }
            else
            {
                return x.CompareTo(y);
            }
        }*/


        public void Finalize(ParsedLog log, Agent agentItem, out HashSet<Buff> trackedBuffs)
        {
            // add buff remove all for each despawn events
            foreach (DespawnEvent dsp in log.CombatData.GetDespawnEvents(agentItem))
            {
                foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _dict)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, dsp.Time + ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            foreach (SpawnEvent sp in log.CombatData.GetSpawnEvents(agentItem))
            {
                foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _dict)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, agentItem, sp.Time - ParserHelper.ServerDelayConstant, int.MaxValue, log.SkillData.Get(pair.Key), BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            trackedBuffs = new HashSet<Buff>();
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _dict)
            {
                trackedBuffs.Add(log.Buffs.BuffsByIds[pair.Key]);
                var auxValue = pair.Value.OrderBy(x => x.Time).ToList();
                pair.Value.Clear();
                pair.Value.AddRange(auxValue);
            }
        }
    }
}
