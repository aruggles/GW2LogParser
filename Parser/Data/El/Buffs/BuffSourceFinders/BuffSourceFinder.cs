﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Buffs.BuffSourceFinders
{
    internal abstract class BuffSourceFinder
    {
        private List<AbstractCastEvent> _extensionSkills = null;
        private readonly HashSet<long> _boonIds = null;
        protected HashSet<long> ExtensionIDS { get; set; } = new HashSet<long>();
        protected Dictionary<long, HashSet<long>> DurationToIDs { get; set; } = new Dictionary<long, HashSet<long>>();
        // non trackable times
        protected long EssenceOfSpeed { get; set; }
        protected long ImbuedMelodies { get; set; }

        protected BuffSourceFinder(HashSet<long> boonIds)
        {
            _boonIds = boonIds;
        }

        private List<AbstractCastEvent> GetExtensionSkills(ParsedLog log, long time, HashSet<long> idsToKeep)
        {
            if (_extensionSkills == null)
            {
                _extensionSkills = new List<AbstractCastEvent>();
                foreach (Player p in log.PlayerList)
                {
                    _extensionSkills.AddRange(p.GetIntersectingCastEvents(log, 0, log.FightData.FightEnd).Where(x => ExtensionIDS.Contains(x.SkillId) && x.Status != AbstractCastEvent.AnimationStatus.Interrupted));
                }
            }
            return _extensionSkills.Where(x => idsToKeep.Contains(x.SkillId) && x.Time <= time && time <= x.EndTime + ParserHelper.ServerDelayConstant).ToList();
        }
        // Spec specific checks
        protected virtual int CouldBeEssenceOfSpeed(Agent dst, long extension, long buffID, ParsedLog log)
        {
            if (extension == EssenceOfSpeed && dst.Spec == ParserHelper.Spec.Soulbeast)
            {
                if (log.FriendliesListBySpec.ContainsKey(ParserHelper.Spec.Herald) ||
                    log.FriendliesListBySpec.ContainsKey(ParserHelper.Spec.Tempest) ||
                    log.FriendliesListBySpec.ContainsKey(ParserHelper.Spec.Vindicator))
                {
                    // uncertain, needs to check more
                    return 0;
                }
                return 1;
            }
            return -1;
        }

        protected virtual bool CouldBeImbuedMelodies(Agent agent, long time, long extension, ParsedLog log)
        {
            if (extension == ImbuedMelodies && log.FriendliesListBySpec.TryGetValue(ParserHelper.Spec.Tempest, out List<AbstractSingleActor> tempests))
            {
                var magAuraApplications = new HashSet<Agent>(log.CombatData.GetBuffData(5684).Where(x => x is BuffApplyEvent && Math.Abs(x.Time - time) < ParserHelper.ServerDelayConstant && x.CreditedBy != agent).Select(x => x.CreditedBy));
                foreach (AbstractSingleActor tempest in tempests)
                {
                    if (magAuraApplications.Contains(tempest.AgentItem))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual List<Agent> CouldBeImperialImpact(long extension, long time, ParsedLog log)
        {
            return new List<Agent>();
        }

        protected virtual HashSet<long> GetIDs(ParsedLog log, long buffID, long extension)
        {
            if (DurationToIDs.TryGetValue(extension, out HashSet<long> idsToCheck))
            {
                return idsToCheck;
            }
            return new HashSet<long>();
        }

        // Main method
        public Agent TryFindSrc(Agent dst, long time, long extension, ParsedLog log, long buffID)
        {
            if (!_boonIds.Contains(buffID))
            {
                return dst;
            }
            List<Agent> imperialImpactCheck = CouldBeImperialImpact(extension, time, log);
            if (imperialImpactCheck.Count > 1)
            {
                return ParserHelper._unknownAgent;
            }
            int essenceOfSpeedCheck = CouldBeEssenceOfSpeed(dst, extension, buffID, log);
            // can only be the soulbeast
            if (essenceOfSpeedCheck == 1)
            {
                return dst;
            }
            HashSet<long> idsToCheck = GetIDs(log, buffID, extension);
            if (idsToCheck.Any())
            {
                List<AbstractCastEvent> cls = GetExtensionSkills(log, time, idsToCheck);
                // If only one cast item
                if (cls.Count == 1)
                {
                    AbstractCastEvent item = cls.First();
                    // If uncertainty due to essence of speed, imbued melodies or imperial impact, return unknown
                    if (essenceOfSpeedCheck == 0 || CouldBeImbuedMelodies(item.Caster, time, extension, log) || imperialImpactCheck.Any())
                    {
                        return ParserHelper._unknownAgent;
                    }
                    // otherwise the src is the caster
                    return item.Caster;
                }
                // If no cast item and 
                else if (!cls.Any())
                {
                    // If uncertainty due to imbued melodies, return unknown
                    if (CouldBeImbuedMelodies(dst, time, extension, log))
                    {
                        return ParserHelper._unknownAgent;
                    }
                    // uncertainty due to essence of speed but not due to imperial impact
                    if (essenceOfSpeedCheck == 0 && !imperialImpactCheck.Any())
                    {
                        // the soulbeast
                        return dst;
                    }
                    // uncertainty due to imperial impact but not due to essence of speed
                    if (essenceOfSpeedCheck == -1 && imperialImpactCheck.Count == 1)
                    {
                        // the vindicator
                        return imperialImpactCheck.First();
                    }
                }
            }
            return ParserHelper._unknownAgent;
        }
    }
}
