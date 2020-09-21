using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.ExportModels.Report
{
    public class HTMLBuilder
    {
        private Report Report;

        public HTMLBuilder(Report report)
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

        private string BuildPlayerSummary(PlayerReport player)
        {
            var html = Properties.Resources.player_summary;
            var damageDistribution = "";
            var takenDistribution = "";
            foreach (SummaryItem item in player.DamageSummary)
            {
                var icon = "<img src=\"" + item.Icon + "\" class=\"icon\">";
                double crits = (item.Hits == 0) ? 0.0 : ((double)item.Crit / (double)item.Hits) * 100;
                double flank = (item.Hits == 0) ? 0.0 : ((double)item.Flank / (double)item.Hits) * 100;
                double glance = (item.Hits == 0) ? 0.0 : ((double)item.Glance / (double)item.Hits) * 100;
                damageDistribution += "<tr><td>" + icon + item.Skill + "</td><td>" + item.Damage + "</td><td>" + item.BarrierDamage + "</td>";
                damageDistribution += "<td>" + item.Min + "</td><td>" + item.Max + "</td><td>" + item.Casts + "</td>";
                damageDistribution += "<td>" + item.Hits + "</td><td>" + item.HitsPerCast + "</td><td>" + string.Format("{0:0.00}", crits) + "%</td>";
                damageDistribution += "<td>" + string.Format("{0:0.00}", flank) + "%</td><td>" + string.Format("{0:0.00}", glance) + "%</td><td>" + item.Wasted + "s</td><td>" + item.Saved + "s</td></tr>";
            }
            foreach (SummaryItem item in player.TakenSummary)
            {
                var icon = "<img src=\"" + item.Icon + "\" class=\"icon\">";
                double crits = (item.Hits == 0) ? 0.0 : ((double)item.Crit / (double)item.Hits) * 100;
                double flank = (item.Hits == 0) ? 0.0 : ((double)item.Flank / (double)item.Hits) * 100;
                double glance = (item.Hits == 0) ? 0.0 : ((double)item.Glance / (double)item.Hits) * 100;
                takenDistribution += "<tr><td>" + icon + item.Skill + "</td><td>" + item.Damage + "</td><td>" + item.BarrierDamage + "</td>";
                takenDistribution += "<td>" + item.Min + "</td><td>" + item.Max + "</td>";
                takenDistribution += "<td>" + item.Hits + "</td><td>" + string.Format("{0:0.00}", crits) + "%</td>";
                takenDistribution += "<td>" + string.Format("{0:0.00}", flank) + "%</td><td>" + string.Format("{0:0.00}", glance) + "%</td></tr>";
            }
            var account = player.Account;
            var index = player.Account.IndexOf('.');
            if (index > 1)
            {
                account = player.Account.Substring(0, index);
            }

            html = html.Replace("<!-- Player Name -->", player.Name + " (" + player.Account + ")");
            html = html.Replace("<!-- Account Name -->", account.ToLower());
            html = html.Replace("<!-- Player Damage Data -->", damageDistribution);
            html = html.Replace("<!-- Player Taken Data -->", takenDistribution);
            return html;
        }

        private string BuildGeneralDataTables(string html)
        {
            var damage = "";
            var defense = "";
            var support = "";
            var summaries = "";
            var gameplay = "";
            var players = Report.players.Values.ToList<PlayerReport>();
            players.Sort(new PlayerReport());
            foreach (PlayerReport player in players)
            {
                var account = player.Account;
                var index = player.Account.IndexOf('.');
                if (index > 1)
                {
                    account = player.Account.Substring(0, index);
                }
                var combatTime = TimeSpan.FromMilliseconds(player.TimeInCombat).TotalSeconds;
                var professionIcon = GeneralHelper.GetProfIcon(player.Profession);

                var profession = "<img src=\"" + professionIcon + "\" alt=\"" + player.Profession + "\" class=\"icon\"><span style=\"display: none;\">" + player.Profession + "</span>";
                var tableStart = "<tr><td>" + player.Group + "</td><td>" + profession + "</td><td><a href=\"#" + account.ToLower() + "\">" + player.Name + "</a></td><td>" + player.Account + "</td>";
                damage += tableStart;
                defense += tableStart;
                support += tableStart;
                damage += "<td>" + Math.Round(combatTime, 0) + "</td>";
                damage += "<td>" + Math.Round(player.Damage.AllDamage / combatTime, 0) + "</td><td>" + player.Damage.AllDamage + "</td>";
                damage += "<td>" + player.Damage.Power + "</td><td>" + player.Damage.Condi + "</td>";
                damage += "<td>" + player.Damage.TargetDamage + "</td><td>" + player.Damage.TargetPower + "</td><td>" + player.Damage.TargetCondi + "</td></tr>";

                defense += "<td>" + player.Defense.DamageTaken + "</td><td>" + player.Defense.DamageBarrier + "</td>";
                defense += "<td>" + player.Defense.Blocked + "</td><td>" + player.Defense.Invulned + "</td>";
                defense += "<td>" + player.Defense.Interrupted + "</td><td>" + player.Defense.Evaded + "</td><td>" + player.Defense.Dodges + "</td>";
                defense += "<td>" + player.Defense.Missed + "</td><td>" + player.Defense.Downed + "</td><td>" + player.Defense.Dead + "</td></tr>";

                support += "<td>" + player.Support.CleanseOnOther + "</td><td>" + player.Support.CleanseOnSelf + "</td>";
                support += "<td>" + player.Support.BoonStrips + "</td><td>" + player.Support.Resurrects + "</td></tr>";
                gameplay += BuildGameplayTable(tableStart, player.Gameplay);
                summaries += BuildPlayerSummary(player);
            }

            html = html.Replace("<!-- Player Damage Stats -->", damage);
            html = html.Replace("<!-- Player Defense Stats -->", defense);
            html = html.Replace("<!-- Player Support Stats -->", support);
            html = html.Replace("<!-- Player Gameplay Stats -->", gameplay);
            html = html.Replace("<!-- Player Summaries -->", summaries);
            return html;
        }

        private string BuildGameplayTable(string tableStart, GameplayReport gameplay)
        {
            var html = tableStart;
            var crits = (gameplay.CriticalHits == 0) ? 0 : ((double)gameplay.CritableHits / (double)gameplay.CriticalHits) * 100.0;
            var flanks = (gameplay.ConnectedHits == 0) ? 0 : ((double)gameplay.Flanking / (double)gameplay.ConnectedHits) * 100.0;
            var glance = (gameplay.ConnectedHits == 0) ? 0 : ((double)gameplay.Glancing / (double)gameplay.ConnectedHits) * 100.0;
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
            html += $"<td>{string.Format("{0:0.00}", gameplay.AvgDistanceToSquad / Report.Logs.Count)}</td>";
            html += $"<td>{string.Format("{0:0.00}", gameplay.AvgDistanceToTag / Report.Logs.Count)}</td>";
            return html;
        }

        private string BuildFightLinks(string html)
        {
            string fightData = "";
            for (var i = 0; i < Report.Logs.Count; i++)
            {
                fightData += $"<button type=\"button\" class=\"btn btn-primary\" onclick=\"window.open('fight_{i}.html', '_blank')\" >{i+1}</button>";
            }
            html = html.Replace("<!-- Fight Links -->", fightData);
            return html;
        }

        public void CreatHTML(StreamWriter sw, string path)
        {
            string html = Properties.Resources.template_html;

            html = ReplaceVariables(html);
            html = BuildGeneralDataTables(html);
            html = BuildFightLinks(html);
            sw.Write(html);
        }
    }
}
