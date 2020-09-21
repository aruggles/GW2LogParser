using System;

namespace Gw2LogParser.Exceptions
{
    public class IncompleteLogException : Exception
    {
        internal IncompleteLogException() : base("Log incomplete")
        {
        }

    }
}
