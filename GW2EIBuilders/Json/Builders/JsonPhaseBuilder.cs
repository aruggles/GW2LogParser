using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonPhaseBuilder
    {
        public static JsonPhase BuildJsonPhase(PhaseData phase, ParsedLog log)
        {
            var jsPhase = new JsonPhase();
            jsPhase.Start = phase.Start;
            jsPhase.End = phase.End;
            jsPhase.Name = phase.Name;
            var targets = new List<int>();
            jsPhase.BreakbarPhase = phase.BreakbarPhase;
            foreach (AbstractSingleActor tar in phase.Targets)
            {
                targets.Add(log.FightData.Logic.Targets.IndexOf(tar));
            }
            jsPhase.Targets = targets;
            IReadOnlyList<PhaseData> phases = log.FightData.GetNonDummyPhases(log);
            if (!jsPhase.BreakbarPhase)
            {
                var subPhases = new List<int>();
                for (int j = 1; j < phases.Count; j++)
                {
                    PhaseData curPhase = phases[j];
                    if (curPhase.Start < jsPhase.Start || curPhase.End > jsPhase.End ||
                         (curPhase.Start == jsPhase.Start && curPhase.End == jsPhase.End) || !curPhase.CanBeSubPhase)
                    {
                        continue;
                    }
                    subPhases.Add(j);
                }
                if (subPhases.Any())
                {
                    jsPhase.SubPhases = subPhases;
                }
            }
            return jsPhase;
        }
    }
}
