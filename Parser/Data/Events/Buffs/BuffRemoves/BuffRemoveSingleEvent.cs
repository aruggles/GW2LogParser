﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.Professions;
using Gw2LogParser.Parser.Data.El.Simulator;
using Gw2LogParser.Parser.Data.Events.Buffs.BuffStacks;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;

namespace Gw2LogParser.Parser.Data.Events.Buffs.BuffRemoves
{
    public class BuffRemoveSingleEvent : AbstractBuffRemoveEvent
    {
        private readonly ArcDPSEnums.IFF _iff;
        public uint BuffInstance { get; protected set; }
        internal BuffRemoveSingleEvent(Combat evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            _iff = evtcItem.IFF;
            BuffInstance = evtcItem.Pad;
        }

        internal BuffRemoveSingleEvent(Agent by, Agent to, long time, int removedDuration, Skill buffSkill, uint id, ArcDPSEnums.IFF iff) : base(by, to, time, removedDuration, buffSkill)
        {
            _iff = iff;
            BuffInstance = id;
        }

        internal override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != Buff.NoBuff &&
                    (hasStackIDs ||
                        (!(_iff == ArcDPSEnums.IFF.Unknown && By == ParserHelper._unknownAgent && !hasStackIDs) && // overstack or natural end removals
                        !(RemovedDuration <= 50 && RemovedDuration != 0 && !hasStackIDs) &&// low value single stack remove that can mess up with the simulator if server delay
                        Time <= fightEnd - 50)); // don't take into account removal that are close to the end of the fight));

        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Remove(By, RemovedDuration, 1, Time, ArcDPSEnums.BuffRemove.Single, BuffInstance);
        }
        internal override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffRemoveSingleEvent)
            {
                return 0;
            }
            if (abe is BuffRemoveAllEvent || abe is AbstractBuffStackEvent)
            {
                return -1;
            }
            return 1;
        }
    }
}
