using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.CircleDecoration;
using static GW2EIEvtcParser.EIData.CombatReplayDescription;

namespace GW2EIEvtcParser.EIData;

public class CircleDecorationRenderingDescription : FormDecorationRenderingDescription
{

    internal CircleDecorationRenderingDescription(ParsedEvtcLog log, CircleDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.Circle;
    }
}
