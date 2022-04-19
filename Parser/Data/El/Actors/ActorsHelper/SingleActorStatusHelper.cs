using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Actors.ActorsHelper
{
    class SingleActorStatusHelper : AbstractSingleActorHelper
    {
        private List<(long start, long end)> _deads;
        private List<(long start, long end)> _downs;
        private List<(long start, long end)> _dcs;
        //
        private List<DeathRecap> _deathRecaps;
        //weaponslist
        private string[] _weaponsArray;

        public SingleActorStatusHelper(AbstractSingleActor actor) : base(actor)
        {
        }



        public (IReadOnlyList<(long start, long end)>, IReadOnlyList<(long start, long end)>, IReadOnlyList<(long start, long end)>) GetStatus(ParsedLog log)
        {
            if (_deads == null)
            {
                _deads = new List<(long start, long end)>();
                _downs = new List<(long start, long end)>();
                _dcs = new List<(long start, long end)>();
                AgentItem.GetAgentStatus(_deads, _downs, _dcs, log.CombatData, log.FightData);
            }
            return (_deads, _downs, _dcs);
        }

        public long GetActiveDuration(ParsedLog log, long start, long end)
        {
            (IReadOnlyList<(long start, long end)> dead, IReadOnlyList<(long start, long end)> down, IReadOnlyList<(long start, long end)> dc) = GetStatus(log);
            return (end - start) -
                dead.Sum(x =>
                {
                    if (x.start <= end && x.end >= start)
                    {
                        long s = Math.Max(x.start, start);
                        long e = Math.Min(x.end, end);
                        return e - s;
                    }
                    return 0;
                }) -
                dc.Sum(x =>
                {
                    if (x.start <= end && x.end >= start)
                    {
                        long s = Math.Max(x.start, start);
                        long e = Math.Min(x.end, end);
                        return e - s;
                    }
                    return 0;
                });
        }


        public IReadOnlyList<string> GetWeaponsArray(ParsedLog log)
        {
            if (_weaponsArray == null)
            {
                EstimateWeapons(log);
            }
            return _weaponsArray;
        }

        private void EstimateWeapons(ParsedLog log)
        {
            if (!(Actor is AbstractPlayer))
            {
                _weaponsArray = new string[]
                {
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                };
                return;
            }
            string[] weapons = new string[8];//first 2 for first set next 2 for second set, second sets of 4 for underwater
            IReadOnlyList<AbstractCastEvent> casting = Actor.GetCastEvents(log, 0, log.FightData.FightEnd);
            int swapped = -1;
            long swappedTime = 0;
            var swaps = casting.OfType<WeaponSwapEvent>().Select(x =>
            {
                return x.SwappedTo;
            }).ToList();
            foreach (AbstractCastEvent cl in casting)
            {
                if (cl.ActualDuration == 0 && cl.SkillId != Skill.WeaponSwapId)
                {
                    continue;
                }
                Skill skill = cl.Skill;
                // first iteration
                if (swapped == -1)
                {
                    swapped = skill.FindWeaponSlot(swaps);
                }
                if (!skill.EstimateWeapons(weapons, swapped, cl.Time > swappedTime + ParserHelper.WeaponSwapDelayConstant) && cl is WeaponSwapEvent swe)
                {
                    //wepswap  
                    swapped = swe.SwappedTo;
                    swappedTime = swe.Time;
                }
            }
            _weaponsArray = weapons;
        }
        public IReadOnlyList<DeathRecap> GetDeathRecaps(ParsedLog log)
        {
            if (_deathRecaps == null)
            {
                SetDeathRecaps(log);
            }
            return _deathRecaps;
        }
        private void SetDeathRecaps(ParsedLog log)
        {
            _deathRecaps = new List<DeathRecap>();
            IReadOnlyList<DeadEvent> deads = log.CombatData.GetDeadEvents(AgentItem);
            IReadOnlyList<DownEvent> downs = log.CombatData.GetDownEvents(AgentItem);
            IReadOnlyList<AliveEvent> ups = log.CombatData.GetAliveEvents(AgentItem);
            long lastDeathTime = 0;
            IReadOnlyList<AbstractHealthDamageEvent> damageLogs = Actor.GetDamageTakenEvents(null, log, 0, log.FightData.FightEnd);
            foreach (DeadEvent dead in deads)
            {
                _deathRecaps.Add(new DeathRecap(log, damageLogs, dead, downs, ups, lastDeathTime));
                lastDeathTime = dead.Time;
            }
        }
    }
}
