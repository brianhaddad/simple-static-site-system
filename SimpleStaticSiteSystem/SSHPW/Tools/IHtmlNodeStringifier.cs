using SSClasses;

namespace SSHPW.Tools
{
    public interface IHtmlNodeStringifier
    {
        string[] Stringify(HtmlDocument htmlDoc);
        string[] Stringify(HtmlNode htmlNode);
    }
}