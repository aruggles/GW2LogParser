﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.Events.Mechanics;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.GW2EIBuilders
{
    public class MechanicDto
    {
        public string Name { get; internal set; }

        public int Icd { get; internal set; }
        public string ShortName { get; internal set; }
        public string Description { get; internal set; }
        public bool EnemyMech { get; internal set; }
        public bool PlayerMech { get; internal set; }

        private static List<int[]> GetMechanicData(HashSet<Mechanic> presMech, ParsedLog log, AbstractActor actor, PhaseData phase)
        {
            var res = new List<int[]>();

            foreach (Mechanic mech in presMech)
            {
                long timeFilter = 0;
                int filterCount = 0;
                var mls = log.MechanicData.GetMechanicLogs(log, mech).Where(x => x.Actor.Agent == actor.Agent && phase.InInterval(x.Time)).ToList();
                int count = mls.Count;
                foreach (MechanicEvent ml in mls)
                {
                    if (mech.InternalCooldown != 0 && ml.Time - timeFilter < mech.InternalCooldown)//ICD check
                    {
                        filterCount++;
                    }
                    timeFilter = ml.Time;

                }
                res.Add(new int[] { count - filterCount, count });
            }
            return res;
        }

        internal static void BuildMechanics(HashSet<Mechanic> mechs, List<MechanicDto> mechsDtos)
        {
            foreach (Mechanic mech in mechs)
            {
                var dto = new MechanicDto
                {
                    Name = mech.FullName,
                    ShortName = mech.ShortName,
                    Description = mech.Description,
                    PlayerMech = mech.ShowOnTable && !mech.IsEnemyMechanic,
                    EnemyMech = mech.IsEnemyMechanic,
                    Icd = mech.InternalCooldown
                };
                mechsDtos.Add(dto);
            }
        }

        internal static List<List<int[]>> BuildPlayerMechanicData(ParsedLog log, int phaseIndex)
        {
            var list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = log.MechanicData.GetPresentPlayerMechs(log, 0);
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];

            foreach (Player p in log.PlayerList)
            {
                list.Add(GetMechanicData(presMech, log, p, phase));
            }
            return list;
        }

        internal static List<List<int[]>> BuildEnemyMechanicData(ParsedLog log, int phaseIndex)
        {
            var list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = log.MechanicData.GetPresentEnemyMechs(log, 0);
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            foreach (AbstractActor enemy in log.MechanicData.GetEnemyList(log, 0))
            {
                list.Add(GetMechanicData(presMech, log, enemy, phase));
            }
            return list;
        }
    }
}
