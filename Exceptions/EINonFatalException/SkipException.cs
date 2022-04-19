
namespace Gw2LogParser.Exceptions
{
    public class SkipException : EINonFatalException
    {
        public SkipException() : base("Option enabled: Failed logs are skipped")
        {
        }
    }
}
