using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes
{
    internal class FirstHitOnPlayerMechanic : HitOnPlayerMechanic
    {
        protected override bool Keep(AbstractHealthDamageEvent c, ParsedLog log)
        {
            if (c.From == ParserHelper._unknownAgent || !base.Keep(c, log) || GetFirstHit(c.From, log) != c)
            {
                return false;
            }
            return true;
        }

        private readonly Dictionary<Agent, AbstractHealthDamageEvent> _firstHits = new Dictionary<Agent, AbstractHealthDamageEvent>();

        public FirstHitOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, SkillChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public FirstHitOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public FirstHitOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public FirstHitOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        private AbstractHealthDamageEvent GetFirstHit(Agent src, ParsedLog log)
        {
            if (!_firstHits.TryGetValue(src, out AbstractHealthDamageEvent evt))
            {
                AbstractHealthDamageEvent res = log.CombatData.GetDamageData(src).Where(x => x.SkillId == SkillId && x.To.Type == Agent.AgentType.Player && base.Keep(x, log)).FirstOrDefault();
                _firstHits[src] = res;
                return res;
            }
            return evt;
        }
    }
}
