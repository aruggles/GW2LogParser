using GW2EIGW2API;
using Gw2LogParser.Parser.Data.Events.MetaData;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.Skills
{
    public class SkillData
    {
        // Fields
        private readonly Dictionary<long, Skill> _skills = new Dictionary<long, Skill>();
        private readonly GW2APIController _apiController;

        // Public Methods

        internal SkillData(GW2APIController apiController)
        {
            _apiController = apiController;
        }

        public Skill Get(long ID)
        {
            if (_skills.TryGetValue(ID, out Skill value))
            {
                return value;
            }
            Add(ID, Skill.DefaultName);
            return _skills[ID];
        }

        internal HashSet<long> NotAccurate = new HashSet<long>();

        public bool IsNotAccurate(long ID)
        {
            return NotAccurate.Contains(ID);
        }

        internal void Add(long id, string name)
        {
            if (!_skills.ContainsKey(id))
            {
                _skills.Add(id, new Skill(id, name, _apiController));
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
