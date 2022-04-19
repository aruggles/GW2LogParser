
namespace Gw2LogParser.Exceptions
{
    public class TooShortException : EINonFatalException
    {
        internal TooShortException(long shortnessValue, long minValue) : base("Fight is too short: " + shortnessValue + " < " + minValue)
        {
        }
    }
}
