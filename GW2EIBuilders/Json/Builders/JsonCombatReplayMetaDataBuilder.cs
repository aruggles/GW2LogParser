using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonCombatReplayMetaDataBuilder
    {
        public static JsonCombatReplayMetaData BuildJsonCombatReplayMetaData(ParsedLog log, RawFormatSettings settings)
        {
            CombatReplayMap map = log.FightData.Logic.GetCombatReplayMap(log);
            (int width, int height) = map.GetPixelMapSize();
            var maps = new List<JsonCombatReplayMetaData.CombatReplayMap>();
            var jsonCR = new JsonCombatReplayMetaData()
            {
                InchToPixel = map.GetInchToPixel(),
                Sizes = new int[2] { width, height },
                PollingRate = ParserHelper.CombatReplayPollingRate,
                Maps = maps
            };
            //
            foreach (CombatReplayMap.MapItem mapItem in map.Maps)
            {
                maps.Add(new JsonCombatReplayMetaData.CombatReplayMap()
                {
                    Url = mapItem.Link,
                    Interval = new long[2] { mapItem.Start, mapItem.End }
                });
            }
            //
            return jsonCR;
        }
    }
}
