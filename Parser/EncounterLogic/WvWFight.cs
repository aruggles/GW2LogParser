using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.Events.MetaData;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal class WvWFight : FightLogic
    {
        private readonly string _defaultName;
        private readonly bool _detailed;
        public WvWFight(int triggerID, bool detailed) : base(triggerID)
        {
            Mode = ParseMode.WvW;
            Icon = "https://wiki.guildwars2.com/images/3/35/WvW_Rank_up.png";
            _detailed = detailed;
            Extension = _detailed ? "detailed_wvw" : "wvw";
            _defaultName = _detailed ? "Detailed WvW" : "World vs World";
            EncounterCategoryInformation.Category = FightCategory.WvW;
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>();
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.WorldVersusWorld);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            if (_detailed)
            {
                phases.Add(new PhaseData(phases[0].Start, phases[0].End)
                {
                    Name = "Detailed Full Fight",
                    CanBeSubPhase = false
                });
                phases[1].AddTargets(Targets);
                if (phases[1].Targets.Any())
                {
                    phases[1].RemoveTarget(mainTarget);
                }
                phases[0].Dummy = true;
            }
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            MapIDEvent mapID = log.CombatData.GetMapIDEvents().LastOrDefault();
            if (mapID == null)
            {
                return base.GetCombatMapInternal(log);
            }
            switch (mapID.MapID)
            {
                // EB
                case 38:
                    return new CombatReplayMap("https://i.imgur.com/t0khtQd.png", (954, 1000), (-36864, -36864, 36864, 36864)/*, (-36864, -36864, 36864, 36864), (8958, 12798, 12030, 15870)*/);
                // Green Alpine
                case 95:
                    return new CombatReplayMap("https://i.imgur.com/nVu2ivF.png", (697, 1000), (-30720, -43008, 30720, 43008)/*, (-30720, -43008, 30720, 43008), (5630, 11518, 8190, 15102)*/);
                // Blue Alpine
                case 96:
                    return new CombatReplayMap("https://i.imgur.com/nVu2ivF.png", (697, 1000), (-30720, -43008, 30720, 43008)/*, (-30720, -43008, 30720, 43008), (12798, 10878, 15358, 14462)*/);
                // Red Desert
                case 1099:
                    return new CombatReplayMap("https://i.imgur.com/R5p9fqw.png", (1000, 1000), (-36864, -36864, 36864, 36864)/*, (-36864, -36864, 36864, 36864), (9214, 8958, 12286, 12030)*/);
                case 968:
                    return new CombatReplayMap("https://i.imgur.com/iEpKYL0.jpg", (3556, 3646), (-36864, -36864, 36864, 36864)/*, (-36864, -36864, 36864, 36864), (9214, 8958, 12286, 12030)*/);
            }
            return base.GetCombatMapInternal(log);
        }
        internal override string GetLogicName(ParsedLog log)
        {
            MapIDEvent mapID = log.CombatData.GetMapIDEvents().LastOrDefault();
            if (mapID == null)
            {
                return _defaultName;
            }
            switch (mapID.MapID)
            {
                case 38:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.EternalBattlegrounds;
                    return _defaultName + " - Eternal Battlegrounds";
                case 95:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.GreenAlpineBorderlands;
                    return _defaultName + " - Green Alpine Borderlands";
                case 96:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.BlueAlpineBorderlands;
                    return _defaultName + " - Blue Alpine Borderlands";
                case 1099:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.RedDesertBorderlands;
                    return _defaultName + " - Red Desert Borderlands";
                case 899:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.ObsidianSanctum;
                    return _defaultName + " - Obsidian Sanctum";
                case 968:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.EdgeOfTheMists;
                    return _defaultName + " - Edge of the Mists";
                case 1315:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.ArmisticeBastion;
                    return _defaultName + " - Armistice Bastion";
            }
            return _defaultName;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            fightData.SetSuccess(true, fightData.FightEnd);
        }

        private void SolveWvWPlayers(AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            IReadOnlyList<Agent> aList = agentData.GetAgentByType(Agent.AgentType.NonSquadPlayer);
            var set = new HashSet<string>();
            var toRemove = new HashSet<Agent>();
            var garbageList = new List<AbstractSingleActor>();
            var teamChangeDict = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.TeamChange).GroupBy(x => x.SrcAgent).ToDictionary(x => x.Key, x => x.ToList());
            //
            IReadOnlyList<Agent> squadPlayers = agentData.GetAgentByType(Agent.AgentType.Player);
            ulong greenTeam = ulong.MaxValue;
            var greenTeams = new List<ulong>();
            foreach (Agent a in squadPlayers)
            {
                if (teamChangeDict.TryGetValue(a.AgentValue, out List<Combat> teamChangeList))
                {
                    greenTeams.AddRange(teamChangeList.Where(x => x.SrcMatchesAgent(a)).Select(x => x.DstAgent));
                }
            }
            if (greenTeams.Any())
            {
                greenTeam = greenTeams.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).First();
            }
            var playersToMerge = new Dictionary<PlayerNonSquad, AbstractSingleActor>();
            var agentsToPlayersToMerge = new Dictionary<ulong, PlayerNonSquad>();
            //
            foreach (Agent a in aList)
            {
                if (teamChangeDict.TryGetValue(a.AgentValue, out List<Combat> teamChangeList))
                {
                    a.OverrideIsNotInSquadFriendlyPlayer(teamChangeList.Where(x => x.SrcMatchesAgent(a)).Select(x => x.DstAgent).Any(x => x == greenTeam));
                }
                List<AbstractSingleActor> actorListToFill = a.IsNotInSquadFriendlyPlayer ? friendlies : _detailed ? _targets : garbageList;
                var nonSquadPlayer = new PlayerNonSquad(a);
                if (!set.Contains(nonSquadPlayer.Character))
                {
                    actorListToFill.Add(nonSquadPlayer);
                    set.Add(nonSquadPlayer.Character);
                }
                else
                {
                    // we merge
                    AbstractSingleActor mainPlayer = actorListToFill.FirstOrDefault(x => x.Character == nonSquadPlayer.Character);
                    playersToMerge[nonSquadPlayer] = mainPlayer;
                    agentsToPlayersToMerge[nonSquadPlayer.AgentItem.AgentValue] = nonSquadPlayer;
                }
            }
            if (playersToMerge.Any())
            {
                foreach (Combat c in combatData)
                {
                    if (agentsToPlayersToMerge.TryGetValue(c.SrcAgent, out PlayerNonSquad nonSquadPlayer) && c.SrcMatchesAgent(nonSquadPlayer.AgentItem, extensions))
                    {
                        AbstractSingleActor mainPlayer = playersToMerge[nonSquadPlayer];
                        c.OverrideSrcAgent(mainPlayer.AgentItem.AgentValue);
                    }
                    if (agentsToPlayersToMerge.TryGetValue(c.DstAgent, out nonSquadPlayer) && c.DstMatchesAgent(nonSquadPlayer.AgentItem, extensions))
                    {
                        AbstractSingleActor mainPlayer = playersToMerge[nonSquadPlayer];
                        c.OverrideDstAgent(mainPlayer.AgentItem.AgentValue);
                    }
                }
                foreach (KeyValuePair<PlayerNonSquad, AbstractSingleActor> pair in playersToMerge)
                {
                    PlayerNonSquad nonSquadPlayer = pair.Key;
                    AbstractSingleActor mainPlayer = pair.Value;
                    agentData.SwapMasters(nonSquadPlayer.AgentItem, mainPlayer.AgentItem);
                    mainPlayer.AgentItem.OverrideAwareTimes(Math.Min(nonSquadPlayer.FirstAware, mainPlayer.FirstAware), Math.Max(nonSquadPlayer.LastAware, mainPlayer.LastAware));
                    toRemove.Add(nonSquadPlayer.AgentItem);
                }
            }
            agentData.RemoveAllFrom(toRemove);
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            Agent dummyAgent = agentData.AddCustomAgent(0, fightData.FightEnd, Agent.AgentType.NPC, _detailed ? "Dummy WvW Agent" : "Enemy Players", ParserHelper.Spec.NPC, (int)ArcDPSEnums.TargetID.WorldVersusWorld, true);

            SolveWvWPlayers(agentData, combatData, friendlies, extensions);
            var friendlyAgents = new HashSet<Agent>(friendlies.Select(x => x.AgentItem));
            if (!_detailed)
            {
                var aList = agentData.GetAgentByType(Agent.AgentType.NonSquadPlayer).Where(x => !friendlyAgents.Contains(x)).ToList();
                var enemyPlayerDicts = aList.GroupBy(x => x.AgentValue).ToDictionary(x => x.Key, x => x.ToList());
                foreach (Combat c in combatData)
                {
                    if (c.IsDamage(extensions))
                    {
                        if (enemyPlayerDicts.TryGetValue(c.SrcAgent, out List<Agent> srcs))
                        {
                            foreach (Agent src in srcs)
                            {
                                if (c.SrcMatchesAgent(src, extensions))
                                {
                                    c.OverrideSrcAgent(dummyAgent.AgentValue);
                                    break;
                                }
                            }
                        }
                        if (enemyPlayerDicts.TryGetValue(c.DstAgent, out List<Agent> dsts))
                        {
                            foreach (Agent dst in dsts)
                            {
                                if (c.DstMatchesAgent(dst, extensions))
                                {
                                    c.OverrideDstAgent(dummyAgent.AgentValue);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            ComputeFightTargets(agentData, combatData, extensions);
        }
    }
}
