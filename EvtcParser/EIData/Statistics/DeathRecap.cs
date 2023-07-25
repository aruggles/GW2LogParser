﻿using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class DeathRecap
    {
        public class DeathRecapDamageItem
        {
            public long ID { get; internal set; }
            public bool IndirectDamage { get; internal set; }
            public string Src { get; internal set; }
            public int Damage { get; internal set; }
            public int Time { get; internal set; }
        }

        public long DeathTime { get; }
        public List<DeathRecapDamageItem> ToDown { get; }
        public List<DeathRecapDamageItem> ToKill { get; }

        internal DeathRecap(ParsedEvtcLog log, IReadOnlyList<AbstractHealthDamageEvent> damageLogs, DeadEvent dead, IReadOnlyList<DownEvent> downs, IReadOnlyList<AliveEvent> ups, long lastDeathTime)
        {
            DeathTime = dead.Time;
            DownEvent downed;
            AliveEvent upped = ups.LastOrDefault(x => x.Time <= dead.Time && x.Time >= lastDeathTime);
            if (upped != null)
            {
                downed = downs.LastOrDefault(x => x.Time <= dead.Time && x.Time >= upped.Time);
            }
            else
            {
                downed = downs.LastOrDefault(x => x.Time <= dead.Time && x.Time >= lastDeathTime);
            }
            if (downed != null)
            {
                var damageToDown = damageLogs.Where(x => x.Time > lastDeathTime && x.Time <= downed.Time && (x.HasHit ||x.HasDowned)).ToList();
                ToDown = damageToDown.Count > 0 ? new List<DeathRecapDamageItem>() : null;
                int damage = 0;
                for (int i = damageToDown.Count - 1; i >= 0; i--)
                {
                    AbstractHealthDamageEvent dl = damageToDown[i];
                    AgentItem ag = dl.From;
                    var item = new DeathRecapDamageItem()
                    {
                        Time = (int)dl.Time,
                        IndirectDamage = dl is NonDirectHealthDamageEvent,
                        ID = dl.SkillId,
                        Damage = dl.HealthDamage,
                        Src = log.FindActor(ag)?.Character
                    };
                    damage += dl.HealthDamage;
                    ToDown.Add(item);
                    if (damage > 20000)
                    {
                        break;
                    }
                }
                var damageToKill = damageLogs.Where(x => x.Time > downed.Time && x.Time <= dead.Time && (x.HasHit || x.HasKilled)).ToList();
                ToKill = damageToKill.Count > 0 ? new List<DeathRecapDamageItem>() : null;
                for (int i = damageToKill.Count - 1; i >= 0; i--)
                {
                    AbstractHealthDamageEvent dl = damageToKill[i];
                    AgentItem ag = dl.From;
                    var item = new DeathRecapDamageItem()
                    {
                        Time = (int)dl.Time,
                        IndirectDamage = dl is NonDirectHealthDamageEvent,
                        ID = dl.SkillId,
                        Damage = dl.HealthDamage,
                        Src = log.FindActor(ag)?.Character
                    };
                    ToKill.Add(item);
                }
            }
            else
            {
                ToDown = null;
                var damageToKill = damageLogs.Where(x => x.Time > lastDeathTime && x.Time <= dead.Time && (x.HasHit || x.HasKilled)).ToList();
                ToKill = damageToKill.Count > 0 ? new List<DeathRecapDamageItem>() : null;
                int damage = 0;
                for (int i = damageToKill.Count - 1; i >= 0; i--)
                {
                    AbstractHealthDamageEvent dl = damageToKill[i];
                    AgentItem ag = dl.From;
                    var item = new DeathRecapDamageItem()
                    {
                        Time = (int)dl.Time,
                        IndirectDamage = dl is NonDirectHealthDamageEvent,
                        ID = dl.SkillId,
                        Damage = dl.HealthDamage,
                        Src = log.FindActor(ag)?.Character
                    };
                    damage += dl.HealthDamage;
                    ToKill.Add(item);
                    if (damage > 20000)
                    {
                        break;
                    }
                }
            }
        }

    }
}
