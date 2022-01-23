using SSHPW.Classes;

namespace SSHPW
{
    public static class HtmlHelpers
    {
        public static string Stringify(this ParsedHtmlNodeTree htmlDoc, HtmlStringificationOptions options)
            => htmlDoc.StringifyToLines(options).Join(Environment.NewLine);

        public static string Stringify(this HtmlNode htmlNode, HtmlStringificationOptions options)
            => htmlNode.StringifyToLines(options).Join(Environment.NewLine);

        public static string[] StringifyToLines(this ParsedHtmlNodeTree htmlDoc, HtmlStringificationOptions options)
        {
            var stringifier = new HtmlNodeStringifier(options);
            return stringifier.Stringify(htmlDoc);
        }

        public static string[] StringifyToLines(this HtmlNode htmlNode, HtmlStringificationOptions options)
        {
            var stringifier = new HtmlNodeStringifier(options);
            return stringifier.Stringify(htmlNode);
        }
    }
}
