
namespace Gw2LogParser.Parser.Data.El.InstantCastFinders
{
    internal abstract class BuffCastFinder : InstantCastFinder
    {
        protected long BuffID { get; }
        protected BuffCastFinder(long skillID, long buffID, long icd) : base(skillID, icd)
        {
            BuffID = buffID;
        }

        protected BuffCastFinder(long skillID, long buffID, long icd, ulong minBuild, ulong maxBuild) : base(skillID, icd, minBuild, maxBuild)
        {
            BuffID = buffID;
        }
    }
}
