﻿using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.El.CombatReplays;
using Gw2LogParser.Parser.Data.El.Mechanics.MechanicTypes;
using Gw2LogParser.Parser.Helper;
using System.Collections.Generic;

namespace Gw2LogParser.Parser.Logic
{
    internal class Ensolyss : Nightmare
    {
        public Ensolyss(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(37154, "Lunge", new MechanicPlotlySetting("triangle-right-open","rgb(255,140,0)"), "Charge","Lunge (KB charge over arena)", "Charge",150),
            new HitOnPlayerMechanic(37278, "Upswing", new MechanicPlotlySetting("circle","rgb(255,100,0)"), "Smash 1","High damage Jump hit", "First Smash",0),
            new HitOnPlayerMechanic(36927, "Lunge", new MechanicPlotlySetting("triangle-right-open","rgb(255,140,0)"), "Charg","Lunge (KB charge over arena)", "Charge",150),
            new HitOnPlayerMechanic(36962, "Upswing", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Smash 2","Torment Explosion from Illusions after jump", "Torment Smash",50),
            new HitOnPlayerMechanic(37466, "Nightmare Miasma", new MechanicPlotlySetting("circle-open","rgb(255,0,255)"), "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new HitOnPlayerMechanic(36944, "Nightmare Miasma", new MechanicPlotlySetting("circle-open","rgb(255,0,255)"), "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new EnemyCastStartMechanic(37096, "Caustic Explosion", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","After Phase CC", "Breakbar", 0),
            new EnemyCastEndMechanic(37096, "Caustic Explosion", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","After Phase CC Failed", "CC Fail", 0, (ce,log) => ce.ActualDuration >= 15260),
            new EnemyCastEndMechanic(37096, "Caustic Explosion", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","After Phase CC Success", "CCed", 0, (ce, log) => ce.ActualDuration < 15260),
            new HitOnPlayerMechanic(37096, "Caustic Explosion", new MechanicPlotlySetting("bowtie","rgb(255,200,0)"), "CC KB","Knockback hourglass during CC", "CC KB", 0),
            new EnemyCastStartMechanic(37523, "Nightmare Devastation", new MechanicPlotlySetting("square-open","rgb(0,0,255)"), "Bubble","Nightmare Devastation (bubble attack)", "Bubble",0),
            new EnemyCastStartMechanic(37050, "Nightmare Devastation", new MechanicPlotlySetting("square-open","rgb(0,0,255)"), "Bubble","Nightmare Devastation (bubble attack)", "Bubble",0),
            new HitOnPlayerMechanic(37151, "Nightmare Miasma", new MechanicPlotlySetting("circle-open","rgb(0,0,255)"), "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new HitOnPlayerMechanic(37003, "Tail Lash", new MechanicPlotlySetting("triangle-left","rgb(255,200,0)"), "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0),
            new HitOnPlayerMechanic(37434, "Rampage", new MechanicPlotlySetting("asterisk-open","rgb(255,0,0)"), "Rampage","Rampage (asterisk shaped Arrow attack)", "Rampage",150),
            new HitOnPlayerMechanic(37045, "Caustic Grasp", new MechanicPlotlySetting("star-diamond","rgb(255,140,0)"), "Pull","Caustic Grasp (Arena Wide Pull)", "Pull",0),
            new HitOnPlayerMechanic(36926, "Tormenting Blast", new MechanicPlotlySetting("diamond","rgb(255,255,0)"), "Quarter","Tormenting Blast (Two Quarter Circle attacks)", "Quarter circle",0),
            });
            Extension = "ensol";
            Icon = "https://i.imgur.com/pqRYRGi.png";
            EncounterCategoryInformation.InSubCategoryOrder = 2;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/kjelZ4t.png",
                            (366, 366),
                            (252, 1, 2892, 2881)/*,
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054)*/);
        }
        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<Combat> combatData)
        {
            return GetFightOffsetByFirstInvulFilter(fightData, agentData, combatData, (int)ArcDPSEnums.TargetID.Ensolyss, 762, 1500);
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.CMnoName;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.NightmareHallucination1,
                ArcDPSEnums.TrashID.NightmareHallucination2
            };
        }
    }
}
