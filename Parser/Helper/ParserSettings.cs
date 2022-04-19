using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Helper
{
    public class ParserSettings
    {
        internal bool AnonymousPlayer { get; }
        internal bool SkipFailedTries { get; }
        internal bool ParsePhases { get; }
        internal bool ParseCombatReplay { get; }
        internal bool ComputeDamageModifiers { get; }
        internal long TooShortLimit { get; }
        internal bool DetailedWvWParse { get; }

        public ParserSettings(bool anonymousPlayer, bool skipFailedTries, bool parsePhases, bool parseCombatReplay, bool computeDamageModifiers, long tooShortLimit, bool detailedWvW)
        {
            AnonymousPlayer = anonymousPlayer;
            SkipFailedTries = skipFailedTries;
            ParsePhases = parsePhases;
            ParseCombatReplay = parseCombatReplay;
            ComputeDamageModifiers = computeDamageModifiers;
            TooShortLimit = Math.Max(tooShortLimit, ParserHelper.MinimumInCombatDuration);
            DetailedWvWParse = detailedWvW;
        }
    }
}
