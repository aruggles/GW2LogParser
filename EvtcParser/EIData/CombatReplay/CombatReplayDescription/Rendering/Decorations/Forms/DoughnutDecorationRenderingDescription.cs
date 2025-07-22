using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.CombatReplayDescription;
using static GW2EIEvtcParser.EIData.DoughnutDecoration;

namespace GW2EIEvtcParser.EIData;

public class DoughnutDecorationRenderingDescription : FormDecorationRenderingDescription
{

    internal DoughnutDecorationRenderingDescription(ParsedEvtcLog log, DoughnutDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.Doughnut;
    }

}
