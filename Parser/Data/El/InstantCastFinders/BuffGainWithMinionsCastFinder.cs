using Gw2LogParser.Parser.Data.Agents;

namespace Gw2LogParser.Parser.Data.El.InstantCastFinders
{
    internal class BuffGainWithMinionsCastFinder : BuffGainCastFinder
    {
        protected override Agent GetCasterAgent(Agent agent)
        {
            return agent.GetFinalMaster();
        }

        public BuffGainWithMinionsCastFinder(long skillID, long buffID, long icd, BuffGainCastChecker checker = null) : base(skillID, buffID, icd, checker)
        {
        }

        public BuffGainWithMinionsCastFinder(long skillID, long buffID, long icd, ulong minBuild, ulong maxBuild, BuffGainCastChecker checker = null) : base(skillID, buffID, icd, minBuild, maxBuild, checker)
        {
        }
    }
}
