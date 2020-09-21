using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Events.Mechanics;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes
{
    internal class HitOnEnemyMechanic : SkillMechanic
    {

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, SkillChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        internal override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            IEnumerable<Agent> agents = log.AgentData.GetNPCsByID((int)SkillId);
            foreach (Agent a in agents)
            {
                List<AbstractDamageEvent> combatitems = combatData.GetDamageTakenData(a);
                foreach (AbstractDamageEvent c in combatitems)
                {
                    if (c is DirectDamageEvent && c.HasHit && Keep(c, log))
                    {
                        foreach (Player p in log.PlayerList)
                        {
                            if (c.From.GetFinalMaster() == p.AgentItem)
                            {
                                mechanicLogs[this].Add(new MechanicEvent(c.Time, this, p));
                            }
                        }
                    }

                }
            }
        }
    }
}
