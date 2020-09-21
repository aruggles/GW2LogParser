using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Statistics;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.GW2EIBuilders
{
    public class BuffData
    {
        public double Avg { get; internal set; }
        public List<List<object>> Data { get; internal set; } = new List<List<object>>();

        private BuffData(Dictionary<long, FinalPlayerBuffs> boons, List<Buff> listToUse, double avg)
        {
            Avg = avg;
            foreach (Buff boon in listToUse)
            {
                var boonVals = new List<object>();
                Data.Add(boonVals);

                if (boons.TryGetValue(boon.ID, out FinalPlayerBuffs uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (boon.Type == Buff.BuffType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
            }
        }

        private BuffData(Dictionary<long, FinalBuffs> boons, List<Buff> listToUse, double avg)
        {
            Avg = avg;
            foreach (Buff boon in listToUse)
            {
                var boonVals = new List<object>();
                Data.Add(boonVals);

                if (boons.TryGetValue(boon.ID, out FinalBuffs uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (boon.Type == Buff.BuffType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
            }
        }

        private BuffData(Dictionary<long, FinalBuffsDictionary> boons, List<Buff> listToUse, Player player)
        {
            foreach (Buff boon in listToUse)
            {
                var boonData = new List<object>();
                if (boons.TryGetValue(boon.ID, out FinalBuffsDictionary toUse))
                {
                    if (toUse.Generated.ContainsKey(player))
                    {
                        boonData.Add(toUse.Generated[player]);
                        boonData.Add(toUse.Overstacked[player]);
                        boonData.Add(toUse.Wasted[player]);
                        boonData.Add(toUse.UnknownExtension[player]);
                        boonData.Add(toUse.Extension[player]);
                        boonData.Add(toUse.Extended[player]);
                    }
                    else
                    {
                        boonData.Add(0);
                        boonData.Add(0);
                        boonData.Add(0);
                        boonData.Add(0);
                        boonData.Add(0);
                        boonData.Add(0);
                    }
                }
                Data.Add(boonData);
            }
        }

        private BuffData(List<Buff> listToUse, Dictionary<long, FinalPlayerBuffs> uptimes)
        {
            foreach (Buff boon in listToUse)
            {
                if (uptimes.TryGetValue(boon.ID, out FinalPlayerBuffs uptime))
                {
                    Data.Add(new List<object>()
                        {
                            uptime.Generation,
                            uptime.Overstack,
                            uptime.Wasted,
                            uptime.UnknownExtended,
                            uptime.ByExtension,
                            uptime.Extended
                        });
                }
                else
                {
                    Data.Add(new List<object>()
                        {
                            0,
                            0,
                            0,
                            0,
                            0,
                            0
                        });
                }
            }
        }

        private BuffData(string prof, Dictionary<string, List<Buff>> boonsBySpec, Dictionary<long, FinalPlayerBuffs> boons)
        {
            foreach (Buff boon in boonsBySpec[prof])
            {
                var boonVals = new List<object>();
                Data.Add(boonVals);
                if (boons.TryGetValue(boon.ID, out FinalPlayerBuffs uptime))
                {
                    boonVals.Add(uptime.Uptime);
                    if (boon.Type == Buff.BuffType.Intensity && uptime.Presence > 0)
                    {
                        boonVals.Add(uptime.Presence);
                    }
                }
                else
                {
                    boonVals.Add(0);
                }
            }
        }

        //////
        internal static List<BuffData> BuildBuffUptimeData(ParsedLog log, List<Buff> listToUse, int phaseIndex)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            var list = new List<BuffData>();
            bool boonTable = listToUse.Select(x => x.Nature).Contains(Buff.BuffNature.Boon);

            foreach (Player player in log.PlayerList)
            {
                double avg = 0.0;
                if (boonTable)
                {
                    avg = player.GetGameplayStats(log, phaseIndex).AvgBoons;
                }
                list.Add(new BuffData(player.GetBuffs(log, phaseIndex, BuffEnum.Self), listToUse, avg));
            }
            return list;
        }

        internal static List<BuffData> BuildActiveBuffUptimeData(ParsedLog log, List<Buff> listToUse, int phaseIndex)
        {
            var list = new List<BuffData>();
            bool boonTable = listToUse.Select(x => x.Nature).Contains(Buff.BuffNature.Boon);

            foreach (Player player in log.PlayerList)
            {
                double avg = 0.0;
                if (boonTable)
                {
                    avg = player.GetGameplayStats(log, phaseIndex).AvgActiveBoons;
                }
                list.Add(new BuffData(player.GetActiveBuffs(log, phaseIndex, BuffEnum.Self), listToUse, avg));
            }
            return list;
        }

        //////
        internal static List<BuffData> BuildPersonalBuffUptimeData(ParsedLog log, Dictionary<string, List<Buff>> boonsBySpec, int phaseIndex)
        {
            var list = new List<BuffData>();
            foreach (Player player in log.PlayerList)
            {
                list.Add(new BuffData(player.Prof, boonsBySpec, player.GetBuffs(log, phaseIndex, BuffEnum.Self)));
            }
            return list;
        }

        internal static List<BuffData> BuildActivePersonalBuffUptimeData(ParsedLog log, Dictionary<string, List<Buff>> boonsBySpec, int phaseIndex)
        {
            var list = new List<BuffData>();
            foreach (Player player in log.PlayerList)
            {
                list.Add(new BuffData(player.Prof, boonsBySpec, player.GetActiveBuffs(log, phaseIndex, BuffEnum.Self)));
            }
            return list;
        }


        //////
        internal static List<BuffData> BuildBuffGenerationData(ParsedLog log, List<Buff> listToUse, int phaseIndex, BuffEnum target)
        {
            var list = new List<BuffData>();

            foreach (Player player in log.PlayerList)
            {
                Dictionary<long, FinalPlayerBuffs> uptimes;
                uptimes = player.GetBuffs(log, phaseIndex, target);
                list.Add(new BuffData(listToUse, uptimes));
            }
            return list;
        }

        internal static List<BuffData> BuildActiveBuffGenerationData(ParsedLog log, List<Buff> listToUse, int phaseIndex, BuffEnum target)
        {
            var list = new List<BuffData>();

            foreach (Player player in log.PlayerList)
            {
                Dictionary<long, FinalPlayerBuffs> uptimes;
                uptimes = player.GetActiveBuffs(log, phaseIndex, target);
                list.Add(new BuffData(listToUse, uptimes));
            }
            return list;
        }

        /////
        internal static List<BuffData> BuildTargetCondiData(ParsedLog log, int phaseIndex, NPC target)
        {
            Dictionary<long, FinalBuffsDictionary> conditions = target.GetBuffsDictionary(log, phaseIndex);
            var list = new List<BuffData>();

            foreach (Player player in log.PlayerList)
            {
                list.Add(new BuffData(conditions, log.Statistics.PresentConditions, player));
            }
            return list;
        }

        internal static BuffData BuildTargetCondiUptimeData(ParsedLog log, int phaseIndex, NPC target)
        {
            Dictionary<long, FinalBuffs> buffs = target.GetBuffs(log, phaseIndex);
            return new BuffData(buffs, log.Statistics.PresentConditions, target.GetGameplayStats(log, phaseIndex).AvgConditions);
        }

        internal static BuffData BuildTargetBoonData(ParsedLog log, int phaseIndex, NPC target)
        {
            Dictionary<long, FinalBuffs> buffs = target.GetBuffs(log, phaseIndex);
            return new BuffData(buffs, log.Statistics.PresentBoons, target.GetGameplayStats(log, phaseIndex).AvgBoons);
        }
    }
}
