﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.Events;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class TwistedCastle : StrongholdOfTheFaithful
    {
        public TwistedCastle(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerBuffApplyMechanic(34918, "Spatial Distortion", new MechanicPlotlySetting("circle",Colors.Magenta), "Statue TP", "Teleported by Statue", "Statue Teleport", 500),
                new PlayerBuffApplyMechanic(35106, "Still Waters", new MechanicPlotlySetting("diamond-tall",Colors.Magenta), "Still Waters (Immunity)", "Used a fountain for immunity", "Still Waters (Immunity)", 0, (evt, log) => log.CombatData.GetBuffData(34955).Any(x => x is BuffApplyEvent ba && ba.To == evt.To && Math.Abs(ba.Time - evt.Time) < 500)),
                new PlayerBuffApplyMechanic(35106, "Still Waters", new MechanicPlotlySetting("diamond-tall",Colors.Magenta), "Still Waters (Removal)", "Used a fountain for stack removal", "Still Waters (Removal)", 0, (evt, log) => !log.CombatData.GetBuffData(34955).Any(x => x is BuffApplyEvent ba && ba.To == evt.To && Math.Abs(ba.Time - evt.Time) < 500)),
                new PlayerBuffApplyMechanic(35006, "Madness", new MechanicPlotlySetting("square",Colors.LightPurple), "Madness", "Stacking debuff", "Madness", 0),
                new PlayerBuffApplyMechanic(34963, "Chaotic Haze", new MechanicPlotlySetting("hexagon",Colors.Red), "Chaotic Haze", "Damaging Debuff from bombardement", "Chaotic Haze", 500),
            }
            );
            Extension = "twstcstl";
            GenericFallBackMethod = FallBackMethod.None;
            Targetless = true;
            Icon = "https://i.imgur.com/xpQnu35.png";
            EncounterCategoryInformation.InSubCategoryOrder = 2;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/txFtRNN.png",
                            (774, 1000),
                            (-8058, -4321, 819, 7143)/*,
                            (-12288, -27648, 12288, 27648),
                            (1920, 12160, 2944, 14464)*/);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            RewardEvent reward = combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == 60685);
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            agentData.AddCustomAgent(0, fightData.FightEnd, Agent.AgentType.NPC, "Twisted Castle", ParserHelper.Spec.NPC, (int)ArcDPSEnums.TargetID.DummyTarget, true);
            ComputeFightTargets(agentData, combatData, extensions);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
               ArcDPSEnums.TrashID.HauntingStatue,
               //ParseEnum.TrashIDS.CastleFountain
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC npc, ParsedLog log, CombatReplay replay)
        {
            switch (npc.ID)
            {
                case (int)ArcDPSEnums.TrashID.HauntingStatue:
                    var lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
                    if (replay.Rotations.Any())
                    {
                        replay.Decorations.Add(new FacingDecoration(lifespan, new AgentConnector(npc), replay.PolledRotations));
                    }
                    break;
                //case (ushort)ParseEnum.TrashIDS.CastleFountain:
                //    break;
                default:
                    break;
            }
        }

        internal override string GetLogicName(ParsedLog log)
        {
            return "Twisted Castle";
        }
    }
}
