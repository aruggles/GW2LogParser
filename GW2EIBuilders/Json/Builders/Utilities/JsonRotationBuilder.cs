﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.GW2EIBuilders.JsonRotation;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonRotationBuilder
    {
        private static JsonSkill BuildJsonSkill(AbstractCastEvent cl)
        {
            var jsonSkill = new JsonSkill();
            jsonSkill.CastTime = (int)cl.Time;
            jsonSkill.Duration = cl.ActualDuration;
            jsonSkill.TimeGained = cl.SavedDuration;
            jsonSkill.Quickness = cl.Acceleration;
            return jsonSkill;
        }
        private static JsonRotation BuildJsonRotation(ParsedLog log, long skillID, List<AbstractCastEvent> skillCasts, Dictionary<string, JsonLog.SkillDesc> skillDesc)
        {
            var jsonRotation = new JsonRotation();
            if (!skillDesc.ContainsKey("s" + skillID))
            {
                Skill skill = skillCasts.First().Skill;
                skillDesc["s" + skillID] = JsonLogBuilder.BuildSkillDesc(skill, log);
            }
            jsonRotation.Id = skillID;
            jsonRotation.Skills = skillCasts.Select(x => BuildJsonSkill(x)).ToList();
            return jsonRotation;
        }

        public static List<JsonRotation> BuildJsonRotationList(ParsedLog log, Dictionary<long, List<AbstractCastEvent>> skillByID, Dictionary<string, JsonLog.SkillDesc> skillDesc)
        {
            var res = new List<JsonRotation>();
            foreach (KeyValuePair<long, List<AbstractCastEvent>> pair in skillByID)
            {
                res.Add(BuildJsonRotation(log, pair.Key, pair.Value, skillDesc));
            }
            return res;
        }
    }
}