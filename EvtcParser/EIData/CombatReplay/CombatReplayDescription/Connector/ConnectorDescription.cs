using System.Text.Json.Serialization;
using static GW2EIEvtcParser.EIData.AgentFacingAgentConnector;
using static GW2EIEvtcParser.EIData.AgentFacingConnector;
using static GW2EIEvtcParser.EIData.AngleConnector;

namespace GW2EIEvtcParser.EIData;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(SkillConnectorDescription))]
[JsonDerivedType(typeof(AngleConnectorDescription))]
[JsonDerivedType(typeof(SpinningConnectorDescription))]
[JsonDerivedType(typeof(AngleInterpolationConnectorDescription))]
[JsonDerivedType(typeof(AgentFacingAgentConnectorDescription))]
[JsonDerivedType(typeof(AgentFacingConnectorDescription))]
[JsonDerivedType(typeof(PositionConnectorDescription))]
[JsonDerivedType(typeof(InterpolationConnectorDescription))]
[JsonDerivedType(typeof(AgentConnectorDescription))]
[JsonDerivedType(typeof(PositionToAgentConnectorDescription))]
[JsonDerivedType(typeof(ScreenSpaceConnectorDescription))]
public abstract class ConnectorDescription
{
    protected ConnectorDescription(Connector connector, CombatReplayMap map, ParsedEvtcLog log)
    {
    }
}
