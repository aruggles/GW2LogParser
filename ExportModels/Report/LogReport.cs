using System.Collections.Generic;

namespace Gw2LogParser.ExportModels.Report;

internal class LogReport
{
    public string Name { get; set; } = "";
    public Dictionary<string, PlayerReport> players { get; set; } = new Dictionary<string, PlayerReport>();
    public long LogsStart { get; set; }
    public long LogsEnd { get; set; }
    public long Duration { get; set; }
    public string DurationString { get; set; } = "";
    public string PointOfView { get; set; } = "";
}
