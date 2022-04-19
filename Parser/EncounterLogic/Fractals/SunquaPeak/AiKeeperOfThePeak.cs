﻿using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Gw2LogParser.Parser.Logic.EncounterCategory;

namespace Gw2LogParser.Parser.Logic.Fractals.SunquaPeak
{
    internal class AiKeeperOfThePeak : FractalLogic
    {
        private bool _hasDarkMode = false;
        private bool _hasElementalMode = false;
        public AiKeeperOfThePeak(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                // General
                new HitOnPlayerMechanic(61463, "Elemental Whirl", new MechanicPlotlySetting("square","rgb(255,125,125)"), "Ele.Whrl.","Elemental Whirl", "Elemental Whirl",0),
                // Air
            new HitOnPlayerMechanic(61574, "Elemental Manipulation (Air)", new MechanicPlotlySetting("square","rgb(255,0,255)"), "Air Manip.","Elemental Manipulation (Air)", "Elemental Manipulation (Air)",0),
            new HitOnPlayerMechanic(61534, "Elemental Manipulation (Air)", new MechanicPlotlySetting("square","rgb(255,0,255)"), "Air Manip.","Elemental Manipulation (Air)", "Elemental Manipulation (Air)",0),
            new HitOnPlayerMechanic(61196, "Elemental Manipulation (Air)", new MechanicPlotlySetting("square","rgb(255,0,255)"), "Air Manip.","Elemental Manipulation (Air)", "Elemental Manipulation (Air)",0),
            new HitOnPlayerMechanic(61487, "Fulgor Sphere", new MechanicPlotlySetting("circle","rgb(255,0,255)"), "Flg.Sph.","Fulgor Sphere", "Fulgor Sphere",0),
            new HitOnPlayerMechanic(61565, "Fulgor Sphere", new MechanicPlotlySetting("circle","rgb(255,0,255)"), "Flg.Sph.","Fulgor Sphere", "Fulgor Sphere",0),
            new HitOnPlayerMechanic(61470, "Volatile Wind", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Vlt.Wnd.","Volatile Wind", "Volatile Wind",0),
            new HitOnPlayerMechanic(61205, "Wind Burst", new MechanicPlotlySetting("triangle-down-open","rgb(255,0,255)"), "Wnd.Brst.","Wind Burst", "Wind Burst",0),
            new HitOnPlayerMechanic(61205, "Wind Burst Launch", new MechanicPlotlySetting("triangle-down","rgb(255,0,255)"), "L.Wnd.Burst","Launched up by Wind Burst", "Wind Burst Launch",0,(de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(61190 , "Call of Storms", new MechanicPlotlySetting("triangle-up","rgb(255,0,255)"), "Call Strs","Call of Storms", "Call of Storms",0),
            new EnemyBuffApplyMechanic(61224, "Whirlwind Shield",new MechanicPlotlySetting("asterisk-open","rgb(255,0,255)"), "W.Shield" ,"Whirlwind Shield","Whirlwind Shield",0),
            // Fire
            new HitOnPlayerMechanic(61279, "Elemental Manipulation (Fire)", new MechanicPlotlySetting("square","rgb(255,125,0)"), "Fire Manip.","Elemental Manipulation (Fire)", "Elemental Manipulation (Fire)",0),
            new HitOnPlayerMechanic(61256, "Elemental Manipulation (Fire)", new MechanicPlotlySetting("square","rgb(255,125,0)"), "Fire Manip.","Elemental Manipulation (Fire)", "Elemental Manipulation (Fire)",0),
            new HitOnPlayerMechanic(61271, "Elemental Manipulation (Fire)", new MechanicPlotlySetting("square","rgb(255,125,0)"), "Fire Manip.","Elemental Manipulation (Fire)", "Elemental Manipulation (Fire)",0),
            new HitOnPlayerMechanic(61273, "Roiling Flames", new MechanicPlotlySetting("circle","rgb(255,125,0)"), "Rlng.Flms.","Roiling Flames", "Roiling Flames",0),
            new HitOnPlayerMechanic(61582, "Roiling Flames", new MechanicPlotlySetting("circle","rgb(255,125,0)"), "Rlng.Flms.","Roiling Flames", "Roiling Flames",0),
            new HitOnPlayerMechanic(61548, "Volatile Fire", new MechanicPlotlySetting("triangle-left","rgb(255,125,0)"), "Vlt.Fr.","Volatile Fire", "Volatile Fire",0),
            new SkillOnPlayerMechanic(61348, "Call Meteor", new MechanicPlotlySetting("hexagram","rgb(255,125,0)"), "Meteor.H","Hit by Meteor", "Meteor Hit",1000, (evt, log) => evt.HasDowned || evt.HasKilled),
            new HitOnPlayerMechanic(61248, "Flame Burst", new MechanicPlotlySetting("triangle-down","rgb(255,125,0)"), "Flm.Brst.","Flame Burst", "Flame Burst",0),
            new HitOnPlayerMechanic(61445, "Firestorm", new MechanicPlotlySetting("triangle-up","rgb(255,125,0)"), "Fr.Strm","Firestorm", "Firestorm",0),
            new EnemyCastStartMechanic(61439, "Call Meteor", new MechanicPlotlySetting("asterisk-open","rgb(255,125,0)"), "Smn.Meteor", "Summoned Meteor", "Summon Meteor", 0),
            // Water
            new HitOnPlayerMechanic(61172, "Elemental Manipulation (Water)", new MechanicPlotlySetting("square","rgb(0,125,255)"), "Water Manip.","Elemental Manipulation (Water)", "Elemental Manipulation (Water)",0),
            new HitOnPlayerMechanic(61207, "Elemental Manipulation (Water)", new MechanicPlotlySetting("square","rgb(0,125,255)"), "Water Manip.","Elemental Manipulation (Water)", "Elemental Manipulation (Water)",0),
            new HitOnPlayerMechanic(61556, "Elemental Manipulation (Water)", new MechanicPlotlySetting("square","rgb(0,125,255)"), "Water Manip.","Elemental Manipulation (Water)", "Elemental Manipulation (Water)",0),
            new HitOnPlayerMechanic(61556, "Torrential Bolt", new MechanicPlotlySetting("circle","rgb(0,125,255)"), "Tor.Bolt","Torrential Bolt", "Torrential Bolt",0),
            new HitOnPlayerMechanic(61177, "Torrential Bolt", new MechanicPlotlySetting("circle","rgb(0,125,255)"), "Tor.Bolt","Torrential Bolt", "Torrential Bolt",0),
            new HitOnPlayerMechanic(61419, "Volatile Water", new MechanicPlotlySetting("triangle-left","rgb(0,125,255)"), "Vlt.Wtr.","Volatile Water", "Volatile Water",0),
            new HitOnPlayerMechanic(61251, "Aquatic Burst", new MechanicPlotlySetting("triangle-down","rgb(0,125,255)"), "Aq.Brst.","Aquatic Burst", "Aquatic Burst",0),
            new EnemyBuffApplyMechanic(61402, "Tidal Barrier", new MechanicPlotlySetting("asterisk-open","rgb(0,125,255)"), "Tid.Bar.", "Tidal Barrier", "Tidal Barrier", 0),
            new PlayerBuffRemoveMechanic(61512, "Tidal Bargain", new MechanicPlotlySetting("star-open","rgb(0,125,255)"), "Tdl.Brgn.","Downed by Tidal Bargain", "Tidal Bargain",0, (evt, log) => evt.RemovedStacks == 10 && log.CombatData.GetDownEvents(evt.To).Any(x => Math.Abs(x.Time - evt.Time) < 50)),
            // Dark
            new HitOnPlayerMechanic(61602, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61606, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61604, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61508, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61217, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61529, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61260, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61600, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61527, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61344, "Focused Wrath", new MechanicPlotlySetting("circle","rgb(150,125,255)"), "Fcsd.Wrth.","Focused Wrath", "Focused Wrath",0),
            new HitOnPlayerMechanic(61499, "Focused Wrath", new MechanicPlotlySetting("circle","rgb(150,125,255)"), "Fcsd.Wrth.","Focused Wrath", "Focused Wrath",0),
            new HitOnPlayerMechanic(61289, "Negative Burst", new MechanicPlotlySetting("diamond-wide","rgb(150,125,255)"), "N.Brst.","Negative Burst", "Negative Burst",500),
            new HitOnPlayerMechanic(61184, "Terrorstorm", new MechanicPlotlySetting("diamond-tall","rgb(150,125,255)"), "TrrStrm","Terrorstorm", "Terrorstorm",0),
            new PlayerBuffRemoveMechanic(61208, "Crushing Guilt", new MechanicPlotlySetting("star-open","rgb(150,125,255)"), "Crsh.Glt.","Downed by Crushing Guilt", "Crushing Guilt",0, (evt, log) => evt.RemovedStacks == 10 && log.CombatData.GetDownEvents(evt.To).Any(x => Math.Abs(x.Time - evt.Time) < 100)),
            new EnemyCastStartMechanic(61508, "Empathic Manipulation (Fear)", new MechanicPlotlySetting("triangle-up","rgb(150,125,255)"), "Fear Manip.", "Empathic Manipulation (Fear)", "Empathic Manipulation (Fear)", 0),
            new EnemyCastEndMechanic(61508, "Empathic Manipulation (Fear) Interrupt", new MechanicPlotlySetting("triangle-up-open","rgb(150,125,255)"), "IntFear Manip.", "Empathic Manipulation (Fear) Interrupt", "Empathic Manipulation (Fear) Interrupt", 0, (evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyCastStartMechanic(61606, "Empathic Manipulation (Sorrow)", new MechanicPlotlySetting("triangle-left","rgb(150,125,255)"), "Sor.Manip.", "Empathic Manipulation (Sorrow)", "Empathic Manipulation (Sorrow)", 0),
            new EnemyCastEndMechanic(61606, "Empathic Manipulation (Sorrow) Interrupt", new MechanicPlotlySetting("triangle-left-open","rgb(150,125,255)"), "IntSor.Manip.", "Empathic Manipulation (Sorrow) Interrupt", "Empathic Manipulation (Sorrow) Interrupt", 0, (evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyCastStartMechanic(61602, "Empathic Manipulation (Guilt)", new MechanicPlotlySetting("triangle-right","rgb(150,125,255)"), "Glt.Manip.", "Empathic Manipulation (Guilt)", "Empathic Manipulation (Guilt)", 0),
            new EnemyCastEndMechanic(61602, "Empathic Manipulation (Guilt) Interrupt", new MechanicPlotlySetting("triangle-right-open","rgb(150,125,255)"), "Int.Glt.Manip.", "Empathic Manipulation (Guilt) Interrupt", "Empathic Manipulation (Guilt) Interrupt", 0, (evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyBuffApplyMechanic(61435, "Cacophonous Mind", new MechanicPlotlySetting("pentagon","rgb(150,125,255)"), "Ccphns.Mnd.","Cacophonous Mind", "Cacophonous Mind",0),
            });
            Extension = "ai";
            Icon = "https://i.imgur.com/3mlCdI9.png";
            EncounterCategoryInformation.SubCategory = SubFightCategory.SunquaPeak;
        }

        internal override string GetLogicName(ParsedLog log)
        {
            if (_hasDarkMode && _hasElementalMode)
            {
                return "Ai, Keeper of the Peak";
            }
            else if (_hasDarkMode)
            {
                return "Dark Ai, Keeper of the Peak";
            }
            else
            {
                return "Elemental Ai, Keeper of the Peak";
            }
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/zSBL7YP.png",
                            (823, 1000),
                            (5411, -95, 8413, 3552));
        }

        /*internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            // Tidal Bargain, Cacophonous Mind and Crushing Guilt adjust
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 61512);
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 61208);
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 61435);
            return res;
        }*/

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak,
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2,
                (int)ArcDPSEnums.TrashID.SorrowDemon5,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.FearDemon,
                ArcDPSEnums.TrashID.GuiltDemon,
                ArcDPSEnums.TrashID.EnrageWaterSprite,
                // Transition sorrow demons
                ArcDPSEnums.TrashID.SorrowDemon1,
                ArcDPSEnums.TrashID.SorrowDemon2,
                ArcDPSEnums.TrashID.SorrowDemon3,
                ArcDPSEnums.TrashID.SorrowDemon4,
            };
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak,
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2,
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<Combat> combatData, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            Agent aiAgent = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.AiKeeperOfThePeak).FirstOrDefault();
            if (aiAgent == null)
            {
                throw new MissingKeyActorsException("Ai not found");
            }
            Combat darkModePhaseEvent = combatData.FirstOrDefault(x => x.SkillID == 53569 && x.SrcMatchesAgent(aiAgent));
            _hasDarkMode = combatData.Exists(x => x.SkillID == 61356 && x.SrcMatchesAgent(aiAgent));
            _hasElementalMode = !_hasDarkMode || darkModePhaseEvent != null;
            if (_hasDarkMode)
            {
                if (_hasElementalMode)
                {
                    long darkModeStart = combatData.FirstOrDefault(x => x.SkillID == 61277 && x.Time >= darkModePhaseEvent.Time && x.SrcMatchesAgent(aiAgent)).Time;
                    Combat invul895Loss = combatData.FirstOrDefault(x => x.Time <= darkModeStart && x.SkillID == 895 && x.IsBuffRemove == ArcDPSEnums.BuffRemove.All && x.SrcMatchesAgent(aiAgent));
                    long lastAwareTime = (invul895Loss != null ? invul895Loss.Time : darkModeStart);
                    Agent darkAiAgent = agentData.AddCustomAgent(lastAwareTime + 1, aiAgent.LastAware, Agent.AgentType.NPC, aiAgent.Name, aiAgent.Spec, (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2, false, aiAgent.Toughness, aiAgent.Healing, aiAgent.Condition, aiAgent.Concentration, aiAgent.HitboxWidth, aiAgent.HitboxHeight);
                    // Redirect combat events
                    foreach (Combat evt in combatData)
                    {
                        if (evt.Time >= darkAiAgent.FirstAware && evt.Time <= darkAiAgent.LastAware)
                        {
                            if (evt.SrcMatchesAgent(aiAgent, extensions))
                            {
                                evt.OverrideSrcAgent(darkAiAgent.AgentValue);
                            }
                            if (evt.DstMatchesAgent(aiAgent, extensions))
                            {
                                evt.OverrideDstAgent(darkAiAgent.AgentValue);
                            }
                        }
                    }
                    Combat healthUpdateToCopy = combatData.LastOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && x.SrcMatchesAgent(aiAgent) && x.Time <= lastAwareTime - 1);
                    if (healthUpdateToCopy != null)
                    {
                        //
                        {
                            var elAI0HP = new Combat(healthUpdateToCopy);
                            elAI0HP.OverrideDstAgent(0);
                            elAI0HP.OverrideTime(lastAwareTime);
                            combatData.Add(elAI0HP);
                        }
                        //
                        {
                            var darkAI0HP = new Combat(healthUpdateToCopy);
                            darkAI0HP.OverrideDstAgent(0);
                            darkAI0HP.OverrideTime(darkAiAgent.FirstAware);
                            darkAI0HP.OverrideSrcAgent(darkAiAgent.AgentValue);
                            combatData.Add(darkAI0HP);
                        }
                    }
                    aiAgent.OverrideAwareTimes(aiAgent.FirstAware, lastAwareTime);
                    // Redirect NPC masters
                    foreach (Agent ag in agentData.GetAgentByType(Agent.AgentType.NPC))
                    {
                        if (ag.Master == aiAgent && ag.FirstAware >= aiAgent.LastAware)
                        {
                            ag.SetMaster(darkAiAgent);
                        }
                    }
                    // Redirect Gadget masters
                    foreach (Agent ag in agentData.GetAgentByType(Agent.AgentType.Gadget))
                    {
                        if (ag.Master == aiAgent && ag.FirstAware >= aiAgent.LastAware)
                        {
                            ag.SetMaster(darkAiAgent);
                        }
                    }
                }
                else
                {
                    Extension = "drkai";
                    aiAgent.OverrideID(ArcDPSEnums.TargetID.AiKeeperOfThePeak2);
                    agentData.Refresh();
                }
            }
            else
            {
                Extension = "elai";
            }
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, friendlies, extensions);
            // Manually set HP and names
            AbstractSingleActor eleAi = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak);
            AbstractSingleActor darkAi = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2);
            darkAi?.OverrideName("Dark Ai");
            eleAi?.OverrideName("Elemental Ai");
            if (_hasElementalMode && _hasDarkMode)
            {
                Combat aiMaxHP = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && x.SrcMatchesAgent(aiAgent));
                if (aiMaxHP != null)
                {
                    darkAi.SetManualHealth((int)aiMaxHP.DstAgent);
                }
            }
            if (_hasDarkMode)
            {
                int sorrowCount = 0;
                foreach (AbstractSingleActor target in Targets)
                {
                    if (target.ID == (int)ArcDPSEnums.TrashID.SorrowDemon5)
                    {
                        target.OverrideName(target.Character + " " + (++sorrowCount));
                    }
                }
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.CMnoName;
        }

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor elementalAi = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak);
            if (elementalAi == null)
            {
                if (_hasElementalMode)
                {
                    throw new MissingKeyActorsException("Ai not found");
                }
            }
            else
            {
                phases[0].AddTarget(elementalAi);
            }
            AbstractSingleActor darkAi = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2);
            if (darkAi == null)
            {
                if (_hasDarkMode)
                {
                    throw new MissingKeyActorsException("Ai not found");
                }
            }
            else
            {
                phases[0].AddTarget(darkAi);
            }
            if (!requirePhases)
            {
                return phases;
            }
            if (_hasElementalMode)
            {
                BuffApplyEvent invul895Gain = log.CombatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == elementalAi.AgentItem).FirstOrDefault();
                long eleStart = Math.Max(elementalAi.FirstAware, 0);
                long eleEnd = invul895Gain != null ? invul895Gain.Time : log.FightData.FightEnd;
                if (_hasDarkMode)
                {
                    var elePhase = new PhaseData(eleStart, eleEnd, "Elemental Phase");
                    elePhase.AddTarget(elementalAi);
                    phases.Add(elePhase);
                }
                //
                var invul762Gains = log.CombatData.GetBuffData(762).OfType<BuffApplyEvent>().Where(x => x.To == elementalAi.AgentItem).ToList();
                var invul762Losses = log.CombatData.GetBuffData(762).OfType<BuffRemoveAllEvent>().Where(x => x.To == elementalAi.AgentItem).ToList();
                // sub phases
                string[] eleNames = { "Air", "Fire", "Water" };
                long subStart = eleStart;
                long subEnd = 0;
                for (int i = 0; i < invul762Gains.Count; i++)
                {
                    subEnd = invul762Gains[i].Time;
                    if (i < invul762Losses.Count)
                    {
                        var subPhase = new PhaseData(subStart, subEnd, eleNames[i]);
                        subPhase.AddTarget(elementalAi);
                        phases.Add(subPhase);
                        long invul762Loss = invul762Losses[i].Time;
                        AbstractCastEvent castEvt = elementalAi.GetCastEvents(log, eleStart, eleEnd).FirstOrDefault(x => x.SkillId == 61385 && x.Time >= invul762Loss);
                        if (castEvt == null)
                        {
                            break;
                        }
                        subStart = castEvt.Time;
                    }
                    else
                    {
                        var subPhase = new PhaseData(subStart, subEnd, eleNames[i]);
                        subPhase.AddTarget(elementalAi);
                        phases.Add(subPhase);
                        break;
                    }

                }
            }
            if (_hasDarkMode)
            {
                BuffApplyEvent invul895Gain = log.CombatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == darkAi.AgentItem).FirstOrDefault();
                long darkStart = Math.Max(darkAi.FirstAware, 0);
                long darkEnd = invul895Gain != null ? invul895Gain.Time : log.FightData.FightEnd;
                if (_hasElementalMode)
                {
                    var darkPhase = new PhaseData(darkStart, darkEnd, "Dark Phase");
                    darkPhase.AddTarget(darkAi);
                    phases.Add(darkPhase);
                }
                // sub phases
                AbstractCastEvent fearToSorrow = darkAi.GetCastEvents(log, darkStart, darkEnd).FirstOrDefault(x => x.SkillId == 61606);
                if (fearToSorrow != null)
                {
                    var fearPhase = new PhaseData(darkStart + 1, fearToSorrow.Time, "Fear");
                    fearPhase.AddTarget(darkAi);
                    phases.Add(fearPhase);
                    AbstractCastEvent sorrowToGuilt = darkAi.GetCastEvents(log, darkStart, darkEnd).FirstOrDefault(x => x.SkillId == 61602);
                    if (sorrowToGuilt != null)
                    {
                        var sorrowPhase = new PhaseData(fearToSorrow.Time + 1, sorrowToGuilt.Time, "Sorrow");
                        sorrowPhase.AddTarget(darkAi);
                        phases.Add(sorrowPhase);
                        var guiltPhase = new PhaseData(sorrowToGuilt.Time + 1, darkEnd, "Guilt");
                        guiltPhase.AddTarget(darkAi);
                        phases.Add(guiltPhase);
                    }
                    else
                    {
                        var sorrowPhase = new PhaseData(fearToSorrow.Time + 1, darkEnd, "Sorrow");
                        sorrowPhase.AddTarget(darkAi);
                        phases.Add(sorrowPhase);
                    }
                }
            }
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<Agent> playerAgents)
        {
            int status = 0;
            if (_hasElementalMode)
            {
                status |= 1;
            }
            if (_hasDarkMode)
            {
                status |= 2;
            }
            switch (status)
            {
                case 1:
                case 2:
                    BuffApplyEvent invul895Gain = combatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == Targets[0].AgentItem).FirstOrDefault();
                    if (invul895Gain != null)
                    {
                        fightData.SetSuccess(true, invul895Gain.Time);
                    }
                    break;
                case 3:
                    BuffApplyEvent darkInvul895Gain = combatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == Targets.FirstOrDefault(y => y.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2).AgentItem).FirstOrDefault();
                    if (darkInvul895Gain != null)
                    {
                        fightData.SetSuccess(true, darkInvul895Gain.Time);
                    }
                    break;
                case 0:
                default:
                    throw new MissingKeyActorsException("Ai not found");
            }
        }
    }
}
