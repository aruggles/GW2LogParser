using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Mechanics;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes
{
    internal class SpawnMechanic : Mechanic
    {

        public SpawnMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public SpawnMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        internal override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (Agent a in log.AgentData.GetNPCsByID((int)SkillId))
            {
                if (!regroupedMobs.TryGetValue(a.ID, out AbstractSingleActor amp))
                {
                    amp = log.FindActor(a, false);
                    if (amp == null)
                    {
                        continue;
                    }
                    regroupedMobs.Add(amp.ID, amp);
                }
                mechanicLogs[this].Add(new MechanicEvent(a.FirstAware, this, amp));
            }
        }
    }
}
