using GW2EIBuilders.HtmlModels.HTMLStats;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gw2LogParser.ExportModels.Report;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class BoonReport
{
    public double Value { get; set; }
    public double Uptime { get; set; }
    public double Extended { get; set; }
    public double Wasted { get; set; }
    public double Overstack {  get; set; }
    public BoonReport() { }
    public BoonReport(List<double> data)
    {
        Value = 0;
        Uptime = 0;
        Extended = 0;
        Wasted = 0;
        Overstack = 0;
        for (int i = 0; i < data.Count; i++)
        {
            switch (i)
            {
                case 0:
                    Value = data[i];
                    break;
                case 1:
                    Uptime = data[i];
                    break;
                case 2:
                    Wasted = data[i];
                    break;
                case 5:
                    Extended = data[i];
                    break;
                default:
                    break;
            }
        }
    }
}
