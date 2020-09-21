using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.Statistics
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
            DamageGain = damageGain;
            TotalDamage = totalDamage;
        }
    }
}
