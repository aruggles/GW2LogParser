using Gw2LogParser.Exceptions;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers;
using Gw2LogParser.Parser.Data.El.Mechanics;
using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Data.Skills;
using Gw2LogParser.Parser.Extensions;
using Gw2LogParser.Parser.Helper;
using Gw2LogParser.Parser.Logic;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gw2LogParser.Parser.Data
{
    public class ParsedLog
    {
        public LogData LogData { get; }
        public FightData FightData { get; }
        public AgentData AgentData { get; }
        public SkillData SkillData { get; }
        public CombatData CombatData { get; }
        public IReadOnlyList<Player> PlayerList { get; }
        public IReadOnlyList<AbstractSingleActor> Friendlies { get; }
        public IReadOnlyCollection<Agent> PlayerAgents { get; }
        public IReadOnlyCollection<Agent> FriendlyAgents { get; }
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Benchmark;
        public IReadOnlyDictionary<ParserHelper.Spec, List<AbstractSingleActor>> FriendliesListBySpec { get; }
        public DamageModifiersContainer DamageModifiers { get; }
        public BuffsContainer Buffs { get; }
        public ParserSettings ParserSettings { get; }
        public bool CanCombatReplay => ParserSettings.ParseCombatReplay && CombatData.HasMovementData;

        public MechanicData MechanicData { get; }
        public StatisticsHelper StatisticsHelper { get; }

        private readonly ParserController _operation;

        private Dictionary<Agent, AbstractSingleActor> _agentToActorDictionary;
        public FileInfo evctFile { get; set; }

        public ParsedLog(int evtcVersion, FightData fightData, AgentData agentData, SkillData skillData,
                List<Combat> combatItems, List<Player> playerList, List<AbstractSingleActor> friendlies, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, long evtcLogDuration, ParserSettings parserSettings, ParserController operation)
        {
            FightData = fightData;
            AgentData = agentData;
            SkillData = skillData;
            PlayerList = playerList;
            ParserSettings = parserSettings;
            _operation = operation;
            Friendlies = friendlies;
            //
            FriendliesListBySpec = friendlies.GroupBy(x => x.Spec).ToDictionary(x => x.Key, x => x.ToList());
            PlayerAgents = new HashSet<Agent>(playerList.Select(x => x.AgentItem));
            FriendlyAgents = new HashSet<Agent>(friendlies.Select(x => x.AgentItem));
            _operation.UpdateProgressWithCancellationCheck("Creating GW2EI Combat Events");
            CombatData = new CombatData(combatItems, FightData, AgentData, SkillData, playerList, operation, extensions, evtcVersion);
            //
            _operation.UpdateProgressWithCancellationCheck("Checking Success");
            FightData.Logic.CheckSuccess(CombatData, AgentData, FightData, PlayerAgents);
            if (FightData.FightDuration <= ParserSettings.TooShortLimit)
            {
                throw new TooShortException(FightData.FightDuration, ParserSettings.TooShortLimit);
            }
            if (ParserSettings.SkipFailedTries && !FightData.Success)
            {
                throw new SkipException();
            }
            _operation.UpdateProgressWithCancellationCheck("Creating GW2EI Log Meta Data");
            LogData = new LogData(evtcVersion, CombatData, evtcLogDuration, playerList, extensions, operation);
            //
            _operation.UpdateProgressWithCancellationCheck("Creating Buff Container");
            Buffs = new BuffsContainer(LogData.GW2Build, CombatData, operation);
            _operation.UpdateProgressWithCancellationCheck("Creating Damage Modifier Container");
            DamageModifiers = new DamageModifiersContainer(LogData.GW2Build, fightData.Logic.Mode, parserSettings);
            _operation.UpdateProgressWithCancellationCheck("Creating Mechanic Data");
            MechanicData = FightData.Logic.GetMechanicData();
            _operation.UpdateProgressWithCancellationCheck("Creating General Statistics Container");
            StatisticsHelper = new StatisticsHelper(CombatData, PlayerList, Buffs);
        }

        public void UpdateProgressWithCancellationCheck(string status)
        {
            _operation.UpdateProgressWithCancellationCheck(status);
        }

        private void AddToDictionary(AbstractSingleActor actor)
        {
            _agentToActorDictionary[actor.AgentItem] = actor;
            /*foreach (Minions minions in actor.GetMinions(this).Values)
            {
                foreach (NPC npc in minions.MinionList)
                {
                    AddToDictionary(npc);
                }
            }*/
        }

        private void InitActorDictionaries()
        {
            if (_agentToActorDictionary == null)
            {
                _operation.UpdateProgressWithCancellationCheck("Initializing Actor dictionary");
                _agentToActorDictionary = new Dictionary<Agent, AbstractSingleActor>();
                foreach (AbstractSingleActor p in Friendlies)
                {
                    AddToDictionary(p);
                }
                foreach (AbstractSingleActor npc in FightData.Logic.Targets)
                {
                    AddToDictionary(npc);
                }

                foreach (NPC npc in FightData.Logic.TrashMobs)
                {
                    AddToDictionary(npc);
                }
            }
        }

        /// <summary>
        /// Find the corresponding actor, creates one otherwise
        /// </summary>
        /// <param name="agentItem"><see cref="AgentItem"/> to find an <see cref="AbstractSingleActor"/> for</param>
        /// <param name="excludePlayers">returns null if true and agentItem is a player or has a player master</param>
        /// <returns></returns>
        public AbstractSingleActor FindActor(Agent agentItem, bool excludePlayers = false)
        {
            if (agentItem == null || (excludePlayers && agentItem.GetFinalMaster().Type == Agent.AgentType.Player))
            {
                return null;
            }
            InitActorDictionaries();
            if (!_agentToActorDictionary.TryGetValue(agentItem, out AbstractSingleActor actor))
            {
                if (agentItem.Type == Agent.AgentType.NonSquadPlayer)
                {
                    actor = new PlayerNonSquad(agentItem);
                }
                else
                {
                    actor = new NPC(agentItem);
                }
                _agentToActorDictionary[agentItem] = actor;
                //throw new EIException("Requested actor with id " + a.ID + " and name " + a.Name + " is missing");
            }
            return actor;
        }
    }
}
