using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.El.Statistics
{
    public class FinalGameplayStats
    {
        public int TotalDamageCount { get; internal set; }
        public int DirectDamageCount { get; internal set; }
        public int ConnectedDirectDamageCount { get; internal set; }
        public int CritableDirectDamageCount { get; internal set; }
        public int CriticalCount { get; internal set; }
        public int CriticalDmg { get; internal set; }
        public int FlankingCount { get; internal set; }
        public int GlanceCount { get; internal set; }
        public int Missed { get; internal set; }
        public int Blocked { get; internal set; }
        public int Evaded { get; internal set; }
        public int Interrupts { get; internal set; }
        public int Invulned { get; internal set; }


        internal FinalGameplayStats(ParsedLog log, PhaseData phase, AbstractSingleActor actor, AbstractSingleActor target)
        {
            List<AbstractDamageEvent> dls = actor.GetJustActorDamageLogs(target, log, phase.Start, phase.End);
            foreach (AbstractDamageEvent dl in dls)
            {
                if (!(dl is NonDirectDamageEvent))
                {
                    if (dl.HasHit)
                    {
                        if (Skill.CanCrit(dl.SkillId, log.LogData.GW2Build))
                        {
                            if (dl.HasCrit)
                            {
                                CriticalCount++;
                                CriticalDmg += dl.Damage;
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

                    if (dl.HasInterrupted)
                    {
                        Interrupts++;
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
            }
        }
    }
}
