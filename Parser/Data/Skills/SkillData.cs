using Gw2LogParser.Parser.Data.Events.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Skills
{
    public class SkillData
    {
        // Fields
        private readonly Dictionary<long, Skill> _skills = new Dictionary<long, Skill>();

        // Public Methods

        internal SkillData()
        {

        }

        public Skill Get(long ID)
        {
            if (_skills.TryGetValue(ID, out Skill value))
            {
                return value;
            }
            var item = new Skill(ID, "UNKNOWN");
            Add(item);
            return item;
        }

        internal HashSet<long> NotAccurate = new HashSet<long>();

        public bool IsNotAccurate(long ID)
        {
            return NotAccurate.Contains(ID);
        }

        internal void Add(Skill Skill)
        {
            if (!_skills.ContainsKey(Skill.ID))
            {
                _skills.Add(Skill.ID, Skill);
            }
        }

        internal void CombineWithSkillInfo(Dictionary<long, SkillInfoEvent> skillInfoEvents)
        {
            foreach (KeyValuePair<long, Skill> pair in _skills)
            {
                if (skillInfoEvents.TryGetValue(pair.Key, out SkillInfoEvent skillInfoEvent))
                {
                    pair.Value.AttachSkillInfoEvent(skillInfoEvent);
                }
            }
        }
    }
}
