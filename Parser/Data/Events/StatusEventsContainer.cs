using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Events.Status;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Data.Events
{
    public class StatusEventsContainer
    {
        public Dictionary<Agent, List<AliveEvent>> AliveEvents { get; } = new Dictionary<Agent, List<AliveEvent>>();
        public Dictionary<Agent, List<AttackTargetEvent>> AttackTargetEvents { get; } = new Dictionary<Agent, List<AttackTargetEvent>>();
        public Dictionary<Agent, List<DeadEvent>> DeadEvents { get; } = new Dictionary<Agent, List<DeadEvent>>();
        public Dictionary<Agent, List<DespawnEvent>> DespawnEvents { get; } = new Dictionary<Agent, List<DespawnEvent>>();
        public Dictionary<Agent, List<DownEvent>> DownEvents { get; } = new Dictionary<Agent, List<DownEvent>>();
        public Dictionary<Agent, List<EnterCombatEvent>> EnterCombatEvents { get; } = new Dictionary<Agent, List<EnterCombatEvent>>();
        public Dictionary<Agent, List<ExitCombatEvent>> ExitCombatEvents { get; } = new Dictionary<Agent, List<ExitCombatEvent>>();
        public Dictionary<Agent, List<HealthUpdateEvent>> HealthUpdateEvents { get; } = new Dictionary<Agent, List<HealthUpdateEvent>>();
        public Dictionary<Agent, List<MaxHealthUpdateEvent>> MaxHealthUpdateEvents { get; } = new Dictionary<Agent, List<MaxHealthUpdateEvent>>();
        public Dictionary<Agent, List<SpawnEvent>> SpawnEvents { get; } = new Dictionary<Agent, List<SpawnEvent>>();
        public Dictionary<Agent, List<TargetableEvent>> TargetableEvents { get; } = new Dictionary<Agent, List<TargetableEvent>>();
        public Dictionary<Agent, List<TeamChangeEvent>> TeamChangeEvents { get; } = new Dictionary<Agent, List<TeamChangeEvent>>();
        public Dictionary<Agent, List<BreakbarStateEvent>> BreakbarStateEvents { get; } = new Dictionary<Agent, List<BreakbarStateEvent>>();
        public Dictionary<Agent, List<BreakbarPercentEvent>> BreakbarPercentEvents { get; } = new Dictionary<Agent, List<BreakbarPercentEvent>>();

    }
}
