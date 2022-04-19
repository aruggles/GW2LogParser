﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Logic
{
    internal class Escort : StrongholdOfTheFaithful
    {
        public Escort(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            }
            );
            Extension = "escort";
            Icon = "https://wiki.guildwars2.com/images/b/b5/Mini_McLeod_the_Silent.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        /*protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/RZbs21b.png",
                            (1099, 1114),
                            (-5467, 8069, -2282, 11297),
                            (-12288, -27648, 12288, 27648),
                            (1920, 12160, 2944, 14464));
        }*/

        internal override string GetLogicName(ParsedLog log)
        {
            return "Escort";
        }
    }
}
