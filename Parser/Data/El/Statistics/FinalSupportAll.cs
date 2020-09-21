using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.Statistics
{
    public class FinalSupportAll : FinalSupport
    {
        //public long allHeal;
        public int Resurrects { get; internal set; }
        public long ResurrectTime { get; internal set; }

        private static long[] GetReses(ParsedLog log, AbstractSingleActor actor, long start, long end)
        {
            List<AbstractCastEvent> cls = actor.GetCastLogs(log, start, end);
            long[] reses = { 0, 0 };
            foreach (AbstractCastEvent cl in cls)
            {
                if (cl.SkillId == Skill.ResurrectId)
                {
                    reses[0]++;
                    reses[1] += cl.ActualDuration;
                }
            }
            return reses;
        }

        internal FinalSupportAll(ParsedLog log, PhaseData phase, AbstractSingleActor actor) : base(log, phase, actor, null)
        {
            long[] resArray = GetReses(log, actor, phase.Start, phase.End);
            Resurrects = (int)resArray[0];
            ResurrectTime = resArray[1];
        }
    }
}
