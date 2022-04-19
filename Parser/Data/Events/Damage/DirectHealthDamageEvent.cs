using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.Events.Damage
{
    public class DirectHealthDamageEvent : AbstractHealthDamageEvent
    {
        internal DirectHealthDamageEvent(Combat evtcItem, AgentData agentData, SkillData skillData, ArcDPSEnums.PhysicalResult result) : base(evtcItem, agentData, skillData)
        {
            HealthDamage = evtcItem.Value;
            AgainstDowned = evtcItem.IsOffcycle == 1;
            IsAbsorbed = result == ArcDPSEnums.PhysicalResult.Absorb;
            IsBlind = result == ArcDPSEnums.PhysicalResult.Blind;
            IsBlocked = result == ArcDPSEnums.PhysicalResult.Block;
            HasCrit = result == ArcDPSEnums.PhysicalResult.Crit;
            HasDowned = result == ArcDPSEnums.PhysicalResult.Downed;
            IsEvaded = result == ArcDPSEnums.PhysicalResult.Evade;
            HasGlanced = result == ArcDPSEnums.PhysicalResult.Glance;
            HasKilled = result == ArcDPSEnums.PhysicalResult.KillingBlow;
            HasInterrupted = result == ArcDPSEnums.PhysicalResult.Interrupt;
            ShieldDamage = evtcItem.IsShields > 0 ? (int)evtcItem.OverstackValue : 0;
            HasHit = result == ArcDPSEnums.PhysicalResult.Normal || HasGlanced || HasCrit; //Downed and Interrupt omitted for now due to double procing mechanics || result == ParseEnum.PhysicalResult.Downed || result == ParseEnum.PhysicalResult.Interrupt;
            DoubleProcHit = HasDowned || HasInterrupted || HasKilled;
        }

        public override bool ConditionDamageBased(ParsedLog log)
        {
            return false;
        }
    }
}
