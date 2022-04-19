﻿using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.Events.Mechanics;
using Gw2LogParser.Parser.Helper.CachingCollections;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Mechanics
{
    public class MechanicData
    {
        private readonly Dictionary<Mechanic, List<MechanicEvent>> _mechanicLogs = new Dictionary<Mechanic, List<MechanicEvent>>();

        private CachingCollection<HashSet<Mechanic>> _presentOnFriendliesMechanics;
        private CachingCollection<HashSet<Mechanic>> _presentOnEnemyMechanics;
        private CachingCollection<HashSet<Mechanic>> _presentMechanics;
        private CachingCollection<List<AbstractSingleActor>> _enemyList;

        internal MechanicData(List<Mechanic> fightMechanics)
        {
            foreach (Mechanic m in fightMechanics)
            {
                _mechanicLogs.Add(m, new List<MechanicEvent>());
            }
        }

        private void ComputeMechanics(ParsedLog log)
        {
            var regroupedMobs = new Dictionary<int, AbstractSingleActor>();
            foreach (Mechanic mech in _mechanicLogs.Keys)
            {
                mech.CheckMechanic(log, _mechanicLogs, regroupedMobs);
            }
            // regroup same mechanics with diff ids
            var altNames = new Dictionary<string, Mechanic>();
            var toRemove = new List<Mechanic>();
            foreach (Mechanic mech in _mechanicLogs.Keys)
            {
                if (altNames.ContainsKey(mech.ShortName))
                {
                    _mechanicLogs[altNames[mech.ShortName]].AddRange(_mechanicLogs[mech]);
                    toRemove.Add(mech);
                }
                else
                {
                    altNames.Add(mech.ShortName, mech);
                }
            }
            foreach (Mechanic mech in toRemove)
            {
                _mechanicLogs.Remove(mech);
            }
        }

        private void ProcessMechanics(ParsedLog log)
        {
            if (_presentMechanics != null)
            {
                return;
            }
            _presentOnFriendliesMechanics = new CachingCollection<HashSet<Mechanic>>(log);
            _presentOnEnemyMechanics = new CachingCollection<HashSet<Mechanic>>(log);
            _presentMechanics = new CachingCollection<HashSet<Mechanic>>(log);
            _enemyList = new CachingCollection<List<AbstractSingleActor>>(log);
            ComputeMechanics(log);
            var emptyMechanic = _mechanicLogs.Where(pair => pair.Value.Count == 0).Select(pair => pair.Key).ToList();
            foreach (Mechanic m in emptyMechanic)
            {
                _mechanicLogs.Remove(m);
            }
            foreach (KeyValuePair<Mechanic, List<MechanicEvent>> pair in _mechanicLogs)
            {
                pair.Value.Sort((x, y) => x.Time.CompareTo(y.Time));
            }
        }

        public Dictionary<Mechanic, List<MechanicEvent>>.ValueCollection GetAllMechanics(ParsedLog log)
        {
            ProcessMechanics(log);
            return _mechanicLogs.Values;
        }

        public IReadOnlyList<MechanicEvent> GetMechanicLogs(ParsedLog log, Mechanic mech)
        {
            ProcessMechanics(log);
            if (_mechanicLogs.TryGetValue(mech, out List<MechanicEvent> list))
            {
                return list;
            }
            return new List<MechanicEvent>();
        }

        internal IReadOnlyList<MechanicEvent> GetMechanicLogs(ParsedLog log, long id)
        {
            ProcessMechanics(log);
            Mechanic mech = _mechanicLogs.Keys.FirstOrDefault(x => x.SkillId == id);
            if (mech != null)
            {
                return _mechanicLogs[mech];
            }
            return new List<MechanicEvent>();
        }

        private void ComputeMechanicData(long start, long end)
        {
            var presentMechanics = new HashSet<Mechanic>();
            var presentOnEnemyMechanics = new HashSet<Mechanic>();
            var presentOnFriendliesMechanics = new HashSet<Mechanic>();
            var enemyHash = new HashSet<AbstractSingleActor>();
            foreach (KeyValuePair<Mechanic, List<MechanicEvent>> pair in _mechanicLogs)
            {
                if (pair.Value.Any(x => x.Time >= start && x.Time <= end))
                {
                    presentMechanics.Add(pair.Key);
                    if (pair.Key.IsEnemyMechanic)
                    {
                        presentOnEnemyMechanics.Add(pair.Key);
                    }
                    else if (pair.Key.ShowOnTable)
                    {
                        presentOnFriendliesMechanics.Add(pair.Key);
                    }
                }
            }
            // ready enemy list
            foreach (Mechanic m in _mechanicLogs.Keys.Where(x => x.IsEnemyMechanic))
            {
                foreach (MechanicEvent mechanicEvent in _mechanicLogs[m].Where(x => x.Time >= start && x.Time <= end))
                {
                    enemyHash.Add(mechanicEvent.Actor);
                }
            }
            _presentMechanics.Set(start, end, presentMechanics);
            _presentOnEnemyMechanics.Set(start, end, presentOnEnemyMechanics);
            _presentOnFriendliesMechanics.Set(start, end, presentOnFriendliesMechanics);
            _enemyList.Set(start, end, new List<AbstractSingleActor>(enemyHash));
        }

        public IReadOnlyCollection<Mechanic> GetPresentEnemyMechs(ParsedLog log, long start, long end)
        {
            ProcessMechanics(log);
            if (!_presentOnEnemyMechanics.HasKeys(start, end))
            {
                ComputeMechanicData(start, end);
            }
            return _presentOnEnemyMechanics.Get(start, end);
        }
        public IReadOnlyCollection<Mechanic> GetPresentFriendlyMechs(ParsedLog log, long start, long end)
        {
            ProcessMechanics(log);
            if (!_presentOnFriendliesMechanics.HasKeys(start, end))
            {
                ComputeMechanicData(start, end);
            }
            return _presentOnFriendliesMechanics.Get(start, end);
        }
        public IReadOnlyCollection<Mechanic> GetPresentMechanics(ParsedLog log, long start, long end)
        {
            ProcessMechanics(log);
            if (!_presentMechanics.HasKeys(start, end))
            {
                ComputeMechanicData(start, end);
            }
            return _presentMechanics.Get(start, end);
        }

        public IReadOnlyList<AbstractSingleActor> GetEnemyList(ParsedLog log, long start, long end)
        {
            ProcessMechanics(log);
            if (!_enemyList.HasKeys(start, end))
            {
                ComputeMechanicData(start, end);
            }
            return _enemyList.Get(start, end);
        }
    }
}
