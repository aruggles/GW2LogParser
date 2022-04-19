﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic
{
    internal class Golem : FightLogic
    {
        public Golem(int id) : base(id)
        {
            Mode = ParseMode.Benchmark;
            switch (ArcDPSEnums.GetTargetID(id))
            {
                case ArcDPSEnums.TargetID.MassiveGolem10M:
                    Extension = "MassiveGolem10M";
                    Icon = "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                    break;
                case ArcDPSEnums.TargetID.MassiveGolem4M:
                    Extension = "MassiveGolem4M";
                    Icon = "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                    break;
                case ArcDPSEnums.TargetID.MassiveGolem1M:
                    Extension = "MassiveGolem1M";
                    Icon = "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                    break;
                case ArcDPSEnums.TargetID.VitalGolem:
                    Extension = "VitalGolem";
                    Icon = "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                    break;
                case ArcDPSEnums.TargetID.AvgGolem:
                    Extension = "AvgGolem";
                    Icon = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    break;
                case ArcDPSEnums.TargetID.StdGolem:
                    Extension = "StdGolem";
                    Icon = "https://wiki.guildwars2.com/images/8/8f/Mini_Professor_Mew.png";
                    break;
                case ArcDPSEnums.TargetID.ConditionGolem:
                    Extension = "ToughGolem";
                    Icon = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    break;
                case ArcDPSEnums.TargetID.PowerGolem:
                    Extension = "ResGolem";
                    Icon = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    break;
                case ArcDPSEnums.TargetID.LGolem:
                    Extension = "LGolem";
                    Icon = "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                    break;
                case ArcDPSEnums.TargetID.MedGolem:
                    Extension = "MedGolem";
                    Icon = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    break;
            }
            EncounterCategoryInformation.Category = FightCategory.Golem;
            EncounterCategoryInformation.SubCategory = SubFightCategory.Golem;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/gmnSuz7.png",
                            (895, 629),
                            (18115.12, -13978.016, 22590.12, -10833.016));
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<Combat> combatData)
        {
            Combat pov = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.PointOfView);
            if (pov != null)
            {
                // to make sure that the logging starts when the PoV starts attacking (in case there is a slave with them)
                Combat enterCombat = combatData.FirstOrDefault(x => x.SrcAgent == pov.SrcAgent && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat);
                if (enterCombat != null)
                {
                    return enterCombat.Time;
                }
            }
            return fightData.LogStart;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            Agent target = agentData.GetNPCsByID(GenericTriggerID).FirstOrDefault();
            foreach (Combat c in combatData)
            {
                // redirect all attacks to the main golem
                if (c.DstAgent == 0 && c.DstInstid == 0 && c.IsDamage(extensions))
                {
                    c.OverrideDstAgent(target.AgentValue);
                }
            }
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Golem not found");
            }
            phases[0].Name = "Final Number";
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem);
            if (hpUpdates.Count > 0)
            {
                long fightDuration = log.FightData.FightEnd;
                var thresholds = new List<double> { 80, 60, 40, 20, 0 };
                string[] numberNames = new string[] { "First Number", "Second Number", "Third Number", "Fourth Number" };
                // Fifth number would the equivalent of full fight phase
                for (int j = 0; j < thresholds.Count - 1; j++)
                {
                    HealthUpdateEvent hpUpdate = hpUpdates.FirstOrDefault(x => x.HPPercent <= thresholds[j]);
                    if (hpUpdate != null)
                    {
                        var phase = new PhaseData(0, hpUpdate.Time, numberNames[j])
                        {
                            CanBeSubPhase = false
                        };
                        phase.AddTarget(mainTarget);
                        phases.Add(phase);
                    }
                }
                phases.AddRange(GetPhasesByHealthPercent(log, mainTarget, thresholds));
            }

            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Golem not found");
            }
            long fightEndLogTime = fightData.FightEnd;
            bool success = false;
            DeadEvent deadEvt = combatData.GetDeadEvents(mainTarget.AgentItem).LastOrDefault();
            if (deadEvt != null)
            {
                fightEndLogTime = deadEvt.Time;
                success = true;
            }
            else
            {
                AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => x.HealthDamage > 0);
                if (lastDamageTaken != null)
                {
                    fightEndLogTime = lastDamageTaken.Time;
                }
                IReadOnlyList<HealthUpdateEvent> hpUpdates = combatData.GetHealthUpdateEvents(mainTarget.AgentItem);
                if (hpUpdates.Count > 0)
                {
                    success = hpUpdates.Last().HPPercent < 2.00;
                }
            }
            fightData.SetSuccess(success, fightEndLogTime);
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }
    }
}
