﻿using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Statistics
{
    public class FinalGameplayStats
    {
        public int TotalDamageCount { get; internal set; }
        public int DirectDamageCount { get; internal set; }
        public int ConnectedDamageCount { get; internal set; }
        public int ConnectedDirectDamageCount { get; internal set; }
        public int CritableDirectDamageCount { get; internal set; }
        public int CriticalCount { get; internal set; }
        public int CriticalDmg { get; internal set; }
        public int FlankingCount { get; internal set; }
        public int GlanceCount { get; internal set; }
        public int AgainstMovingCount { get; internal set; }
        public int Missed { get; internal set; }
        public int Blocked { get; internal set; }
        public int Evaded { get; internal set; }
        public int Interrupts { get; internal set; }
        public int Invulned { get; internal set; }
        public int Killed { get; internal set; }
        public int Downed { get; internal set; }


        internal FinalGameplayStats(ParsedLog log, long start, long end, AbstractSingleActor actor, AbstractSingleActor target)
        {
            IReadOnlyList<AbstractHealthDamageEvent> dls = actor.GetDamageEvents(target, log, start, end);
            foreach (AbstractHealthDamageEvent dl in dls)
            {
                if (dl.From == actor.AgentItem)
                {
                    if (!(dl is NonDirectHealthDamageEvent))
                    {
                        if (dl.HasHit)
                        {
                            if (Skill.CanCrit(dl.SkillId, log.LogData.GW2Build))
                            {
                                if (dl.HasCrit)
                                {
                                    CriticalCount++;
                                    CriticalDmg += dl.HealthDamage;
                                }
                                CritableDirectDamageCount++;
                            }
                            if (dl.IsFlanking)
                            {
                                FlankingCount++;
                            }

                            if (dl.HasGlanced)
                            {
                                GlanceCount++;
                            }
                            ConnectedDirectDamageCount++;
                        }

                        if (dl.IsBlind)
                        {
                            Missed++;
                        }
                        if (dl.IsEvaded)
                        {
                            Evaded++;
                        }
                        if (dl.IsBlocked)
                        {
                            Blocked++;
                        }
                        if (!dl.DoubleProcHit)
                        {
                            DirectDamageCount++;
                        }
                    }
                    if (dl.IsAbsorbed)
                    {
                        Invulned++;
                    }
                    if (!dl.DoubleProcHit)
                    {
                        TotalDamageCount++;
                    }

                    if (dl.HasHit)
                    {
                        ConnectedDamageCount++;
                        if (dl.AgainstMoving)
                        {
                            AgainstMovingCount++;
                        }
                    }
                }

                if (!(dl is NonDirectHealthDamageEvent))
                {
                    if (dl.HasInterrupted)
                    {
                        Interrupts++;
                    }
                }
                if (dl.HasKilled)
                {
                    Killed++;
                }
                if (dl.HasDowned)
                {
                    Downed++;
                }

            }
        }
    }
}
