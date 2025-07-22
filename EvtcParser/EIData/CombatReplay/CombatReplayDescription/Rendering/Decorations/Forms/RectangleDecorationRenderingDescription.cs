using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.CombatReplayDescription;
using static GW2EIEvtcParser.EIData.RectangleDecoration;

namespace GW2EIEvtcParser.EIData;

public class RectangleDecorationRenderingDescription : FormDecorationRenderingDescription
{

    internal RectangleDecorationRenderingDescription(ParsedEvtcLog log, RectangleDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.Rectangle;
    }
}
