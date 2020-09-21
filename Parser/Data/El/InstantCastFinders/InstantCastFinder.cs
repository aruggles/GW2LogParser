using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events.Cast;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Interfaces;
using System.Collections.Generic;


namespace Gw2LogParser.Parser.Data.El.InstantCastFinders
{
    internal abstract class InstantCastFinder : IVersionable
    {
        public const long DefaultICD = 50;
        public long SkillID { get; }

        public bool NotAccurate { get; protected set; } = false;

        protected long ICD { get; }

        private ulong _maxBuild { get; } = ulong.MaxValue;
        private ulong _minBuild { get; } = ulong.MinValue;

        protected InstantCastFinder(long skillID, long icd)
        {
            SkillID = skillID;
            ICD = icd;
        }

        protected InstantCastFinder(long skillID, long icd, ulong minBuild, ulong maxBuild) : this(skillID, icd)
        {
            _maxBuild = maxBuild;
            _minBuild = minBuild;
        }


        public bool Available(ulong gw2Build)
        {
            return gw2Build < _maxBuild && gw2Build >= _minBuild;
        }

        public abstract List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData);
    }
}
