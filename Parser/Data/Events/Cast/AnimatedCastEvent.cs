﻿using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Helper;
using System;

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

        private void SetAcceleration(Combat endItem)
        {
            double nonScaledToScaledRatio = 1.0;
            if (_scaledActualDuration > 0)
            {
                nonScaledToScaledRatio = (double)_scaledActualDuration / ActualDuration;
                if (nonScaledToScaledRatio > 1.0)
                {
                    // faster
                    Acceleration = (nonScaledToScaledRatio - 1.0) / 0.5;
                }
                else
                {
                    Acceleration = -(1.0 - nonScaledToScaledRatio) / 0.6;
                }
                Acceleration = Math.Max(Math.Min(Acceleration, 1.0), -1.0);
            }
            if (SkillId != Skill.ResurrectId)
            {
                switch (endItem.IsActivation)
                {
                    case ArcDPSEnums.Activation.CancelCancel:
                        Status = AnimationStatus.Interrupted;
                        SavedDuration = -ActualDuration;
                        break;
                    case ArcDPSEnums.Activation.Reset:
                        Status = AnimationStatus.Full;
                        break;
                    case ArcDPSEnums.Activation.CancelFire:
                        int scaledExpectedDuration = (int)Math.Round(ExpectedDuration / nonScaledToScaledRatio);
                        SavedDuration = Math.Max(scaledExpectedDuration - ActualDuration, 0);
                        Status = AnimationStatus.Reduced;
                        break;
                }
            }
            Acceleration = Math.Round(Acceleration, ParserHelper.AccelerationDigit);
        }

        // Start missing
        internal AnimatedCastEvent(AgentData agentData, SkillData skillData, Combat endItem) : base(endItem, agentData, skillData)
        {
            ActualDuration = endItem.Value;
            ExpectedDuration = ActualDuration;
            _scaledActualDuration = endItem.BuffDmg;
            if (Skill.ID == Skill.DodgeId)
            {
                // dodge animation start item has always 0 as expected duration
                ExpectedDuration = ActualDuration;
                _scaledActualDuration = 0;
            }
            Time -= ActualDuration;
            SetAcceleration(endItem);
        }

        // Start and End both present
        internal AnimatedCastEvent(Combat startItem, AgentData agentData, SkillData skillData, Combat endItem) : this(startItem, agentData, skillData)
        {
            ActualDuration = endItem.Value;
            _scaledActualDuration = endItem.BuffDmg;
            int expectedActualDuration = (int)(endItem.Time - startItem.Time);
            // Sanity check, sometimes the difference is massive
            if (Math.Abs(ActualDuration - expectedActualDuration) > ParserHelper.ServerDelayConstant)
            {
                ActualDuration = expectedActualDuration;
                _scaledActualDuration = 0;
            }
            if (Skill.ID == Skill.DodgeId)
            {
                // dodge animation start item has always 0 as expected duration
                ExpectedDuration = ActualDuration;
                _scaledActualDuration = 0;
            }
            SetAcceleration(endItem);
        }

        // End missing
        internal AnimatedCastEvent(Combat startItem, AgentData agentData, SkillData skillData, long maxEnd) : this(startItem, agentData, skillData)
        {
            if (Skill.ID == Skill.DodgeId)
            {
                // TODO: vindicator dodge duration
                ExpectedDuration = 750;
            }
            ActualDuration = ExpectedDuration;
            CutAt(maxEnd);
        }

        internal void CutAt(long maxEnd)
        {
            if (EndTime > maxEnd && Status == AnimationStatus.Unknown)
            {
                ActualDuration = (int)(maxEnd - Time);
            }
        }
    }
}
