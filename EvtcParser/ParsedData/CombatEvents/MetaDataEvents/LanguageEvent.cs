using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class LanguageEvent : MetaDataEvent
{

    public readonly LanguageEnum Language;

    internal LanguageEvent(CombatItem evtcItem) : base(evtcItem)
    {
        Language = GetLanguage(evtcItem);
    }

    internal static LanguageEnum GetLanguage(CombatItem evtcItem)
    {
        return ArcDPSEnums.GetLanguage((byte)evtcItem.SrcAgent);
    }

    public override string ToString()
    {
        switch (Language)
        {
            case LanguageEnum.English:
                return "English";
            case LanguageEnum.Missing:
                return "Missing";
            case LanguageEnum.French:
                return "French";
            case LanguageEnum.German:
                return "German";
            case LanguageEnum.Spanish:
                return "Spanish";
            case LanguageEnum.Chinese:
                return "Chinese";
        }
        return "Unknown";
    }

}
