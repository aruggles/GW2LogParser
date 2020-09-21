﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.Mechanics;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Data.Events.Buffs;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffApplies;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Events.Status;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Logic
{
    public abstract class FightLogic
    {

        public enum ParseMode { Instanced10, Instanced5, Benchmark, WvW, sPvP, Unknown };

        private CombatReplayMap _map;
        protected List<Mechanic> MechanicList { get; }//Resurrects (start), Resurrect
        public ParseMode Mode { get; protected set; } = ParseMode.Unknown;
        public string Extension { get; protected set; }
        public string Icon { get; protected set; }
        private readonly int _basicMechanicsCount;
        public bool HasNoFightSpecificMechanics => MechanicList.Count == _basicMechanicsCount;
        public List<NPC> TrashMobs { get; } = new List<NPC>();
        public List<NPC> Targets { get; } = new List<NPC>();

        public bool Targetless { get; protected set; } = false;
        protected int GenericTriggerID { get; }

        protected FightLogic(int triggerID)
        {
            GenericTriggerID = triggerID;
            MechanicList = new List<Mechanic>() {
                new PlayerStatusMechanic(Skill.DeathId, "Dead", new MechanicPlotlySetting("x","rgb(0,0,0)"), "Dead",0),
                new PlayerStatusMechanic(Skill.DownId, "Downed", new MechanicPlotlySetting("cross","rgb(255,0,0)"), "Downed",0),
                new PlayerStatusMechanic(Skill.ResurrectId, "Resurrect", new MechanicPlotlySetting("cross-open","rgb(0,255,255)"), "Res",0),
                new PlayerStatusMechanic(Skill.AliveId, "Got up", new MechanicPlotlySetting("cross","rgb(0,255,0)"), "Got up",0),
                new PlayerStatusMechanic(Skill.DCId, "Disconnected", new MechanicPlotlySetting("x","rgb(120,120,120)"), "DC",0),
                new PlayerStatusMechanic(Skill.RespawnId, "Respawn", new MechanicPlotlySetting("cross","rgb(120,120,255)"), "Resp",0)
            };
            _basicMechanicsCount = MechanicList.Count;
        }

        // Only used for CSV files
        public NPC GetLegacyTarget()
        {
            return Targets.Find(x => x.ID == GenericTriggerID);
        }

        public MechanicData GetMechanicData()
        {
            return new MechanicData(MechanicList);
        }

        protected virtual CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("", (800, 800), (0, 0, 0, 0), (0, 0, 0, 0), (0, 0, 0, 0));
        }

        public CombatReplayMap GetCombatMap(ParsedLog log)
        {
            if (_map == null)
            {
                _map = GetCombatMapInternal(log);
                _map.ComputeBoundingBox(log);
            }
            return _map;
        }

        protected virtual List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                GenericTriggerID
            };
        }

        internal virtual string GetLogicName(ParsedLog log)
        {
            NPC target = Targets.Find(x => x.ID == GenericTriggerID);
            if (target == null)
            {
                return "UNKNOWN";
            }
            return target.Character;
        }

        private static void RegroupTargetsByID(int id, AgentData agentData, List<Combat> combatItems)
        {
            List<Agent> agents = agentData.GetNPCsByID(id);
            if (agents.Count > 1)
            {
                Agent firstItem = agents.First();
                var agentValues = new HashSet<ulong>(agents.Select(x => x.AgentValue));
                var newTargetAgent = new Agent(firstItem);
                newTargetAgent.OverrideAwareTimes(agents.Min(x => x.FirstAware), agents.Max(x => x.LastAware));
                agentData.SwapMasters(new HashSet<Agent>(agents), firstItem);
                agentData.OverrideID(id, newTargetAgent);
                foreach (Combat c in combatItems)
                {
                    if (agentValues.Contains(c.SrcAgent) && c.IsStateChange.SrcIsAgent())
                    {
                        c.OverrideSrcAgent(newTargetAgent.AgentValue);
                    }
                    if (agentValues.Contains(c.DstAgent) && c.IsStateChange.DstIsAgent())
                    {
                        c.OverrideDstAgent(newTargetAgent.AgentValue);
                    }
                }
            }
        }

        protected abstract HashSet<int> GetUniqueTargetIDs();

        protected virtual void ComputeFightTargets(AgentData agentData, List<Combat> combatItems)
        {
            foreach (int id in GetUniqueTargetIDs())
            {
                RegroupTargetsByID(id, agentData, combatItems);
            }
            List<int> ids = GetFightTargetsIDs();
            foreach (int id in ids)
            {
                List<Agent> agents = agentData.GetNPCsByID(id);
                foreach (Agent agentItem in agents)
                {
                    Targets.Add(new NPC(agentItem));
                }
            }
            List<ArcDPSEnums.TrashID> ids2 = GetTrashMobsIDS();
            var aList = agentData.GetAgentByType(Agent.AgentType.NPC).Where(x => ids2.Contains(ArcDPSEnums.GetTrashID(x.ID))).ToList();
            //aList.AddRange(agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => ids2.Contains(ParseEnum.GetTrashIDS(x.ID))));
            foreach (Agent a in aList)
            {
                TrashMobs.Add(new NPC(a));
            }
        }

        protected static List<PhaseData> GetPhasesByHealthPercent(ParsedLog log, NPC mainTarget, List<double> thresholds)
        {
            var phases = new List<PhaseData>();
            if (thresholds.Count == 0)
            {
                return phases;
            }
            long fightDuration = log.FightData.FightEnd;
            long start = 0;
            double offset = 100.0 / thresholds.Count;
            List<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem);
            for (int i = 0; i < thresholds.Count; i++)
            {
                HealthUpdateEvent evt = hpUpdates.FirstOrDefault(x => x.HPPercent <= thresholds[i]);
                if (evt == null)
                {
                    break;
                }
                var phase = new PhaseData(start, Math.Min(evt.Time, fightDuration), (offset + thresholds[i]) + "% - " + thresholds[i] + "%");
                phase.Targets.Add(mainTarget);
                phases.Add(phase);
                start = Math.Max(evt.Time, 0);
            }
            if (phases.Count > 0 && phases.Count < thresholds.Count)
            {
                var lastPhase = new PhaseData(start, fightDuration, (offset + thresholds[phases.Count]) + "% -" + thresholds[phases.Count] + "%");
                lastPhase.Targets.Add(mainTarget);
                phases.Add(lastPhase);
            }
            return phases;
        }

        protected static List<PhaseData> GetPhasesByInvul(ParsedLog log, long skillID, NPC mainTarget, bool addSkipPhases, bool beginWithStart)
        {
            long fightDuration = log.FightData.FightEnd;
            var phases = new List<PhaseData>();
            long last = 0;
            List<AbstractBuffEvent> invuls = GetFilteredList(log.CombatData, skillID, mainTarget, beginWithStart);
            invuls.RemoveAll(x => x.Time < 0);
            for (int i = 0; i < invuls.Count; i++)
            {
                AbstractBuffEvent c = invuls[i];
                if (c is BuffApplyEvent)
                {
                    long end = Math.Min(c.Time, fightDuration);
                    phases.Add(new PhaseData(last, end));
                    /*if (i == invuls.Count - 1)
                    {
                        mainTarget.AddCustomCastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None, log);
                    }*/
                    last = end;
                }
                else
                {
                    long end = Math.Min(c.Time, fightDuration);
                    if (addSkipPhases)
                    {
                        phases.Add(new PhaseData(last, end));
                    }
                    //mainTarget.AddCustomCastLog(last, -5, (int)(end - last), ParseEnum.Activation.None, (int)(end - last), ParseEnum.Activation.None, log);
                    last = end;
                }
            }
            if (fightDuration - last > ParserHelper.PhaseTimeLimit)
            {
                phases.Add(new PhaseData(last, fightDuration));
            }
            return phases;
        }

        protected static List<PhaseData> GetInitialPhase(ParsedLog log)
        {
            var phases = new List<PhaseData>();
            long fightDuration = log.FightData.FightEnd;
            phases.Add(new PhaseData(0, fightDuration, "Full Fight")
            {
                CanBeSubPhase = false
            });
            return phases;
        }

        internal List<PhaseData> GetBreakbarPhases(ParsedLog log, bool requirePhases)
        {
            if (!requirePhases)
            {
                return new List<PhaseData>();
            }
            var breakbarPhases = new List<PhaseData>();
            foreach (NPC target in Targets)
            {
                int i = 0;
                List<BreakbarStateEvent> breakbarStateEvents = log.CombatData.GetBreakbarStateEvents(target.AgentItem);
                List<BreakbarPercentEvent> breakbarPercentEvents = log.CombatData.GetBreakbarPercentEvents(target.AgentItem);
                var breakbarActiveEvents = breakbarStateEvents.Where(x => x.State == ArcDPSEnums.BreakbarState.Active).ToList();
                var breakbarNotActiveEvents = breakbarStateEvents.Where(x => x.State != ArcDPSEnums.BreakbarState.Active).ToList();
                foreach (BreakbarStateEvent active in breakbarActiveEvents)
                {
                    long start = Math.Max(active.Time - 2000, 0);
                    BreakbarStateEvent notActive = breakbarNotActiveEvents.FirstOrDefault(x => x.Time >= active.Time);
                    long end = Math.Min(notActive != null ? notActive.Time : log.FightData.FightEnd, log.FightData.FightEnd);
                    var phase = new PhaseData(start, end, target.Character + " Breakbar " + ++i)
                    {
                        BreakbarPhase = true,
                        CanBeSubPhase = false
                    };
                    phase.Targets.Add(target);
                    breakbarPhases.Add(phase);
                }
            }
            return breakbarPhases;
        }

        internal virtual List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            return phases;
        }

        protected void AddTargetsToPhase(PhaseData phase, List<int> ids, ParsedLog log)
        {
            foreach (NPC target in Targets)
            {
                if (ids.Contains(target.ID) && phase.InInterval(Math.Max(target.FirstAware, 0)))
                {
                    phase.Targets.Add(target);
                }
            }
            phase.OverrideTimes(log);
        }

        internal virtual List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<Agent, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            return new List<AbstractBuffEvent>();
        }

        protected static void NegateDamageAgainstBarrier(List<Agent> agentItems, Dictionary<Agent, List<AbstractDamageEvent>> damageByDst)
        {
            var dmgEvts = new List<AbstractDamageEvent>();
            foreach (Agent agentItem in agentItems)
            {
                if (damageByDst.TryGetValue(agentItem, out List<AbstractDamageEvent> list))
                {
                    dmgEvts.AddRange(list);
                }
            }
            foreach (AbstractDamageEvent de in dmgEvts)
            {
                if (de.ShieldDamage > 0)
                {
                    de.NegateShieldDamage();
                }
            }
        }

        internal virtual List<AbstractDamageEvent> SpecialDamageEventProcess(Dictionary<Agent, List<AbstractDamageEvent>> damageBySrc, Dictionary<Agent, List<AbstractDamageEvent>> damageByDst, Dictionary<long, List<AbstractDamageEvent>> damageById, SkillData skillData)
        {
            return new List<AbstractDamageEvent>();
        }

        internal virtual void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
        }

        internal virtual void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
        }

        protected virtual List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>();
        }

        internal virtual FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.NoCM;
        }

        protected void SetSuccessByDeath(CombatData combatData, FightData fightData, HashSet<Agent> playerAgents, bool all, int idFirst, params int[] ids)
        {
            var idsToUse = new List<int>
            {
                idFirst
            };
            idsToUse.AddRange(ids);
            SetSuccessByDeath(combatData, fightData, playerAgents, all, idsToUse);
        }

        protected void SetSuccessByDeath(CombatData combatData, FightData fightData, HashSet<Agent> playerAgents, bool all, List<int> idsToUse)
        {
            int success = 0;
            long maxTime = long.MinValue;
            foreach (int id in idsToUse)
            {
                NPC target = Targets.Find(x => x.ID == id);
                if (target == null)
                {
                    return;
                }
                DeadEvent killed = combatData.GetDeadEvents(target.AgentItem).LastOrDefault();
                if (killed != null)
                {
                    long time = killed.Time;
                    success++;
                    AbstractDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.Damage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTaken != null)
                    {
                        time = Math.Min(lastDamageTaken.Time, time);
                    }
                    maxTime = Math.Max(time, maxTime);
                }
            }
            if ((all && success == idsToUse.Count) || (!all && success > 0))
            {
                fightData.SetSuccess(true, maxTime);
            }
        }

        protected static void SetSuccessByCombatExit(List<NPC> targets, CombatData combatData, FightData fightData, HashSet<Agent> playerAgents)
        {
            if (targets.Count == 0)
            {
                return;
            }
            var playerExits = new List<ExitCombatEvent>();
            var targetExits = new List<ExitCombatEvent>();
            var lastTargetDamages = new List<AbstractDamageEvent>();
            foreach (Agent a in playerAgents)
            {
                playerExits.AddRange(combatData.GetExitCombatEvents(a));
            }
            foreach (NPC t in targets)
            {
                EnterCombatEvent enterCombat = combatData.GetEnterCombatEvents(t.AgentItem).LastOrDefault();
                if (enterCombat != null)
                {
                    targetExits.AddRange(combatData.GetExitCombatEvents(t.AgentItem).Where(x => x.Time > enterCombat.Time));
                }
                else
                {
                    targetExits.AddRange(combatData.GetExitCombatEvents(t.AgentItem));
                }
                AbstractDamageEvent lastDamage = combatData.GetDamageTakenData(t.AgentItem).LastOrDefault(x => (x.Damage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                if (lastDamage != null)
                {
                    lastTargetDamages.Add(lastDamage);
                }
            }
            ExitCombatEvent lastPlayerExit = playerExits.Count > 0 ? playerExits.MaxBy(x => x.Time) : null;
            ExitCombatEvent lastTargetExit = targetExits.Count > 0 ? targetExits.MaxBy(x => x.Time) : null;
            AbstractDamageEvent lastDamageTaken = lastTargetDamages.Count > 0 ? lastTargetDamages.MaxBy(x => x.Time) : null;
            if (lastTargetExit != null && lastDamageTaken != null)
            {
                if (lastPlayerExit != null)
                {
                    fightData.SetSuccess(lastPlayerExit.Time > lastTargetExit.Time + 1000, lastDamageTaken.Time);
                }
                else if (fightData.FightEnd > targets.Max(x => x.LastAware) + 2000)
                {
                    fightData.SetSuccess(true, lastDamageTaken.Time);
                }
            }
        }

        internal virtual void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<Agent> playerAgents)
        {
            SetSuccessByDeath(combatData, fightData, playerAgents, true, GenericTriggerID);
        }

        internal virtual long GetFightOffset(FightData fightData, AgentData agentData, List<Combat> combatData)
        {
            return fightData.FightOffset;
        }

        internal virtual void EIEvtcParse(FightData fightData, AgentData agentData, List<Combat> combatData, List<Player> playerList)
        {
            ComputeFightTargets(agentData, combatData);
        }

        //
        protected static List<AbstractBuffEvent> GetFilteredList(CombatData combatData, long buffID, AbstractSingleActor target, bool beginWithStart)
        {
            bool needStart = beginWithStart;
            var main = combatData.GetBuffData(buffID).Where(x => x.To == target.AgentItem && (x is BuffApplyEvent || x is BuffRemoveAllEvent)).ToList();
            var filtered = new List<AbstractBuffEvent>();
            for (int i = 0; i < main.Count; i++)
            {
                AbstractBuffEvent c = main[i];
                if (needStart && c is BuffApplyEvent)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c is BuffRemoveAllEvent)
                {
                    // consider only last remove event before another application
                    if ((i == main.Count - 1) || (i < main.Count - 1 && main[i + 1] is BuffApplyEvent))
                    {
                        needStart = true;
                        filtered.Add(c);
                    }
                }
            }
            return filtered;
        }

    }
}
