using GW2EIBuilders;
using GW2EIBuilders.HtmlModels.HTMLStats;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Gw2LogParser.ExportModels.Report;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
internal class HTMLReportBuilder
{
    private Report Report;

    public HTMLReportBuilder(Report report)
    {
        Report = report;
    }

    private string ReplaceVariables(string html)
    {
        html = html.Replace("<!-- STARTLOG -->", Report.LogsStart);
        html = html.Replace("<!-- ENDLOG -->", Report.LogsEnd);
        html = html.Replace("<!-- POINTOFVIEW -->", Report.PointOfView);
        return html;
    }

    private string CompressAndBase64(string s)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        using (var msi = new MemoryStream(bytes))
        {
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }
    }

    private string BuildPlayerSummary(PlayerReport player)
    {
        var html = Properties.Resources.player_summary;
        /*
        var damageDistribution = "";
        var takenDistribution = "";
        foreach (SummaryItem item in player.DamageSummary)
        {
            var icon = "<img src=\"" + item.Icon + "\" class=\"icon\">";
            damageDistribution += "<tr><td>" + icon + item.Skill + "</td><td>" + item.Damage + "</td><td>" + item.BarrierDamage + "</td>";
            damageDistribution += "<td>" + item.Min + "</td><td>" + item.Max + "</td><td>" + formatNumber(item.Casts) + "</td>";
            damageDistribution += "<td>" + item.Hits + "</td><td>" + formatNumber(Math.Round(item.HitsPerCast, 1)) + "</td><td>" + formatNumber(item.CritPercent, "{0:0.00}", "%") + "</td>";
            damageDistribution += "<td>" + formatNumber(item.FlankPercent, "{0:0.00}", "%") + "</td><td>" + formatNumber(item.GlancePercent, "{0:0.00}", "%") + "</td><td>" + formatNumber(item.Wasted, "{0:0.000}", "s") + "</td><td>" + formatNumber(item.Saved, "{0:0.000}", "s") + "</td></tr>";
        }
        foreach (SummaryItem item in player.TakenSummary)
        {
            var icon = "<img src=\"" + item.Icon + "\" class=\"icon\">";
            takenDistribution += "<tr><td>" + icon + item.Skill + "</td><td>" + item.Damage + "</td><td>" + item.BarrierDamage + "</td>";
            takenDistribution += "<td>" + item.Min + "</td><td>" + item.Max + "</td>";
            takenDistribution += "<td>" + item.Hits + "</td><td>" + formatNumber(item.CritPercent, "{0:0.00}", "%") + "</td>";
            takenDistribution += "<td>" + formatNumber(item.FlankPercent, "{0:0.00}", "%") + "</td><td>" + formatNumber(item.GlancePercent, "{0:0.00}", "%") + "</td></tr>";
        }
        */
        /*
        var account = player.Account;
        var index = player.Account.IndexOf('.');
        if (index > 1)
        {
            account = player.Account.Substring(0, index);
        }
        */

        html = html.Replace("<!-- Player Name -->", player.Name + " (" + player.Account + ")");
        html = html.Replace("<!-- Player Identifier -->", player.Identifier);
        // html = html.Replace("<!-- Player Damage Data -->", damageDistribution);
        //html = html.Replace("<!-- Player Taken Data -->", takenDistribution);
        return html;
    }

    private string BuildGeneralDataTables(string html)
    {
        //var damage = "";
        //var defense = "";
        //var support = "";
        var summaries = "";
        // var gameplay = "";
        var boonuptime = "";
        var boonself = "";
        var boongroup = "";
        var boonoffgroup = "";
        var boonsquad = "";
        var players = Report.players.Values.ToList<PlayerReport>();
        players.Sort(new PlayerReport());
        foreach (PlayerReport player in players)
        {
            var combatTime = TimeSpan.FromMilliseconds(player.TimeInCombat).TotalSeconds;

            var profession = "<img src=\"" + player.Icon + "\" alt=\"" + player.Profession + "\" class=\"icon\"><span style=\"display: none;\">" + player.Profession + "</span>";
            var tableStart = "<tr><td>" + player.Group + "</td><td>" + profession + "</td><td><a href=\"#" + player.Identifier + "\">" + player.Name + "</a></td><td>" + player.Account + "</td>";
            // damage += tableStart;
            // defense += tableStart;
            //support += tableStart;
            //damage += "<td>" + Math.Round(combatTime, 0) + "</td>";
            //damage += "<td>" + Math.Round(player.Damage.AllDamage / combatTime, 0) + "</td><td>" + player.Damage.AllDamage + "</td>";
            //damage += "<td>" + player.Damage.Power + "</td><td>" + player.Damage.Condi + "</td>";
            //damage += "<td>" + player.Damage.TargetDamage + "</td><td>" + player.Damage.TargetPower + "</td><td>" + player.Damage.TargetCondi + "</td></tr>";

            //defense += "<td>" + player.Defense.DamageTaken + "</td><td>" + player.Defense.DamageBarrier + "</td>";
            //defense += "<td>" + player.Defense.Blocked + "</td><td>" + player.Defense.Invulned + "</td>";
            //defense += "<td>" + player.Defense.Interrupted + "</td><td>" + player.Defense.Evaded + "</td><td>" + player.Defense.Dodges + "</td>";
            //defense += "<td>" + player.Defense.Missed + "</td><td>" + player.Defense.Downed + "</td><td>" + player.Defense.Dead + "</td></tr>";

            //support += "<td>" + player.Support.CleanseOnOther + "</td><td>" + player.Support.CleanseOnSelf + "</td>";
            //support += "<td>" + player.Support.BoonStrips + "</td><td>" + player.Support.Resurrects + "</td></tr>";
            //gameplay += BuildGameplayTable(tableStart, player.Gameplay, player);
            summaries += BuildPlayerSummary(player);
            boonuptime += BuildBoonTable(tableStart, player.BoonStats, false, player);
            boonself += BuildBoonTable(tableStart, player.BoonGenSelfStats, true, player);
            boongroup += BuildBoonTable(tableStart, player.BoonGenGroupStats, true, player);
            boonoffgroup += BuildBoonTable(tableStart, player.BoonGenOGroupStats, true, player);
            boonsquad += BuildBoonTable(tableStart, player.BoonGenSquadStats, true, player);
        }

        // html = html.Replace("<!-- Player Damage Stats -->", damage);
        // html = html.Replace("<!-- Player Defense Stats -->", defense);
        // html = html.Replace("<!-- Player Support Stats -->", support);
        // html = html.Replace("<!-- Player Gameplay Stats -->", gameplay);
        html = html.Replace("<!-- Player Summaries -->", summaries);
        html = html.Replace("<!-- Player Boon Uptime Stats -->", boonuptime);
        html = html.Replace("<!-- Player Boon Self Stats -->", boonself);
        html = html.Replace("<!-- Player Boon Group Stats -->", boongroup);
        html = html.Replace("<!-- Player Boon OffGroup Stats -->", boonoffgroup);
        html = html.Replace("<!-- Player Boon Squad Stats -->", boonsquad);
        return html;
    }

    private string BuildGameplayTable(string tableStart, GameplayReport gameplay, PlayerReport player)
    {
        var html = tableStart;
        var crits = (gameplay.CriticalHits == 0) ? 0 : ((double)gameplay.CritableHits / (double)gameplay.CriticalHits) * 100.0;
        var flanks = (gameplay.ConnectedHits == 0) ? 0 : ((double)gameplay.Flanking / (double)gameplay.ConnectedHits) * 100.0;
        var glance = (gameplay.ConnectedHits == 0) ? 0 : ((double)gameplay.Glancing / (double)gameplay.ConnectedHits) * 100.0;
        html += $"<td>{player.numberOfFights}</td>";
        html += $"<td data-toggle=\"tooltip\" title=\"{gameplay.CritableHits} out of {gameplay.CriticalHits} critable hit(s)\">{string.Format("{0:0.00}", crits)}%</td>";
        html += $"<td data-toggle=\"tooltip\" title=\"{gameplay.Flanking} out of {gameplay.ConnectedHits} connected hit(s)\">{string.Format("{0:0.00}", flanks)}%</td>";
        html += $"<td data-toggle=\"tooltip\" title=\"{gameplay.Glancing} out of {gameplay.ConnectedHits} connected hit(s)\">{string.Format("{0:0.00}", glance)}%</td>";
        html += $"<td>{gameplay.Blined}</td>";
        html += $"<td>{gameplay.Interrupted}</td>";
        html += $"<td>{gameplay.Invulnerable}</td>";
        html += $"<td>{gameplay.Evaded}</td>";
        html += $"<td>{gameplay.Blocked}</td>";
        html += $"<td>{gameplay.Wasted}</td>";
        html += $"<td>{gameplay.Saved}</td>";
        html += $"<td>{gameplay.WeaponSwapped}</td>";
        html += $"<td>{string.Format("{0:0.00}", gameplay.AvgDistanceToSquad / (double) player.numberOfFights)}</td>";
        html += $"<td>{string.Format("{0:0.00}", gameplay.AvgDistanceToTag / (double)player.numberOfFights)}</td></tr>";
        return html;
    }

    private string BuildBoonTable(string tableStart, List<BoonReport> boons, bool wasted, PlayerReport player)
    {
        //var numberOfFights = (double) player.numberOfFights;
        double numberOfFights = player.numberOfFights;
        var html = tableStart;
        for (var i = 0; i < 12; i++)
        {
            if (boons.Count > i)
            {
                var boon = boons[i];
                var valueText = (boon.Value == 0) ? "-" : $"{string.Format("{0:0.000}", boon.Value / numberOfFights)}";
                if (!wasted && boon.Uptime != 0 && boon.Uptime != boon.Value) // Turns on tooltip for uptime info.
                {
                    html += $"<td data-toggle=\"tooltip\" title=\"Uptime: {string.Format("{0:0.000}", boon.Uptime / numberOfFights)}%\">{valueText}</td>";
                }
                else if (wasted) // Turns on tooltip for overstacked info.
                {
                    var toolTip = ""; // $"{string.Format("{0:0.000}", boon.Value / numberOfFights)} with overstack";
                    toolTip += (boon.Wasted != 0) ? $"{string.Format("{0:0.000}", boon.Wasted / numberOfFights)} wasted" : "";
                    toolTip += (boon.Extended != 0) ? $" {string.Format("{0:0.000}", boon.Extended / numberOfFights)} extended" : "";
                    html += $"<td data-toggle=\"tooltip\" title=\"{toolTip}\">";
                    if (i == 0 || i == 8) // Stack.
                    {
                        html += $"{string.Format("{0:0.000}", boon.Value / numberOfFights)}</td>";
                    }
                    else // Percentage.
                    {
                        html += (valueText == "-") ? $"{valueText}</td>" : $"{valueText}%</td>";
                    }
                }
                else // No Tooltip.
                {
                    html += (valueText == "-") ? $"<td>{valueText}</td>" : $"<td>{valueText}%</td>";
                }
            }
            else // No Boon.
            {
                html += "<td>-</td>";
            }
        }
        html += "</tr>";
        return html;
    }

    private static string ToJson(object value)
    {
        var settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            StringEscapeHandling = StringEscapeHandling.EscapeHtml
        };
        return JsonConvert.SerializeObject(value, settings);
    }

    private string BuildFightLinks(string html)
    {
        string fightData = "";
        for (var i = 0; i < Report.Logs.Count; i++)
        {
            fightData += $"<button type=\"button\" class=\"btn btn-primary\" onclick=\"window.open('fight_{i+1}.html', '_blank')\" >{i+1} {Report.Logs[i].lengthInSeconds}s</button>";
        }
        html = html.Replace("<!-- Fight Links -->", fightData);
        return html;
    }

    public void CreatHTML(StreamWriter sw, string path)
    {
        string html = Properties.Resources.template_html;
        var json = ToJson(Report.players);

        html = ReplaceVariables(html);
        html = BuildGeneralDataTables(html);
        html = BuildFightLinks(html);
        html = html.Replace("${logDataJson}", "'" + CompressAndBase64(json) + "'");
        html = html.Replace("<!--${CompressionUtils}-->", Properties.Resources.compressionUtils);
        sw.Write(html);
    }
}
