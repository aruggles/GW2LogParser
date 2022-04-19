﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations.Connectors;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    internal class ConjuredAmalgamate : MythwrightGambit
    {
        public ConjuredAmalgamate(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(52173, "Pulverize", new MechanicPlotlySetting("square",Colors.LightOrange), "Arm Slam","Pulverize (Arm Slam)", "Arm Slam",0),
            new HitOnPlayerMechanic(52173, "Pulverize", new MechanicPlotlySetting("square-open",Colors.LightOrange), "Stab.Slam","Pulverize (Arm Slam) while affected by stability", "Stabilized Arm Slam",0,(de, log) => de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(52086, "Junk Absorption", new MechanicPlotlySetting("circle-open",Colors.Purple), "Balls","Junk Absorption (Purple Balls during collect)", "Purple Balls",0),
            new HitOnPlayerMechanic(52878, "Junk Fall", new MechanicPlotlySetting("circle-open",Colors.Pink), "Junk","Junk Fall (Falling Debris)", "Junk Fall",0),
            new HitOnPlayerMechanic(52120, "Junk Fall", new MechanicPlotlySetting("circle-open",Colors.Pink), "Junk","Junk Fall (Falling Debris)", "Junk Fall",0),
            new HitOnPlayerMechanic(52161, "Ruptured Ground", new MechanicPlotlySetting("square-open",Colors.Teal), "Ground","Ruptured Ground (Relics after Junk Wall)", "Ruptured Ground",0, (de,log) => de.HealthDamage > 0),
            new HitOnPlayerMechanic(52656, "Tremor", new MechanicPlotlySetting("circle-open",Colors.Red), "Tremor","Tremor (Field adjacent to Arm Slam)", "Near Arm Slam",0, (de,log) => de.HealthDamage > 0),
            new HitOnPlayerMechanic(52150, "Junk Torrent", new MechanicPlotlySetting("square-open",Colors.Red), "Wall","Junk Torrent (Moving Wall)", "Junk Torrent (Wall)",0, (de,log) => de.HealthDamage > 0),
            new PlayerCastStartMechanic(52325, "Conjured Slash", new MechanicPlotlySetting("square",Colors.Red), "Sword.Cst","Conjured Slash (Special action sword)", "Sword Cast",0),
            new PlayerCastStartMechanic(52780, "Conjured Protection", new MechanicPlotlySetting("square",Colors.Green), "Shield.Cst","Conjured Protection (Special action shield)", "Shield Cast",0),
            new PlayerBuffApplyMechanic(52667, "Greatsword Power", new MechanicPlotlySetting("diamond-tall",Colors.Red), "Sword.C","Collected Sword", "Sword Collect",50),
            new PlayerBuffApplyMechanic(52754, "Conjured Shield", new MechanicPlotlySetting("diamond-tall",Colors.Green), "Shield.C","Collected Shield", "Shield Collect",50),
            new EnemyBuffApplyMechanic(52074, "Augmented Power", new MechanicPlotlySetting("asterisk-open",Colors.Red), "Augmented Power","Augmented Power", "Augmented Power",50),
            new EnemyBuffApplyMechanic(53003, "Shielded", new MechanicPlotlySetting("asterisk-open",Colors.Green), "Shielded","Shielded", "Shielded",50),
            });
            Extension = "ca";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = "https://i.imgur.com/eLyIWd2.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/lgzr1xD.png",
                            (544, 1000),
                            (-5064, -15030, -2864, -10830)/*,
                            (-21504, -21504, 24576, 24576),
                            (13440, 14336, 15360, 16256)*/);
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.ConjuredAmalgamate,
                (int)ArcDPSEnums.TargetID.CARightArm,
                (int)ArcDPSEnums.TargetID.CALeftArm
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.ConjuredGreatsword,
                ArcDPSEnums.TrashID.ConjuredShield
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // make those into npcs
            IReadOnlyList<Agent> cas = agentData.GetGadgetsByID((int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
            IReadOnlyList<Agent> leftArms = agentData.GetGadgetsByID((int)ArcDPSEnums.TargetID.CALeftArm);
            IReadOnlyList<Agent> rightArms = agentData.GetGadgetsByID((int)ArcDPSEnums.TargetID.CARightArm);
            foreach (Agent ca in cas)
            {
                ca.OverrideType(Agent.AgentType.NPC);
            }
            foreach (Agent leftArm in leftArms)
            {
                leftArm.OverrideType(Agent.AgentType.NPC);
            }
            foreach (Agent rightArm in rightArms)
            {
                rightArm.OverrideType(Agent.AgentType.NPC);
            }
            agentData.Refresh();
            ComputeFightTargets(agentData, combatData, extensions);
            Agent sword = agentData.AddCustomAgent(0, fightData.FightEnd, Agent.AgentType.NPC, "Conjured Sword\0:Conjured Sword\051", ParserHelper.Spec.NPC, (int)ArcDPSEnums.TrashID.ConjuredPlayerSword, true);
            friendlies.Add(new NPC(sword));
            foreach (Combat c in combatData)
            {
                if (c.IsDamage(extensions) && c.SkillID == 52370)
                {
                    c.OverrideSrcAgent(sword.AgentValue);
                }
            }
        }
        /*internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            // Greatsword Power
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 52667);
            // Conjured Shield
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 52754);
            return res;
        }*/

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.ConjuredAmalgamate,
                (int)ArcDPSEnums.TargetID.CALeftArm,
                (int)ArcDPSEnums.TargetID.CARightArm
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.ConjuredAmalgamate:
                    List<AbstractBuffEvent> shieldCA = GetFilteredList(log.CombatData, 53003, target, true);
                    int shieldCAStart = 0;
                    foreach (AbstractBuffEvent c in shieldCA)
                    {
                        if (c is BuffApplyEvent)
                        {
                            shieldCAStart = (int)c.Time;
                        }
                        else
                        {
                            int shieldEnd = (int)c.Time;
                            int radius = 500;
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (shieldCAStart, shieldEnd), "rgba(0, 150, 255, 0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.CALeftArm:
                case (int)ArcDPSEnums.TargetID.CARightArm:
                    break;
                case (int)ArcDPSEnums.TrashID.ConjuredGreatsword:
                    break;
                case (int)ArcDPSEnums.TrashID.ConjuredShield:
                    List<AbstractBuffEvent> shield = GetFilteredList(log.CombatData, 53003, target, true);
                    int shieldStart = 0;
                    foreach (AbstractBuffEvent c in shield)
                    {
                        if (c is BuffApplyEvent)
                        {
                            shieldStart = (int)c.Time;
                        }
                        else
                        {
                            int shieldEnd = (int)c.Time;
                            int radius = 100;
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (shieldStart, shieldEnd), "rgba(0, 150, 255, 0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
                AbstractSingleActor leftArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CALeftArm);
                AbstractSingleActor rightArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CARightArm);
                if (target == null)
                {
                    throw new MissingKeyActorsException("Conjured Amalgamate not found");
                }
                Agent zommoros = agentData.GetNPCsByID(21118).LastOrDefault();
                if (zommoros == null)
                {
                    return;
                }
                SpawnEvent npcSpawn = combatData.GetSpawnEvents(zommoros).LastOrDefault();
                AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                if (lastDamageTaken == null)
                {
                    return;
                }
                if (rightArm != null)
                {
                    AbstractHealthDamageEvent lastDamageTakenArm = combatData.GetDamageTakenData(rightArm.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTakenArm != null)
                    {
                        lastDamageTaken = lastDamageTaken.Time > lastDamageTakenArm.Time ? lastDamageTaken : lastDamageTakenArm;
                    }
                }
                if (leftArm != null)
                {
                    AbstractHealthDamageEvent lastDamageTakenArm = combatData.GetDamageTakenData(leftArm.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTakenArm != null)
                    {
                        lastDamageTaken = lastDamageTaken.Time > lastDamageTakenArm.Time ? lastDamageTaken : lastDamageTakenArm;
                    }
                }
                if (npcSpawn != null)
                {
                    fightData.SetSuccess(true, lastDamageTaken.Time);
                }
            }
        }

        private static List<long> GetTargetableTimes(ParsedLog log, AbstractSingleActor target)
        {
            var attackTargetsAgents = log.CombatData.GetAttackTargetEvents(target.AgentItem).Take(2).ToList(); // 3rd one is weird
            var attackTargets = new List<Agent>();
            foreach (AttackTargetEvent c in attackTargetsAgents)
            {
                attackTargets.Add(c.AttackTarget);
            }
            var targetables = new List<long>();
            foreach (Agent attackTarget in attackTargets)
            {
                IReadOnlyList<TargetableEvent> aux = log.CombatData.GetTargetableEvents(attackTarget);
                targetables.AddRange(aux.Where(x => x.Targetable).Select(x => x.Time));
            }
            return targetables;
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor ca = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
            if (ca == null)
            {
                throw new MissingKeyActorsException("Conjured Amalgamate not found");
            }
            phases[0].AddTarget(ca);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 52255, ca, true, false));
            for (int i = 1; i < phases.Count; i++)
            {
                string name;
                PhaseData phase = phases[i];
                if (i % 2 == 1)
                {
                    name = "Arm Phase";
                }
                else
                {
                    name = "Burn Phase";
                    phase.AddTarget(ca);
                }
                phase.Name = name;
            }
            AbstractSingleActor leftArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CALeftArm);
            if (leftArm != null)
            {
                List<long> targetables = GetTargetableTimes(log, leftArm);
                for (int i = 1; i < phases.Count; i += 2)
                {
                    PhaseData phase = phases[i];
                    if (targetables.Exists(x => phase.InInterval(x)))
                    {
                        phase.Name = "Left " + phase.Name;
                        phase.AddTarget(leftArm);
                    }
                }
            }
            AbstractSingleActor rightArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CARightArm);
            if (rightArm != null)
            {
                List<long> targetables = GetTargetableTimes(log, rightArm);
                for (int i = 1; i < phases.Count; i += 2)
                {
                    PhaseData phase = phases[i];
                    if (targetables.Exists(x => phase.InInterval(x)))
                    {
                        if (phase.Name.Contains("Left"))
                        {
                            phase.Name = "Both Arms Phase";
                        }
                        else
                        {
                            phase.Name = "Right " + phase.Name;
                        }
                        phase.AddTarget(rightArm);
                    }
                }
            }
            return phases;
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = p.GetCastEvents(log, 0, log.FightData.FightEnd);
            var shieldCast = cls.Where(x => x.SkillId == 52780).ToList();
            foreach (AbstractCastEvent c in shieldCast)
            {
                int start = (int)c.Time;
                int duration = 10000;
                int radius = 300;
                Point3D shieldNextPos = replay.PolledPositions.FirstOrDefault(x => x.Time >= start);
                Point3D shieldPrevPos = replay.PolledPositions.LastOrDefault(x => x.Time <= start);
                if (shieldNextPos != null || shieldPrevPos != null)
                {
                    replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, start + duration), "rgba(255, 0, 255, 0.1)", new InterpolatedPositionConnector(shieldPrevPos, shieldNextPos, start)));
                    replay.Decorations.Add(new CircleDecoration(false, 0, radius, (start, start + duration), "rgba(255, 0, 255, 0.3)", new InterpolatedPositionConnector(shieldPrevPos, shieldNextPos, start)));
                }
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
            if (target == null)
            {
                throw new MissingKeyActorsException("Conjured Amalgamate not found");
            }
            return combatData.GetBuffData(53075).Count > 0 ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
