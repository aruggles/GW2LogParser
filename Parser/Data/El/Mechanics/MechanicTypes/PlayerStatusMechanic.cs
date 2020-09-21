using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Mechanics;
using Gw2LogParser.Parser.Data.Skills;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes
{
    internal class PlayerStatusMechanic : Mechanic
    {

        public PlayerStatusMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public PlayerStatusMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            ShowOnTable = false;
        }

        internal override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            foreach (Player p in log.PlayerList)
            {
                var cList = new List<long>();
                switch (SkillId)
                {
                    case Skill.DeathId:
                        cList = combatData.GetDeadEvents(p.AgentItem).Select(x => x.Time).ToList();
                        break;
                    case Skill.DCId:
                        cList = combatData.GetDespawnEvents(p.AgentItem).Select(x => x.Time).ToList();
                        break;
                    case Skill.RespawnId:
                        cList = combatData.GetSpawnEvents(p.AgentItem).Select(x => x.Time).ToList();
                        break;
                    case Skill.AliveId:
                        cList = combatData.GetAliveEvents(p.AgentItem).Select(x => x.Time).ToList();
                        break;
                    case Skill.DownId:
                        cList = combatData.GetDownEvents(p.AgentItem).Select(x => x.Time).ToList();
                        var downByVaporForm = combatData.GetBuffRemoveAllData(5620).Where(x => x.To == p.AgentItem).Select(x => x.Time).ToList();
                        foreach (long time in downByVaporForm)
                        {
                            cList.RemoveAll(x => Math.Abs(x - time) < 20);
                        }
                        break;
                    case Skill.ResurrectId:
                        cList = log.CombatData.GetAnimatedCastData(p.AgentItem).Where(x => x.SkillId == Skill.ResurrectId).Select(x => x.Time).ToList();
                        break;
                }
                foreach (long time in cList)
                {
                    mechanicLogs[this].Add(new MechanicEvent(time, this, p));
                }
            }
        }
    }
}
