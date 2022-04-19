﻿using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Helper;
using System;
using static Gw2LogParser.Parser.Data.El.Buffs.Buff;

namespace Gw2LogParser.Parser.Data.El.Statistics
{
    public class FinalToPlayersSupport
    {
        //public long allHeal;
        public int Resurrects { get; internal set; }
        public double ResurrectTime { get; internal set; }
        public int CondiCleanse { get; internal set; }
        public double CondiCleanseTime { get; internal set; }
        public int CondiCleanseSelf { get; internal set; }
        public double CondiCleanseTimeSelf { get; internal set; }
        public int BoonStrips { get; internal set; }
        public double BoonStripsTime { get; internal set; }

        internal FinalToPlayersSupport(ParsedLog log, AbstractSingleActor actor, long start, long end)
        {
            FinalSupportAll totals = actor.GetSupportStats(log, start, end);
            Resurrects = totals.Resurrects;
            ResurrectTime = Math.Round(totals.ResurrectTime / 1000.0, ParserHelper.TimeDigit);
            FinalSupport self = actor.GetSupportStats(actor, log, start, end);
            foreach (Buff boon in log.Buffs.BuffsByNature[BuffNature.Boon])
            {
                // add everything from total
                if (totals.Removals.TryGetValue(boon.ID, out (int count, long time) item))
                {
                    BoonStrips += item.count;
                    BoonStripsTime += item.time;
                }
                // remove everything from self
                if (self.Removals.TryGetValue(boon.ID, out item))
                {
                    BoonStrips -= item.count;
                    BoonStripsTime -= item.time;
                }
            }
            foreach (Buff condition in log.Buffs.BuffsByNature[BuffNature.Condition])
            {
                // add everything from self
                if (self.Removals.TryGetValue(condition.ID, out (int count, long time) item))
                {
                    CondiCleanseSelf += item.count;
                    CondiCleanseTimeSelf += item.time;
                }
                foreach (Player p in log.PlayerList)
                {
                    if (p == actor)
                    {
                        continue;
                    }
                    FinalSupport other = actor.GetSupportStats(p, log, start, end);
                    // Add everything from other
                    if (other.Removals.TryGetValue(condition.ID, out item))
                    {
                        CondiCleanse += item.count;
                        CondiCleanseTime += item.time;
                    }
                }
            }
            CondiCleanseTime = Math.Round(CondiCleanseTime / 1000.0, ParserHelper.TimeDigit);
            CondiCleanseTimeSelf = Math.Round(CondiCleanseTimeSelf / 1000.0, ParserHelper.TimeDigit);
            BoonStripsTime = Math.Round(BoonStripsTime / 1000.0, ParserHelper.TimeDigit);
        }
    }
}
