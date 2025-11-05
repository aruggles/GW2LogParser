using GW2EIEvtcParser;

namespace GW2EIParserCommons.Exceptions;

public class ProgramException : Exception
{
    public ProgramException(string? message) : base(message)
    {
    }

    internal ProgramException(Exception ex) : base("Operation aborted", ParserHelper.GetFinalException(ex))
    {
    }
}
