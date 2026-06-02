using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
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

    private static string ToJson(object value)
    {
        var settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            StringEscapeHandling = StringEscapeHandling.EscapeHtml
        };
        return JsonConvert.SerializeObject(value, settings);
    }

    public void CreatHTML(StreamWriter sw, string path)
    {
        // The template renders the entire UI client-side; the builder only emits data.
        // Project a slim envelope: report metadata + lightweight per-fight info (NOT the
        // heavy per-fight LogReport.players dict) + the existing name-keyed players dict.
        var payload = new
        {
            logsStart = Report.LogsStart,
            logsEnd = Report.LogsEnd,
            pointOfView = Report.PointOfView,
            fights = Report.Logs.Select((log, i) => new
            {
                index = i + 1,
                name = log.Name,
                duration = log.DurationString
            }),
            players = Report.players
        };
        var json = ToJson(payload);

        string html = Properties.Resources.template_html;
        html = html.Replace("${logDataJson}", "'" + CompressAndBase64(json) + "'");
        html = html.Replace("<!--${CompressionUtils}-->", Properties.Resources.compressionUtils);
        sw.Write(html);
    }
}
