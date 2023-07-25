using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;

namespace Gw2LogParser.ExportModels
{
    public class LoggedSkill
    {
        public long Id { get; internal set; }
        public string Name { get; internal set; }
        public string Icon { get; internal set; }
        public bool Aa { get; internal set; }
        public bool IsSwap { get; internal set; }
        public bool NotAccurate { get; internal set; }

        internal static void AssembleSkills(ICollection<SkillItem> skills, Dictionary<string, LoggedSkill> dict, SkillData skillData)
        {
            foreach (SkillItem skill in skills)
            {
                dict["s" + skill.ID] = new LoggedSkill()
                {
                    Id = skill.ID,
                    Name = skill.Name,
                    Icon = skill.Icon,
                    Aa = skill.AA,
                    IsSwap = skill.IsSwap,
                    NotAccurate = skillData.IsNotAccurate(skill.ID)
                };
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

        internal static List<object[]> BuildRotationData(ParsedLog log, AbstractSingleActor p, PhaseData phase, Dictionary<long, SkillItem> usedSkills)
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
