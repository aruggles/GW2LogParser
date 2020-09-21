﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events.Cast
{
    public class AnimatedCastEvent : AbstractCastEvent
    {
        private readonly int _scaledActualDuration;
        //private readonly int _effectHappenedDuration;

        private AnimatedCastEvent(Combat startItem, AgentData agentData, SkillData skillData) : base(startItem, agentData, skillData)
        {
            ExpectedDuration = startItem.BuffDmg > 0 ? startItem.BuffDmg : startItem.Value;
            if (startItem.IsActivation == ArcDPSEnums.Activation.Quickness)
            {
                Acceleration = 1;
            }
            //_effectHappenedDuration = startItem.Value;
        }

        internal AnimatedCastEvent(Combat startItem, Combat endItem, AgentData agentData, SkillData skillData) : this(startItem, agentData, skillData)
        {
            ActualDuration = endItem.Value;
            _scaledActualDuration = endItem.BuffDmg;
            if (Skill.ID == Skill.DodgeId)
            {
                ExpectedDuration = 750;
                ActualDuration = 750;
                _scaledActualDuration = 0;
            }
            double nonScaledToScaledRatio = 1.0;
            if (_scaledActualDuration > 0)
            {
                nonScaledToScaledRatio = (double)_scaledActualDuration / ActualDuration;
                if (nonScaledToScaledRatio > 1.0)
                {
                    // faster
                    Acceleration = (nonScaledToScaledRatio - 1.0) / 0.66;
                }
                else
                {
                    Acceleration = -(1.0 - nonScaledToScaledRatio) / 0.5;
                }
                Acceleration = Math.Max(Math.Min(Acceleration, 1.0), -1.0);
            }
            switch (endItem.IsActivation)
            {
                case ArcDPSEnums.Activation.CancelCancel:
                    Status = AnimationStatus.Iterrupted;
                    SavedDuration = -ActualDuration;
                    break;
                case ArcDPSEnums.Activation.Reset:
                    Status = AnimationStatus.Full;
                    break;
                case ArcDPSEnums.Activation.CancelFire:
                    int nonScaledExpectedDuration = (int)Math.Round(ExpectedDuration / nonScaledToScaledRatio);
                    SavedDuration = Math.Max(nonScaledExpectedDuration - ActualDuration, 0);
                    Status = AnimationStatus.Reduced;
                    break;
            }
        }

        internal AnimatedCastEvent(Combat startItem, AgentData agentData, SkillData skillData, long logEnd) : this(startItem, agentData, skillData)
        {
            if (Skill.ID == Skill.DodgeId)
            {
                ExpectedDuration = 750;
            }
            ActualDuration = ExpectedDuration;
            if (ActualDuration + Time > logEnd)
            {
                ActualDuration = (int)(logEnd - Time);
            }
        }
    }
}
