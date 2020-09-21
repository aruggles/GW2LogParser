
namespace Gw2LogParser.Parser.Interfaces
{
    internal interface IVersionable
    {
        bool Available(ulong gw2Build);
    }
}
