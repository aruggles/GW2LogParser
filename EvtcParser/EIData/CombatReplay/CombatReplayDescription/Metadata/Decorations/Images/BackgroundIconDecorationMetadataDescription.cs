using System;
using static GW2EIEvtcParser.EIData.BackgroundIconDecoration;
using static GW2EIEvtcParser.EIData.CombatReplayDescription;

namespace GW2EIEvtcParser.EIData;

public class BackgroundIconDecorationMetadataDescription : ImageDecorationMetadataDescription
{

    internal BackgroundIconDecorationMetadataDescription(BackgroundIconDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.BackgroundIcon;
    }
}
