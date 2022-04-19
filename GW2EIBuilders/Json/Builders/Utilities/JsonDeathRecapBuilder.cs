﻿using Gw2LogParser.Parser.Data.El.Statistics;
using System.Linq;
using static Gw2LogParser.GW2EIBuilders.JsonDeathRecap;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonDeathRecapBuilder
    {
        private static JsonDeathRecapDamageItem BuildJsonDeathRecapDamageItem(DeathRecap.DeathRecapDamageItem item)
        {
            var jsonDeathRecapDamageItem = new JsonDeathRecapDamageItem();
            jsonDeathRecapDamageItem.Id = item.ID;
            jsonDeathRecapDamageItem.IndirectDamage = item.IndirectDamage;
            jsonDeathRecapDamageItem.Src = item.Src;
            jsonDeathRecapDamageItem.Damage = item.Damage;
            jsonDeathRecapDamageItem.Time = item.Time;
            return jsonDeathRecapDamageItem;
        }

        public static JsonDeathRecap BuildJsonDeathRecap(DeathRecap recap)
        {
            var jsonDeathRecap = new JsonDeathRecap();
            jsonDeathRecap.DeathTime = recap.DeathTime;
            jsonDeathRecap.ToDown = recap.ToDown?.Select(x => BuildJsonDeathRecapDamageItem(x)).ToList();
            jsonDeathRecap.ToKill = recap.ToKill?.Select(x => BuildJsonDeathRecapDamageItem(x)).ToList();
            return jsonDeathRecap;
        }
    }
}