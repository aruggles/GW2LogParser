﻿using System;

namespace GW2EIEvtcParser.EIData
{
    public class DamageModifierStat
    {
        public int HitCount { get; }
        public int TotalHitCount { get; }
        public double DamageGain { get; }
        public int TotalDamage { get; }

        public DamageModifierStat(int hitCount, int totalHitCount, double damageGain, int totalDamage)
        {
            HitCount = hitCount;
            TotalHitCount = totalHitCount;
            DamageGain = Math.Round(damageGain, ParserHelper.DamageModGainDigit);
            TotalDamage = totalDamage;
        }
    }
}
