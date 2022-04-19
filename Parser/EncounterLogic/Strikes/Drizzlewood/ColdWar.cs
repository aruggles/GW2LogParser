﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.El;
using Gw2LogParser.Parser.Data.El.Actors;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Helper;
using System.Linq;
using System.Collections.Generic;
using Gw2LogParser.Exceptions;

namespace Gw2LogParser.Parser.Logic
{
    internal class ColdWar : StrikeMissionLogic
    {
        public ColdWar(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new HitOnPlayerMechanic(60354, "Icy Echoes", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Icy.Ech","Tight stacking damage", "Icy Echoes",0),
                new HitOnPlayerMechanic(60006, "Detonate", new MechanicPlotlySetting("circle","rgb(255,125,0)"), "Det.","Hit by Detonation", "Detonate",50),
                new HitOnPlayerMechanic(60545, "Lethal Coalescence", new MechanicPlotlySetting("hexagram","rgb(255,100,0)"), "Leth.Coal.","Soaked damage", "Lethal Coalescence",50),
                new HitOnPlayerMechanic(60171, "Flame Wall", new MechanicPlotlySetting("square","rgb(255,125,0)"), "Flm.Wall","Stood in Flame Wall", "Flame Wall",50),
                new HitOnPlayerMechanic(60308, "Call Assassins", new MechanicPlotlySetting("diamond-tall","rgb(255,0,125)"), "Call Ass.","Hit by Assassins", "Call Assassins",50),
                new HitOnPlayerMechanic(60132, "Charge!", new MechanicPlotlySetting("diamond-tall","rgb(255,125,0)"), "Charge!","Hit by Charge", "Charge!",50),
            }
            );
            Extension = "coldwar";
            Icon = "https://i.imgur.com/r9b2oww.png";
            EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.Drizzlewood;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        /*protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/sXvx6AL.png",
                            (729, 581),
                            (-32118, -11470, -28924, -8274),
                            (-0, -0, 0, 0),
                            (0, 0, 0, 0));
        }*/

        internal override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor varinia = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.VariniaStormsounder);
            if (varinia == null)
            {
                throw new MissingKeyActorsException("Varinia Stormsounder not found");
            }
            phases[0].AddTarget(varinia);
            //
            // TODO - add phases if applicable
            //
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].AddTarget(varinia);
            }
            return phases;
        }

        // TODO - complete IDs
        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.PropagandaBallon,
                ArcDPSEnums.TrashID.DominionBladestorm,
                ArcDPSEnums.TrashID.DominionStalker,
                ArcDPSEnums.TrashID.DominionSpy1,
                ArcDPSEnums.TrashID.DominionSpy2,
                ArcDPSEnums.TrashID.DominionAxeFiend,
                ArcDPSEnums.TrashID.DominionEffigy,
                ArcDPSEnums.TrashID.FrostLegionCrusher,
                ArcDPSEnums.TrashID.FrostLegionMusketeer,
                ArcDPSEnums.TrashID.BloodLegionBlademaster,
                ArcDPSEnums.TrashID.CharrTank,
                ArcDPSEnums.TrashID.SonsOfSvanirHighShaman,
            };
        }
    }
}
