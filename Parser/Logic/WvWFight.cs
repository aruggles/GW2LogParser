using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.Events.MetaData;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class WvWFight : FightLogic
    {
        private const string DefaultName = "World vs World";
        private HashSet<int> uniqueTargetIDs = new HashSet<int>();
        public WvWFight(int triggerID) : base(triggerID)
        {
            Extension = "wvw";
            Mode = ParseMode.WvW;
            Icon = "https://wiki.guildwars2.com/images/3/35/WvW_Rank_up.png";
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return uniqueTargetIDs;
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.WorldVersusWorld);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            /*phases.Add(new PhaseData(phases[0].Start + 1, phases[0].End)
            {
                Name = "Detailed Full Fight"
            });
            foreach (Target tar in Targets)
            {
                if (tar != mainTarget)
                {
                    phases[1].Targets.Add(tar);
                }
            }*/
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
                    return new CombatReplayMap("https://i.imgur.com/7DnLZ7G.png", (3100, 3250), (-36864, -36864, 36864, 36864), (-36864, -36864, 36864, 36864), (8958, 12798, 12030, 15870));
                // Green Alpine
                case 95:
                    return new CombatReplayMap("https://i.imgur.com/s4wMYgZ.png", (2492, 3574), (-30720, -43008, 30720, 43008), (-30720, -43008, 30720, 43008), (5630, 11518, 8190, 15102));
                // Blue Alpine
                case 96:
                    return new CombatReplayMap("https://i.imgur.com/s4wMYgZ.png", (2492, 3574), (-30720, -43008, 30720, 43008), (-30720, -43008, 30720, 43008), (12798, 10878, 15358, 14462));
                // Red Desert
                case 1099:
                    return new CombatReplayMap("https://i.imgur.com/IbXfEwV.jpg", (3200, 3200), (-36864, -36864, 36864, 36864), (-36864, -36864, 36864, 36864), (9214, 8958, 12286, 12030));
            }
            return base.GetCombatMapInternal(log);
        }
        internal override string GetLogicName(ParsedLog log)
        {
            MapIDEvent mapID = log.CombatData.GetMapIDEvents().LastOrDefault();
            if (mapID == null)
            {
                return DefaultName;
            }
            switch (mapID.MapID)
            {
                case 38:
                    return "Eternal Battlegrounds";
                case 95:
                    return "Green Alpine Borderlands";
                case 96:
                    return "Blue Alpine Borderlands";
                case 1099:
                    return "Red Desert Borderlands";
                case 899:
                    return "Obsidian Sanctum";
                case 968:
                    return "Edge of the Mists";
            }
            return DefaultName;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<Agent> playerAgents)
        {
            fightData.SetSuccess(true, fightData.FightEnd);
        }

        internal override void EIEvtcParse(FightData fightData, AgentData agentData, List<Combat> combatData, List<Player> playerList)
        {
            Agent dummyAgent = agentData.AddCustomAgent(fightData.FightStart, fightData.FightEnd, Agent.AgentType.NPC, "Enemy Players", "", (int)ArcDPSEnums.TargetID.WorldVersusWorld);
            ComputeFightTargets(agentData, combatData);
            var aList = agentData.GetAgentByType(Agent.AgentType.EnemyPlayer).ToList();
            /*foreach (AgentItem a in aList)
            {
                TrashMobs.Add(new NPC(a));
            }*/
            var enemyPlayerDicts = aList.GroupBy(x => x.AgentValue).ToDictionary(x => x.Key, x => x.ToList().First());
            foreach (Combat c in combatData)
            {
                if (c.IsStateChange == ArcDPSEnums.StateChange.None &&
                    c.IsActivation == ArcDPSEnums.Activation.None &&
                    c.IsBuffRemove == ArcDPSEnums.BuffRemove.None &&
                    ((c.IsBuff != 0 && c.Value == 0) || (c.IsBuff == 0)))
                {
                    if (enemyPlayerDicts.TryGetValue(c.SrcAgent, out Agent src))
                    {
                        c.OverrideSrcAgent(dummyAgent.AgentValue);
                    }
                    if (enemyPlayerDicts.TryGetValue(c.DstAgent, out Agent dst))
                    {
                        c.OverrideDstAgent(dummyAgent.AgentValue);
                    }
                }
            }
        }
    }
}
