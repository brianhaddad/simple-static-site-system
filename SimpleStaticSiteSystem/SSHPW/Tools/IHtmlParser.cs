using SSClasses;

namespace SSHPW.Tools
{
    public interface IHtmlParser
    {
        HtmlDocument Parse(string text);
        HtmlDocument Parse(string[] lines);
    }
}