using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Skills;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal class SkillDto : AbstractSkillDto
    {
        public bool Aa { get; set; }
        public bool IsSwap { get; set; }
        public bool NotAccurate { get; set; }

        public SkillDto(Skill skill, ParsedLog log) : base(skill, log)
        {
            Aa = skill.AA;
            IsSwap = skill.IsSwap;
            NotAccurate = log.SkillData.IsNotAccurate(skill.ID);
        }

        public static void AssembleSkills(ICollection<Skill> skills, Dictionary<string, SkillDto> dict, ParsedLog log)
        {
            foreach (Skill skill in skills)
            {
                dict["s" + skill.ID] = new SkillDto(skill, log);
            }
        }

        private static object[] GetSkillData(AbstractCastEvent cl, long phaseStart)
        {
            object[] rotEntry = new object[5];
            double start = (cl.Time - phaseStart) / 1000.0;
            rotEntry[0] = start;
            rotEntry[1] = cl.SkillId;
            rotEntry[2] = cl.ActualDuration;
            rotEntry[3] = (int)cl.Status;
            rotEntry[4] = cl.Acceleration;
            return rotEntry;
        }

        public static List<object[]> BuildRotationData(ParsedLog log, AbstractSingleActor p, PhaseData phase, Dictionary<long, Skill> usedSkills)
        {
            var list = new List<object[]>();
            IReadOnlyList<AbstractCastEvent> casting = p.GetIntersectingCastEvents(log, phase.Start, phase.End);
            foreach (AbstractCastEvent cl in casting)
            {
                if (!usedSkills.ContainsKey(cl.SkillId))
                {
                    usedSkills.Add(cl.SkillId, cl.Skill);
                }

                list.Add(GetSkillData(cl, phase.Start));
            }
            return list;
        }
    }
}
