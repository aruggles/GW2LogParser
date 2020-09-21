using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.Agents
{
    public class AgentData
    {
        private readonly List<Agent> _allAgentsList;
        private Dictionary<ulong, Agent> _allAgentsByAgent;
        private Dictionary<ushort, List<Agent>> _allAgentsByInstID;
        private Dictionary<int, List<Agent>> _allNPCsByID;
        private Dictionary<int, List<Agent>> _allGadgetsByID;
        private Dictionary<Agent.AgentType, List<Agent>> _allAgentsByType;
        private Dictionary<string, List<Agent>> _allAgentsByName;
        public HashSet<ulong> AgentValues => new HashSet<ulong>(_allAgentsList.Select(x => x.AgentValue));
        public HashSet<ushort> InstIDValues => new HashSet<ushort>(_allAgentsList.Select(x => x.InstID));

        internal AgentData(List<Agent> allAgentsList)
        {
            _allAgentsList = allAgentsList;
            Refresh();
        }

        internal Agent AddCustomAgent(long start, long end, Agent.AgentType type, string name, string prof, int ID, uint toughness = 0, uint healing = 0, uint condition = 0, uint concentration = 0, uint hitboxWidth = 0, uint hitboxHeight = 0)
        {
            var rnd = new Random();
            ulong agentValue = 0;
            while (AgentValues.Contains(agentValue) || agentValue == 0)
            {
                agentValue = (ulong)rnd.Next(int.MaxValue / 2, int.MaxValue);
            }
            ushort instID = 0;
            while (InstIDValues.Contains(instID) || instID == 0)
            {
                instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
            }
            var agent = new Agent(agentValue, name, prof, ID, instID, type, toughness, healing, condition, concentration, hitboxWidth, hitboxHeight, start, end);
            _allAgentsList.Add(agent);
            Refresh();
            return agent;
        }

        public Agent GetAgent(ulong agentAddress)
        {
            if (agentAddress != 0)
            {
                if (_allAgentsByAgent.TryGetValue(agentAddress, out Agent a))
                {
                    return a;
                }
            }
            return ParserHelper._unknownAgent;
        }

        public List<Agent> GetNPCsByID(int id)
        {
            if (id != 0)
            {
                if (_allNPCsByID.TryGetValue(id, out List<Agent> list))
                {
                    return list;
                }
            }
            return new List<Agent>();
        }

        public List<Agent> GetGadgetsByID(int id)
        {
            if (id != 0)
            {
                if (_allGadgetsByID.TryGetValue(id, out List<Agent> list))
                {
                    return list;
                }
            }
            return new List<Agent>();
        }

        public Agent GetAgentByInstID(ushort instid, long logTime)
        {
            if (instid != 0)
            {
                if (_allAgentsByInstID.TryGetValue(instid, out List<Agent> list))
                {
                    Agent a = list.FirstOrDefault(x => x.FirstAware <= logTime && x.LastAware >= logTime);
                    if (a != null)
                    {
                        return a;
                    }
                    return ParserHelper._unknownAgent;
                }
            }
            return ParserHelper._unknownAgent;
        }

        internal void OverrideID(int ID, Agent Agent)
        {
            _allAgentsList.RemoveAll(x => x.ID == ID);
            _allAgentsList.Add(Agent);
            Refresh();
        }

        internal void Refresh()
        {
            _allAgentsByAgent = _allAgentsList.GroupBy(x => x.AgentValue).ToDictionary(x => x.Key, x => x.ToList().First());
            _allNPCsByID = _allAgentsList.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.Where(y => y.Type == Agent.AgentType.NPC).ToList());
            _allGadgetsByID = _allAgentsList.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.Where(y => y.Type == Agent.AgentType.Gadget).ToList());
            _allAgentsByInstID = _allAgentsList.GroupBy(x => x.InstID).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByType = _allAgentsList.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByName = _allAgentsList.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList());
        }

        public List<Agent> GetAgentByType(Agent.AgentType type)
        {
            if (_allAgentsByType.TryGetValue(type, out List<Agent> list))
            {
                return list;
            }
            else
            {
                return new List<Agent>();
            }
        }

        internal void SwapMasters(HashSet<Agent> froms, Agent to)
        {
            foreach (Agent a in GetAgentByType(Agent.AgentType.NPC))
            {
                if (a.Master != null && froms.Contains(a.Master))
                {
                    a.SetMaster(to);
                }
            }
        }

        internal void SwapMasters(Agent from, Agent to)
        {
            foreach (Agent a in GetAgentByType(Agent.AgentType.NPC))
            {
                if (a.Master != null && a.Master == from)
                {
                    a.SetMaster(to);
                }
            }
        }
    }
}
