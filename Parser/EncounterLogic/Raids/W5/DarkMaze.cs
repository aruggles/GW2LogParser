﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class DarkMaze : HallOfChains
    {
        // TODO - add CR icons and some mechanics
        public DarkMaze(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(791, "Fear", new MechanicPlotlySetting("star-square",Colors.Black), "Feared","Feared by Eye Teleport Skill", "Feared",0),
            new PlayerBuffApplyMechanic(48779, "Light Carrier", new MechanicPlotlySetting("circle-open",Colors.Yellow), "Light Orb","Light Carrier (picked up a light orb)", "Picked up orb",0),
            new PlayerCastStartMechanic(47074, "Flare", new MechanicPlotlySetting("circle",Colors.Green), "Detonate","Flare (detonate light orb to incapacitate eye)", "Detonate orb",0),
            new HitOnPlayerMechanic(47518, "Piercing Shadow", new MechanicPlotlySetting("hexagram-open",Colors.Blue), "Spin","Piercing Shadow (damaging spin to all players in sight)", "Eye Spin",0),
            new HitOnPlayerMechanic(48150, "Deep Abyss", new MechanicPlotlySetting("triangle-right-open",Colors.Red), "Beam","Deep Abyss (ticking eye beam)", "Eye Beam",0),
            //47857 <- teleport + fear skill? 
            }
            );
            Extension = "eyes";
            Icon = "https://wiki.guildwars2.com/images/thumb/a/a7/Eye_of_Fate.jpg/188px-Eye_of_Fate.jpg";
            EncounterCategoryInformation.InSubCategoryOrder = 2;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/ZMCqeQd.png",
                            (809, 1000),
                            (11664, -2108, 16724, 4152)/*,
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508)*/);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.LightThieves,
                ArcDPSEnums.TrashID.MazeMinotaur,
            };
        }


        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.EyeOfFate,
                (int)ArcDPSEnums.TargetID.EyeOfJudgement
            };
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.EyeOfFate,
                (int)ArcDPSEnums.TargetID.EyeOfJudgement
            };
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor eye1 = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.EyeOfFate);
            AbstractSingleActor eye2 = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.EyeOfJudgement);
            if (eye2 == null || eye1 == null)
            {
                throw new MissingKeyActorsException("Eyes not found");
            }
            phases[0].AddTarget(eye2);
            phases[0].AddTarget(eye1);
            return phases;
        }

        private void HPCheck(CombatData combatData, FightData fightData)
        {
            AbstractSingleActor eye1 = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.EyeOfFate);
            AbstractSingleActor eye2 = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.EyeOfJudgement);
            if (eye2 == null || eye1 == null)
            {
                throw new MissingKeyActorsException("Eyes not found");
            }
            IReadOnlyList<HealthUpdateEvent> eye1HPs = combatData.GetHealthUpdateEvents(eye1.AgentItem);
            IReadOnlyList<HealthUpdateEvent> eye2HPs = combatData.GetHealthUpdateEvents(eye2.AgentItem);
            if (eye1HPs.Count == 0 || eye2HPs.Count == 0)
            {
                return;
            }
            double lastEye1Hp = eye1HPs.LastOrDefault().HPPercent;
            double lastEye2Hp = eye2HPs.LastOrDefault().HPPercent;
            double margin1 = Math.Min(0.80, lastEye1Hp);
            double margin2 = Math.Min(0.80, lastEye2Hp);
            if (lastEye1Hp <= margin1 && lastEye2Hp <= margin2)
            {
                int lastIEye1;
                for (lastIEye1 = eye1HPs.Count - 1; lastIEye1 >= 0; lastIEye1--)
                {
                    if (eye1HPs[lastIEye1].HPPercent > margin1)
                    {
                        lastIEye1++;
                        break;
                    }
                }
                int lastIEye2;
                for (lastIEye2 = eye2HPs.Count - 1; lastIEye2 >= 0; lastIEye2--)
                {
                    if (eye2HPs[lastIEye2].HPPercent > margin2)
                    {
                        lastIEye2++;
                        break;
                    }
                }
                fightData.SetSuccess(true, Math.Max(eye1HPs[lastIEye1].Time, eye2HPs[lastIEye2].Time));
            }
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            // First check using hp, best
            HPCheck(combatData, fightData);
            // hp could be unreliable or missing, fall back (around 200 ms more)
            if (!fightData.Success)
            {
                SetSuccessByDeath(combatData, fightData, playerAgents, false, (int)ArcDPSEnums.TargetID.EyeOfFate, (int)ArcDPSEnums.TargetID.EyeOfJudgement);
            }
        }

        internal override string GetLogicName(ParsedLog log)
        {
            return "Statue of Darkness";
        }
    }
}
