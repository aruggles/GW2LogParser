﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Buffs.BuffSourceFinders
{
    class BuffSourceFinder20210921 : BuffSourceFinder20210511
    {
        private List<AbstractCastEvent> _vindicatorDodges = null;
        public BuffSourceFinder20210921(HashSet<long> boonIds) : base(boonIds)
        {
        }

        protected override List<Agent> CouldBeImperialImpact(long extension, long time, ParsedLog log)
        {
            if (extension > 2000)
            {
                return new List<Agent>();
            }
            if (_vindicatorDodges == null)
            {
                _vindicatorDodges = new List<AbstractCastEvent>();
                foreach (Player p in log.PlayerList)
                {
                    if (p.Spec == ParserHelper.Spec.Vindicator)
                    {
                        var dodges = p.GetIntersectingCastEvents(log, 0, log.FightData.FightEnd).Where(x => x.SkillId == Skill.DodgeId).ToList();
                        //
                        var buffApplyTimes = log.CombatData.GetBuffData(62994).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).Select(x => x.Time).ToList(); // Saint of zu Heltzer
                        buffApplyTimes.AddRange(log.CombatData.GetBuffData(62811).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).Select(x => x.Time)); // Forerunner of Death
                        dodges.RemoveAll(x => buffApplyTimes.Any(y => y >= x.Time && y <= x.EndTime + ParserHelper.ServerDelayConstant));
                        //
                        _vindicatorDodges.AddRange(dodges);
                    }
                }
                _vindicatorDodges = new List<AbstractCastEvent>(_vindicatorDodges.OrderBy(x => x.Time));
            }
            var candidates = _vindicatorDodges.Where(x => x.Time <= time && time <= x.EndTime + ParserHelper.ServerDelayConstant).ToList();
            return candidates.Select(x => x.Caster).ToList();
        }
    }
}
