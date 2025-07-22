using System;
using static GW2EIEvtcParser.EIData.CircleDecoration;
using static GW2EIEvtcParser.EIData.CombatReplayDescription;

namespace GW2EIEvtcParser.EIData;

public class CircleDecorationMetadataDescription : FormDecorationMetadataDescription
{
    public readonly uint Radius;
    public readonly uint MinRadius;

    internal CircleDecorationMetadataDescription(CircleDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.Circle;
        Radius = decoration.Radius;
        MinRadius = decoration.MinRadius;
    }
}
