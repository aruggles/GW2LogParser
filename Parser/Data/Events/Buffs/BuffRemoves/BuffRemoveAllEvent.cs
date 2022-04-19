﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Simulator;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffStacks;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves
{
    public class BuffRemoveAllEvent : AbstractBuffRemoveEvent
    {
        public const int FullRemoval = int.MaxValue;

        public int RemovedStacks { get; }
        private readonly int _lastRemovedDuration;
        private bool _lowValueRemove => (RemovedDuration <= ParserHelper.BuffSimulatorDelayConstant && RemovedDuration != 0 && _lastRemovedDuration <= ParserHelper.BuffSimulatorDelayConstant && _lastRemovedDuration != 0);

        internal BuffRemoveAllEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            _lastRemovedDuration = evtcItem.BuffDmg;
            RemovedStacks = evtcItem.Result;
        }

        internal BuffRemoveAllEvent(Agent by, Agent to, long time, int removedDuration, Skill buffSkill, int removedStacks, int lastRemovedDuration) : base(by, to, time, removedDuration, buffSkill)
        {
            _lastRemovedDuration = lastRemovedDuration;
            RemovedStacks = removedStacks;
        }

        internal override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            if (BuffID == Buff.NoBuff || Time > fightEnd - ParserHelper.BuffSimulatorDelayConstant)
            {
                // don't take into account removal that are close to the end of the fight
                return false;
            }
            if (hasStackIDs)
            {
                return true;
            }
            // low value all stack remove that can mess up with the simulator if server delay
            return !_lowValueRemove;
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Remove(CreditedBy, RemovedDuration, RemovedStacks, Time, ArcDPSEnums.BuffRemove.All, 0);
        }
    }
}
