
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gw2LogParser.ExportModels.Report;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class SupportReport
{
    public int CleanseOnOther { get; set; }
    public int CleanseOnSelf { get; set; }
    public int BoonStrips { get; set; }
    public int Resurrects { get; set; }

    public SupportReport() {  }
    public SupportReport(SupportReport other)
    {
        CleanseOnOther = other.CleanseOnOther;
        CleanseOnSelf = other.CleanseOnSelf;
        BoonStrips = other.BoonStrips;
        Resurrects = other.Resurrects;
    }
}
