using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;

namespace Gw2LogParser.Parser.Helper.CachingCollections
{
    public class CachingCollectionWithTarget<T> : CachingCollectionCustom<AbstractSingleActor, T>
    {
        private static readonly NPC _nullActor = new NPC(new Agent());

        public CachingCollectionWithTarget(ParsedLog log) : base(log, _nullActor)
        {
        }

    }
}
