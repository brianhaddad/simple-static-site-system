using SSClasses;

namespace SSHPW
{
    public interface ISuperSimpleHtmlParserWriter
    {
        string[] Stringify(HtmlDocument htmlDoc);
        HtmlDocument Parse(string[] documentLines);
        HtmlDocument Parse(string documentText);
    }
}
