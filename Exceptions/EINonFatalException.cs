namespace Gw2LogParser.Exceptions
{
    public abstract class EINonFatalException : EIException
    {
        internal EINonFatalException(string message) : base(message)
        {
        }
    }
}
