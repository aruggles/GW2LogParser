using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Extensions.HealingStatsExtensionHandler;

namespace Gw2LogParser.Parser.Extensions
{
    public class EXTHealingCombatData
    {
        private readonly Dictionary<Agent, List<EXTAbstractHealingEvent>> _healData;
        private readonly Dictionary<Agent, List<EXTAbstractHealingEvent>> _healReceivedData;
        private readonly Dictionary<long, List<EXTAbstractHealingEvent>> _healDataById;

        private readonly Dictionary<long, EXTHealingType> EncounteredIDs = new Dictionary<long, EXTHealingType>();

        private readonly HashSet<long> HybridHealIDs;

        internal EXTHealingCombatData(Dictionary<Agent, List<EXTAbstractHealingEvent>> healData, Dictionary<Agent, List<EXTAbstractHealingEvent>> healReceivedData, Dictionary<long, List<EXTAbstractHealingEvent>> healDataById, HashSet<long> hybridHealIDs)
        {
            _healData = healData;
            _healReceivedData = healReceivedData;
            _healDataById = healDataById;
            HybridHealIDs = hybridHealIDs;
        }

        public IReadOnlyList<EXTAbstractHealingEvent> GetHealData(Agent key)
        {
            if (_healData.TryGetValue(key, out List<EXTAbstractHealingEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractHealingEvent>();
        }
        public IReadOnlyList<EXTAbstractHealingEvent> GetHealReceivedData(Agent key)
        {
            if (_healReceivedData.TryGetValue(key, out List<EXTAbstractHealingEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractHealingEvent>();
        }

        public IReadOnlyList<EXTAbstractHealingEvent> GetHealData(long key)
        {
            if (_healDataById.TryGetValue(key, out List<EXTAbstractHealingEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractHealingEvent>();
        }

        public EXTHealingType GetHealingType(long id, ParsedLog log)
        {
            if (HybridHealIDs.Contains(id))
            {
                return EXTHealingType.Hybrid;
            }
            if (EncounteredIDs.TryGetValue(id, out EXTHealingType type))
            {
                return type;
            }
            if (log.CombatData.GetDamageData(id).Any(x => x.HealthDamage > 0 && !x.DoubleProcHit))
            {
                type = EXTHealingType.ConversionBased;
            }
            else
            {
                type = EXTHealingType.HealingPower;
            }
            EncounteredIDs[id] = type;
            return type;
        }

        public EXTHealingType GetHealingType(Skill skill, ParsedLog log)
        {
            return GetHealingType(skill.ID, log);
        }

        public EXTHealingType GetHealingType(Buff buff, ParsedLog log)
        {
            return GetHealingType(buff.ID, log);
        }
    }
}
