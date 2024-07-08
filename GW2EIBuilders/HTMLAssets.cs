using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GW2EIBuilders
{
    public class HTMLAssets
    {
        internal string EIJavascriptCode { get; }

        internal string EICRJavascriptCode { get; }
        internal string EIHealingExtJavascriptCode { get; }

        public HTMLAssets()
        {
            var orderedScripts = new List<string>()
            {
                Gw2LogParser.Properties.Resources.global,
                Gw2LogParser.Properties.Resources.mixins,
                Gw2LogParser.Properties.Resources.functions,
                Gw2LogParser.Properties.Resources.main_js,
            };
            string scriptContent = orderedScripts[0];
            for (int i = 1; i < orderedScripts.Count; i++)
            {
                scriptContent += orderedScripts[i];
            }
            List<string> templates = BuildTemplates();
            EIJavascriptCode = scriptContent.Replace("TEMPLATE_COMPILE", string.Join("\n", templates));
            //
            var orderedCRScripts = new List<string>()
            {
                Gw2LogParser.Properties.Resources.animator,
                Gw2LogParser.Properties.Resources.actors,
                Gw2LogParser.Properties.Resources.decorations,
            };
            string scriptCRContent = orderedCRScripts[0];
            for (int i = 1; i < orderedCRScripts.Count; i++)
            {
                scriptCRContent += orderedCRScripts[i];
            }
            List<string> templatesCR = BuildCRTemplates();
            EICRJavascriptCode = scriptCRContent.Replace("TEMPLATE_CR_COMPILE", string.Join("\n", templatesCR));
            //
            var orderedHealingExtScripts = new List<string>()
            {
                Gw2LogParser.Properties.Resources.healingExtGlobals,
                Gw2LogParser.Properties.Resources.healingExtFunctions,
            };
            string scriptHealingExtContent = orderedHealingExtScripts[0];
            for (int i = 1; i < orderedHealingExtScripts.Count; i++)
            {
                scriptHealingExtContent += orderedHealingExtScripts[i];
            }
            List<string> templateHealingExt = BuildHealingExtensionTemplates();
            EIHealingExtJavascriptCode = scriptHealingExtContent.Replace("TEMPLATE_HEALING_EXT_COMPILE", string.Join("\n", templateHealingExt));
        }
        private static string PrepareTemplate(string template)
        {
            if (!template.Contains("<template>") || !template.Contains("<script>") || !template.Contains("${template}"))
            {
                throw new InvalidDataException("Not a template");
            }
            string html = template.Split(new string[] { "<template>" }, StringSplitOptions.None)[1].Split(new string[] { "</template>" }, StringSplitOptions.None)[0];
            string js = template.Split(new string[] { "<script>" }, StringSplitOptions.None)[1].Split(new string[] { "</script>" }, StringSplitOptions.None)[0];
            js = js.Replace("${template}", Regex.Replace(html, @"\t|\n|\r", ""));
            js = "{" + js + "}";
            return js;
        }

        private static List<string> BuildTemplates()
        {
            var templates = new List<string>
            {
                Gw2LogParser.Properties.Resources.tmplBuffStats,
                Gw2LogParser.Properties.Resources.tmplBuffStatsPlayer,
                Gw2LogParser.Properties.Resources.tmplBuffStatsTarget,
                Gw2LogParser.Properties.Resources.tmplBuffTable,
                Gw2LogParser.Properties.Resources.tmplDamageDistPlayer,
                Gw2LogParser.Properties.Resources.tmplDamageDistTable,
                Gw2LogParser.Properties.Resources.tmplDamageDistTarget,
                Gw2LogParser.Properties.Resources.tmplDamageModifierTable,
                Gw2LogParser.Properties.Resources.tmplDamageModifierStatsContainer,
                Gw2LogParser.Properties.Resources.tmplDamageModifierStats,
                Gw2LogParser.Properties.Resources.tmplDamageModifierPersStats,
                Gw2LogParser.Properties.Resources.tmplDamageTable,
                Gw2LogParser.Properties.Resources.tmplDamageTaken,
                Gw2LogParser.Properties.Resources.tmplDeathRecap,
                Gw2LogParser.Properties.Resources.tmplDefenseTable,
                Gw2LogParser.Properties.Resources.tmplEncounter,
                Gw2LogParser.Properties.Resources.tmplFood,
                Gw2LogParser.Properties.Resources.tmplGameplayTable,
                Gw2LogParser.Properties.Resources.tmplOffensiveTable,
                Gw2LogParser.Properties.Resources.tmplBuffTables,
                Gw2LogParser.Properties.Resources.tmplStatTables,
                Gw2LogParser.Properties.Resources.tmplMechanicsTable,
                Gw2LogParser.Properties.Resources.tmplGearBuffTable,
                Gw2LogParser.Properties.Resources.tmplNourishmentBuffTable,
                Gw2LogParser.Properties.Resources.tmplEnhancementBuffTable,
                Gw2LogParser.Properties.Resources.tmplOtherConsumableBuffTable,
                Gw2LogParser.Properties.Resources.tmplDebuffTable,
                Gw2LogParser.Properties.Resources.tmplConditionsTable,
                Gw2LogParser.Properties.Resources.tmplPersonalBuffTable,
                Gw2LogParser.Properties.Resources.tmplPhase,
                Gw2LogParser.Properties.Resources.tmplPlayers,
                Gw2LogParser.Properties.Resources.tmplPlayerStats,
                Gw2LogParser.Properties.Resources.tmplPlayerTab,
                Gw2LogParser.Properties.Resources.tmplSimpleRotation,
                Gw2LogParser.Properties.Resources.tmplAdvancedRotation,
                Gw2LogParser.Properties.Resources.tmplSupportTable,
                Gw2LogParser.Properties.Resources.tmplTargets,
                Gw2LogParser.Properties.Resources.tmplTargetStats,
                Gw2LogParser.Properties.Resources.tmplTargetTab,
                Gw2LogParser.Properties.Resources.tmplDPSGraph,
                Gw2LogParser.Properties.Resources.tmplDPSGraphModeSelector,
                Gw2LogParser.Properties.Resources.tmplGraphStats,
                Gw2LogParser.Properties.Resources.tmplPlayerTabGraph,
                Gw2LogParser.Properties.Resources.tmplPlayersRotation,
                Gw2LogParser.Properties.Resources.tmplPlayersRotationTab,
                Gw2LogParser.Properties.Resources.tmplRotationLegend,
                Gw2LogParser.Properties.Resources.tmplTargetTabGraph,
                Gw2LogParser.Properties.Resources.tmplTargetTabPerPlayerGraph,
                Gw2LogParser.Properties.Resources.tmplTargetData,
                Gw2LogParser.Properties.Resources.tmplMainView,
            };
            var res = new List<string>();
            foreach (string template in templates)
            {
                res.Add(PrepareTemplate(template));
            }
            return res;
        }

        private static List<string> BuildCRTemplates()
        {
            var templates = new List<string>
            {
                Gw2LogParser.Properties.Resources.tmplCombatReplayDamageData,
                Gw2LogParser.Properties.Resources.tmplCombatReplayStatusData,
                Gw2LogParser.Properties.Resources.tmplCombatReplayDamageTable,
                Gw2LogParser.Properties.Resources.tmplCombatReplayActorBuffStats,
                Gw2LogParser.Properties.Resources.tmplCombatReplayPlayerStats,
                Gw2LogParser.Properties.Resources.tmplCombatReplayPlayerStatus,
                Gw2LogParser.Properties.Resources.tmplCombatReplayActorRotation,
                Gw2LogParser.Properties.Resources.tmplCombatReplayTargetStats,
                Gw2LogParser.Properties.Resources.tmplCombatReplayTargetStatus,
                Gw2LogParser.Properties.Resources.tmplCombatReplayTargetsStats,
                Gw2LogParser.Properties.Resources.tmplCombatReplayPlayersStats,
                Gw2LogParser.Properties.Resources.tmplCombatReplayUI,
                Gw2LogParser.Properties.Resources.tmplCombatReplayPlayerSelect,
                Gw2LogParser.Properties.Resources.tmplCombatReplayTargetSelect,
                Gw2LogParser.Properties.Resources.tmplCombatReplayExtraDecorations,
                Gw2LogParser.Properties.Resources.tmplCombatReplayAnimationControl,
                Gw2LogParser.Properties.Resources.tmplCombatReplayMechanicsList
            };
            var res = new List<string>();
            foreach (string template in templates)
            {
                res.Add(PrepareTemplate(template));
            }
            return res;
        }

        private static List<string> BuildHealingExtensionTemplates()
        {
            var templates = new List<string>
            {
                Gw2LogParser.Properties.Resources.tmplHealingExtensionView,
                Gw2LogParser.Properties.Resources.tmplTargetPlayers,
                Gw2LogParser.Properties.Resources.tmplIncomingHealingTable,
                Gw2LogParser.Properties.Resources.tmplHealingStatTables,
                Gw2LogParser.Properties.Resources.tmplOutgoingHealingTable,
                Gw2LogParser.Properties.Resources.tmplHPSGraphModeSelector,
                Gw2LogParser.Properties.Resources.tmplHPSGraph,
                Gw2LogParser.Properties.Resources.tmplHealingGraphStats,
                Gw2LogParser.Properties.Resources.tmplHealingDistPlayer,
                Gw2LogParser.Properties.Resources.tmplHealingDistTable,
                Gw2LogParser.Properties.Resources.tmplPlayerHealingStats,
                Gw2LogParser.Properties.Resources.tmplPlayerHealingTab,
                Gw2LogParser.Properties.Resources.tmplHealingTaken,
                Gw2LogParser.Properties.Resources.tmplPlayerHealingTabGraph,
                Gw2LogParser.Properties.Resources.tmplBarrierDistPlayer,
                Gw2LogParser.Properties.Resources.tmplBarrierDistTable,
                Gw2LogParser.Properties.Resources.tmplBarrierTaken,
            };
            var res = new List<string>();
            foreach (string template in templates)
            {
                res.Add(PrepareTemplate(template));
            }
            return res;
        }
    }
}
