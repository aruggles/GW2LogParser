using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Gw2LogParser.ExportModels.Report;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
internal class Report
{
    public Dictionary<string, PlayerReport> players { get; set; } = [];
    public string LogsStart { get; set; } = "";
    public long LogsStartRaw { get; set; } = long.MaxValue;
    public string LogsEnd { get; set; } = "";
    public long LogsEndRaw { get; set; } = long.MinValue;
    public string PointOfView { get; set; } = "";
    public List<LogReport> Logs { get; set; } = [];
}
