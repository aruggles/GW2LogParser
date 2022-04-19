using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Extensions;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal class UnknownFightLogic : FightLogic
    {
        public UnknownFightLogic(int triggerID) : base(triggerID)
        {
            Extension = "boss";
            Icon = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
            EncounterCategoryInformation.Category = FightCategory.UnknownEncounter;
            EncounterCategoryInformation.SubCategory = SubFightCategory.UnknownEncounter;
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>();
        }

        internal override void ComputeFightTargets(AgentData agentData, List<Combat> combatItems, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            int id = GetFightTargetsIDs().First();
            Agent agentItem = agentData.GetNPCsByID(id).FirstOrDefault();
            // Trigger ID is not NPC
            if (agentItem == null)
            {
                agentItem = agentData.GetGadgetsByID(id).FirstOrDefault();
                if (agentItem != null)
                {
                    _targets.Add(new NPC(agentItem));
                }
            }
            else
            {
                _targets.Add(new NPC(agentItem));
            }
            //
            TargetAgents = new HashSet<Agent>(_targets.Select(x => x.AgentItem));
            TrashMobAgents = new HashSet<Agent>(_trashMobs.Select(x => x.AgentItem));
        }
    }
}
