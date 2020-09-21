
namespace Gw2LogParser.Parser.Interfaces
{
    internal interface IStateable
    {
        (long start, double value) ToState();
    }
}
