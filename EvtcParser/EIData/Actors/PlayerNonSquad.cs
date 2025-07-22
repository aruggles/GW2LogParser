using GW2EIEvtcParser.ParsedData;
using System.IO;
using System.Security.Principal;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

public class PlayerNonSquad : PlayerActor
{

    private static int NonSquadPlayers = 0;
    // Constructors
    internal PlayerNonSquad(AgentItem agent) : base(agent)
    {
        if (agent.Type == AgentItem.AgentType.Player)
        {
            throw new InvalidDataException("Agent is not a squad Player");
        }
        Character = Spec.ToString() + " pl-" + AgentItem.InstID;
        Account = "Non Squad Player " + (++NonSquadPlayers);
    }
    protected override void TrimCombatReplay(ParsedEvtcLog log, CombatReplay replay)
    {
        // Down, Dead, Alive, Spawn and Despawn events are not reliable
        replay.Trim(FirstAware, LastAware);
    }

}
