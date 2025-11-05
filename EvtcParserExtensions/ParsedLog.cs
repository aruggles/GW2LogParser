using GW2EIEvtcParser;

namespace Gw2LogParser.EvtcParserExtensions
{
    public class ParsedLog : ParsedEvtcLog
    {
        public FileInfo evctFile { get; set; } = null!;

        // int evtcVersion, FightData fightData, AgentData agentData, SkillData skillData,
        // List<CombatItem> combatItems, List<Player> playerList, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, EvtcParserSettings parserSettings, ParserController operation)
        internal ParsedLog(ParsedEvtcLog log) : base(log)
        {
            // No need to assign readonly fields from base class here; base constructor already handles them.
            // Remove assignments to readonly fields to fix CS0191.
        }
    }
}
