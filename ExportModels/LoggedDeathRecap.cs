using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.ExportModels
{
    public class LoggedDeathRecap
    {
        public long Time { get; internal set; }
        public List<object[]> ToDown { get; internal set; } = null;
        public List<object[]> ToKill { get; internal set; } = null;

        private static List<object[]> BuildDeathRecapItemList(List<DeathRecap.DeathRecapDamageItem> list)
        {
            var data = new List<object[]>();
            foreach (DeathRecap.DeathRecapDamageItem item in list)
            {
                data.Add(new object[]
                {
                            item.Time,
                            item.ID,
                            item.Damage,
                            item.Src,
                            item.IndirectDamage
                });
            }
            return data;
        }
        
        internal static List<LoggedDeathRecap> BuildDeathRecap(ParsedLog log, AbstractSingleActor actor)
        {
            var res = new List<LoggedDeathRecap>();
            IReadOnlyList<DeathRecap> recaps = actor.GetDeathRecaps(log);
            if (!recaps.Any())
            {
                return null;
            }
            foreach (DeathRecap deathRecap in recaps)
            {
                var recap = new LoggedDeathRecap()
                {
                    Time = deathRecap.DeathTime
                };
                res.Add(recap);
                if (deathRecap.ToKill != null)
                {
                    recap.ToKill = BuildDeathRecapItemList(deathRecap.ToKill);
                }
                if (deathRecap.ToDown != null)
                {
                    recap.ToDown = BuildDeathRecapItemList(deathRecap.ToDown);
                }

            }
            return res;
        }
    }
}
