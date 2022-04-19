using System;

namespace Gw2LogParser.Exceptions
{
    public abstract class EIException : Exception
    {
        internal EIException(string message) : base(message)
        {
        }
    }
}
