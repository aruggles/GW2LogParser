using GW2EIEvtcParser;

namespace Gw2LogParser.EvtcParserExtensions
{
    public class ParsedLog : ParsedEvtcLog
    {
        public FileInfo evctFile { get; set; }

        // int evtcVersion, FightData fightData, AgentData agentData, SkillData skillData,
        // List<CombatItem> combatItems, List<Player> playerList, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, EvtcParserSettings parserSettings, ParserController operation)
        internal ParsedLog(ParsedEvtcLog log) : base(log) { }
    }
}
