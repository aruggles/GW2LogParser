
namespace Gw2LogParser.GW2EIBuilders
{
    public class HTMLSettings
    {

        public bool HTMLLightTheme { get; }

        public bool ExternalHTMLScripts { get; }

        public HTMLSettings(bool htmlLightTheme, bool externalHTMLScripts)
        {
            HTMLLightTheme = htmlLightTheme;
            ExternalHTMLScripts = externalHTMLScripts;
        }
    }
}
