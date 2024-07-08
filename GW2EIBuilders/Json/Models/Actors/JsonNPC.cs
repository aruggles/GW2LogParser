﻿using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    /// <summary>
    /// Class representing an NPC
    /// </summary>
    public class JsonNPC : JsonActor
    {

        /// <summary>
        /// Game ID of the target
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Final health of the target
        /// </summary>
        public int FinalHealth { get; set; }

        /// <summary>
        /// % of health burned
        /// </summary>
        public double HealthPercentBurned { get; set; }

        /// <summary>
        /// Time at which target became active
        /// </summary>
        public int FirstAware { get; set; }

        /// <summary>
        /// Time at which target became inactive 
        /// </summary>
        public int LastAware { get; set; }

        /// <summary>
        /// List of buff status
        /// </summary>
        /// <seealso cref="JsonBuffsUptime"/>
        public IReadOnlyList<JsonBuffsUptime> Buffs { get; set; }

        /// <summary>
        /// Indicates that the JsonNPC is actually an enemy player
        /// </summary>
        public bool EnemyPlayer { get; set; }

        /// <summary>
        /// Array of double[2] that represents the breakbar percent of the actor \n
        /// Value[i][0] will be the time, value[i][1] will be breakbar % \n
        /// If i corresponds to the last element that means the breakbar did not change for the remainder of the fight \n
        /// </summary>
        public IReadOnlyList<IReadOnlyList<double>> BreakbarPercents { get; set; }


        public JsonNPC()
        {

        }
    }
}
