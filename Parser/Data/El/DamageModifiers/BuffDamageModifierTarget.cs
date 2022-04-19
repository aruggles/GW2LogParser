﻿using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Data.El.DamageModifiers.BuffTrackers;
using Gw2LogParser.Parser.Data.El.DamageModifiers.GainComputers;
using Gw2LogParser.Parser.Data.Events;
using Gw2LogParser.Parser.Data.Events.Damage;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Helper.ParserHelper;

namespace Gw2LogParser.Parser.Data.El.DamageModifiers
{
    public class BuffDamageModifierTarget : BuffDamageModifier
    {
        private readonly BuffsTracker _trackerPlayer = null;
        private readonly GainComputer _gainComputerPlayer = null;

        protected double ComputeGainPlayer(int stack, AbstractHealthDamageEvent dl, ParsedLog log)
        {
            if (DLChecker != null && !DLChecker(dl, log))
            {
                return -1.0;
            }
            double gain = _gainComputerPlayer.ComputeGain(1.0, stack);
            return gain > 0.0 ? 1.0 : -1.0;
        }

        internal BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode, dlChecker)
        {
        }

        internal BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, mode, dlChecker)
        {
        }

        internal BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode, dlChecker)
        {
        }

        internal BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, mode, dlChecker)
        {
        }

        internal BuffDamageModifierTarget(long id, long playerId, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : this(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerSingle(playerId);
            _gainComputerPlayer = gainComputerPlayer;
        }

        internal BuffDamageModifierTarget(long id, long[] playerIds, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : this(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, mode, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerMulti(new List<long>(playerIds));
            _gainComputerPlayer = gainComputerPlayer;
        }

        internal BuffDamageModifierTarget(long[] ids, long playerId, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : this(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerSingle(playerId);
            _gainComputerPlayer = gainComputerPlayer;
        }

        internal BuffDamageModifierTarget(long[] ids, long[] playerIds, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : this(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, mode, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerMulti(new List<long>(playerIds));
            _gainComputerPlayer = gainComputerPlayer;
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedLog log)
        {
            Dictionary<long, BuffsGraphModel> bgmsP = actor.GetBuffGraphs(log);
            if (_trackerPlayer != null)
            {
                if (!_trackerPlayer.Has(bgmsP) && _gainComputerPlayer != ByAbsence)
                {
                    return new List<DamageModifierEvent>();
                }
            }
            var res = new List<DamageModifierEvent>();
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = GetHitDamageEvents(actor, log, null, 0, log.FightData.FightEnd);
            if (_trackerPlayer != null)
            {
                foreach (AbstractHealthDamageEvent evt in typeHits)
                {
                    AbstractSingleActor target = log.FindActor(evt.To);
                    Dictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
                    double gain = ComputeGainPlayer(_trackerPlayer.GetStack(bgmsP, evt.Time), evt, log) < 0.0 ? -1.0 : ComputeGain(Tracker.GetStack(bgms, evt.Time), evt, log);
                    res.Add(new DamageModifierEvent(evt, this, gain));
                }
            }
            else
            {
                foreach (AbstractHealthDamageEvent evt in typeHits)
                {
                    AbstractSingleActor target = log.FindActor(evt.To);
                    Dictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
                    res.Add(new DamageModifierEvent(evt, this, ComputeGain(Tracker.GetStack(bgms, evt.Time), evt, log)));
                }
            }
            res.RemoveAll(x => x.DamageGain == -1.0);
            return res;
        }
    }
}
