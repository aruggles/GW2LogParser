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
        public string Name { get; set; }

        public int Icd { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public bool EnemyMech { get; set; }
        public bool PlayerMech { get; set; }

        private static List<int[]> GetMechanicData(IReadOnlyCollection<Mechanic> presMech, ParsedLog log, AbstractSingleActor actor, PhaseData phase)
        {
            var res = new List<int[]>();

            foreach (Mechanic mech in presMech)
            {
                int filterCount = 0;
                int count = 0;
                if (mech.InternalCooldown > 0)
                {
                    long timeFilter = 0;
                    var mls = log.MechanicData.GetMechanicLogs(log, mech).Where(x => x.Actor == actor).ToList();
                    foreach (MechanicEvent ml in mls)
                    {
                        bool inInterval = phase.InInterval(ml.Time);
                        if (ml.Time - timeFilter < mech.InternalCooldown)//ICD check
                        {
                            if (inInterval)
                            {
                                filterCount++;
                            }
                        }
                        timeFilter = ml.Time;
                        if (inInterval)
                        {
                            count++;
                        }
                    }
                }
                else
                {
                    count = log.MechanicData.GetMechanicLogs(log, mech).Where(x => x.Actor == actor && phase.InInterval(x.Time)).Count();
                }
                res.Add(new int[] { count - filterCount, count });
            }
            return res;
        }

        public static void BuildMechanics(IReadOnlyCollection<Mechanic> mechs, List<MechanicDto> mechsDtos)
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

        public static List<List<int[]>> BuildPlayerMechanicData(ParsedLog log, PhaseData phase)
        {
            var list = new List<List<int[]>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(GetMechanicData(log.MechanicData.GetPresentFriendlyMechs(log, 0, log.FightData.FightEnd), log, actor, phase));
            }
            return list;
        }

        public static List<List<int[]>> BuildEnemyMechanicData(ParsedLog log, PhaseData phase)
        {
            var list = new List<List<int[]>>();
            foreach (AbstractSingleActor enemy in log.MechanicData.GetEnemyList(log, 0, log.FightData.FightEnd))
            {
                list.Add(GetMechanicData(log.MechanicData.GetPresentEnemyMechs(log, 0, log.FightData.FightEnd), log, enemy, phase));
            }
            return list;
        }
    }
}
