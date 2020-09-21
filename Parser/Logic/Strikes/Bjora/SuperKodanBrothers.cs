﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class SuperKodanBrothers : StrikeMissionLogic
    {
        public SuperKodanBrothers(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerBuffApplyMechanic(727, "Trapped", new MechanicPlotlySetting("circle","rgb(0,0,255)"), "Trapped","Trapped", "Trapped",2500),
                new EnemyBuffApplyMechanic(58619, "Enrage", new MechanicPlotlySetting("circle","rgb(255,125,0)"), "Enrage","Enrage", "Enrage",1 << 16),
            }
            );
            Extension = "supkodbros";
            Icon = "https://i.imgur.com/lNXXbnC.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/kLjZ7eU.png",
                            (905, 789),
                            (-1013, -1600, 2221, 1416),
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0));
        }

        protected override void SetSuccessByDeath(CombatData combatData, FightData fightData, HashSet<Agent> playerAgents, bool all)
        {
            SetSuccessByDeath(combatData, fightData, playerAgents, all, (int)ArcDPSEnums.TargetID.ClawOfTheFallen, (int)ArcDPSEnums.TargetID.VoiceOfTheFallen);
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC voice = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.ClawOfTheFallen);
            NPC claw = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.VoiceOfTheFallen);
            if (voice == null || claw == null)
            {
                throw new InvalidOperationException("Claw or Voice not found");
            }
            phases[0].Targets.Add(voice);
            phases[0].Targets.Add(claw);
            var fightEnd = log.FightData.FightEnd;
            if (!requirePhases)
            {
                return phases;
            }
            //
            List<PhaseData> unmergedPhases = GetPhasesByInvul(log, 762, claw, false, true);
            for (int i = 0; i < unmergedPhases.Count; i++)
            {
                unmergedPhases[i].Name = "Phase " + (i + 1);
                unmergedPhases[i].Targets.Add(claw);
                unmergedPhases[i].Targets.Add(voice);
            }
            phases.AddRange(unmergedPhases);
            //
            int voiceAndClawCount = 0;
            var offset = 1;
            foreach (NPC voiceAndClaw in Targets.Where(x => x.ID == (int)ArcDPSEnums.TargetID.VoiceAndClaw))
            {
                EnterCombatEvent enterCombat = log.CombatData.GetEnterCombatEvents(voiceAndClaw.AgentItem).FirstOrDefault();
                PhaseData nextUnmergedPhase = unmergedPhases.Count > offset + 1 ? unmergedPhases[offset] : null;
                if (enterCombat != null)
                {
                    var phase = new PhaseData(enterCombat.Time, nextUnmergedPhase != null ? nextUnmergedPhase.Start : Math.Min(fightEnd, voiceAndClaw.LastAware), "Voice and Claw " + ++voiceAndClawCount);
                    phase.Targets.Add(voiceAndClaw);
                    phases.Add(phase);
                    offset++;
                }
            }
            //
            AbstractBuffEvent enrage = log.CombatData.GetBuffData(58619).FirstOrDefault(x => x is BuffApplyEvent);
            if (enrage != null)
            {
                var phase = new PhaseData(enrage.Time, log.FightData.FightEnd, "Enrage");
                phase.Targets.Add(claw.AgentItem == enrage.To ? claw : voice);
                phases.Add(phase);
            }
            return phases;
        }

        internal override string GetLogicName(ParsedLog log)
        {
            return "Super Kodan Brothers";
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.ClawOfTheFallen,
                (int)ArcDPSEnums.TargetID.VoiceOfTheFallen,
            };
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.VoiceOfTheFallen,
                (int)ArcDPSEnums.TargetID.ClawOfTheFallen,
                (int)ArcDPSEnums.TargetID.VoiceAndClaw,
            };
        }
    }
}
