using SSHPW.Classes;

namespace SSHPW.Tools
{
    public interface IHtmlPreParser
    {
        List<NodeParsingData> GetParsedSymbols(string[] lines);
        List<NodeParsingData> GetParsedSymbols(string text);
    }
}