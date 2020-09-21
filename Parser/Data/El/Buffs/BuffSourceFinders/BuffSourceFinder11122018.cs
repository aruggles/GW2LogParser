﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.El.Buffs.BuffSourceFinders
{
    internal class BuffSourceFinder11122018 : BuffSourceFinder
    {
        public BuffSourceFinder11122018(HashSet<long> boonIds) : base(boonIds)
        {
            ExtensionIDS = new HashSet<long>()
            {
                10236,
                51696,
                29453
            };
            DurationToIDs = new Dictionary<long, HashSet<long>>
            {
                {5000, new HashSet<long> { 10236 } }, // SoI
                {3000, new HashSet<long> { 51696 } }, // Treated TN
                {2000, new HashSet<long> { 51696 , 29453 } }, // TN, SandSquall
            };
            EssenceOfSpeed = 2000;
            ImbuedMelodies = 2000;
        }
    }
}
