using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Helper;
using System.Linq;
using System.Collections.Generic;
using Gw2LogParser.Exceptions;

namespace Gw2LogParser.Parser.Logic
{
    internal class Freezie : RaidLogic
    {
        public Freezie(int triggerID) : base(triggerID)
        {
            Extension = "freezie";
            Icon = "https://wiki.guildwars2.com/images/thumb/8/8b/Freezie.jpg/189px-Freezie.jpg";
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Freezie);
            AbstractSingleActor heartTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TrashID.FreeziesFrozenHeart);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Freezie not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 895, mainTarget, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 1)
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mainTarget);
                }
                else
                {
                    phase.Name = "Heal " + (i) / 2;
                    phase.AddTarget(heartTarget);
                }
            }
            return phases;
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Freezie,
                (int)ArcDPSEnums.TrashID.FreeziesFrozenHeart
            };
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Freezie,
                (int)ArcDPSEnums.TrashID.FreeziesFrozenHeart
            };
        }
    }
}
