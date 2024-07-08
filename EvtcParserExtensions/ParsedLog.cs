using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.IO;

namespace Gw2LogParser.EvtcParserExtensions
{
    public class ParsedLog : ParsedEvtcLog
    {
        public FileInfo evctFile { get; set; }
        internal ParsedLog(int evtcVersion, FightData fightData, AgentData agentData, SkillData skillData, 
            List<CombatItem> combatItems, List<Player> playerList, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, EvtcParserSettings parserSettings, ParserController operation) : base(evtcVersion, fightData, agentData, skillData, combatItems, playerList, extensions, parserSettings, operation)
        {
            
        }
        // int evtcVersion, FightData fightData, AgentData agentData, SkillData skillData,
        // List<CombatItem> combatItems, List<Player> playerList, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, EvtcParserSettings parserSettings, ParserController operation)
        internal ParsedLog(ParsedEvtcLog log) : base(log) { }
    }
}
