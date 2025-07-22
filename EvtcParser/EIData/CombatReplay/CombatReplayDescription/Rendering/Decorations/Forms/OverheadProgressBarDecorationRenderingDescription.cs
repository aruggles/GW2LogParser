using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.CombatReplayDescription;
using static GW2EIEvtcParser.EIData.OverheadProgressBarDecoration;

namespace GW2EIEvtcParser.EIData;

public class OverheadProgressBarDecorationRenderingDescription : ProgressBarDecorationRenderingDescription
{

    internal OverheadProgressBarDecorationRenderingDescription(ParsedEvtcLog log, OverheadProgressBarDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.ProgressBarOverhead;
    }
}
