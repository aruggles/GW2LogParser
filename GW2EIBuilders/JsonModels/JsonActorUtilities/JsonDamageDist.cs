﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Skills;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.GW2EIBuilders
{
    /// <summary>
    /// Class corresponding a damage distribution
    /// </summary>
    public class JsonDamageDist
    {
        [JsonProperty]
        /// <summary>
        /// Total damage done
        /// </summary>
        public int TotalDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Minimum damage done
        /// </summary>
        public int Min { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Maximum damage done
        /// </summary>
        public int Max { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of hits
        /// </summary>
        public int Hits { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of connected hits
        /// </summary>
        public int ConnectedHits { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of crits
        /// </summary>
        public int Crit { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of glances
        /// </summary>
        public int Glance { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of flanks
        /// </summary>
        public int Flank { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of times the hit missed due to blindness
        /// </summary>
        public int Missed { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of times the hit was invulned
        /// </summary>
        public int Invulned { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of times the hit nterrupted
        /// </summary>
        public int Interrupted { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of times the hit was evaded
        /// </summary>
        public int Evaded { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of times the hit was blocked
        /// </summary>
        public int Blocked { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Damage done against barrier, not necessarily included in total damage
        /// </summary>
        public int ShieldDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Critical damage
        /// </summary>
        public int CritDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// ID of the damaging skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// True if indirect damage \n
        /// If true, the id is a buff
        /// </summary>
        public bool IndirectDamage { get; internal set; }

        [JsonConstructor]
        internal JsonDamageDist()
        {

        }

        protected JsonDamageDist(long id, List<AbstractDamageEvent> list, ParsedLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            IndirectDamage = list.Exists(x => x is NonDirectDamageEvent);
            if (IndirectDamage)
            {
                if (!buffDesc.ContainsKey("b" + id))
                {
                    if (log.Buffs.BuffsByIds.TryGetValue(id, out Buff buff))
                    {
                        buffDesc["b" + id] = new JsonLog.BuffDesc(buff, log);
                    }
                    else
                    {
                        Skill skill = list.First().Skill;
                        var auxBoon = new Buff(skill.Name, id, skill.Icon);
                        buffDesc["b" + id] = new JsonLog.BuffDesc(auxBoon, log);
                    }
                }
            }
            else
            {
                if (!skillDesc.ContainsKey("s" + id))
                {
                    Skill skill = list.First().Skill;
                    skillDesc["s" + id] = new JsonLog.SkillDesc(skill, log.LogData.GW2Build, log.SkillData);
                }
            }
            Id = id;
            Min = int.MaxValue;
            Max = int.MinValue;
            foreach (AbstractDamageEvent dmgEvt in list)
            {
                Hits += dmgEvt.DoubleProcHit ? 0 : 1;
                TotalDamage += dmgEvt.Damage;
                Min = Math.Min(Min, dmgEvt.Damage);
                Max = Math.Max(Max, dmgEvt.Damage);
                if (!IndirectDamage)
                {
                    if (dmgEvt.HasHit)
                    {
                        Flank += dmgEvt.IsFlanking ? 1 : 0;
                        Glance += dmgEvt.HasGlanced ? 1 : 0;
                        Crit += dmgEvt.HasCrit ? 1 : 0;
                        CritDamage += dmgEvt.HasCrit ? dmgEvt.Damage : 0;
                    }
                    Missed += dmgEvt.IsBlind ? 1 : 0;
                    Evaded += dmgEvt.IsEvaded ? 1 : 0;
                    Blocked += dmgEvt.IsBlocked ? 1 : 0;
                    Interrupted += dmgEvt.HasInterrupted ? 1 : 0;
                }
                ConnectedHits += dmgEvt.HasHit ? 1 : 0;
                Invulned += dmgEvt.IsAbsorbed ? 1 : 0;
                ShieldDamage += dmgEvt.ShieldDamage;
            }
        }

        internal static List<JsonDamageDist> BuildJsonDamageDistList(Dictionary<long, List<AbstractDamageEvent>> dlsByID, ParsedLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<JsonDamageDist>();
            foreach (KeyValuePair<long, List<AbstractDamageEvent>> pair in dlsByID)
            {
                res.Add(new JsonDamageDist(pair.Key, pair.Value, log, skillDesc, buffDesc));
            }
            return res;
        }

    }
}
