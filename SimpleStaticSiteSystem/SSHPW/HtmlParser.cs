using SSHPW.Classes;
using SSHPW.Classes.Enums;
using SSHPW.Exceptions;

namespace SSHPW
{
    public class HtmlParser
    {
        private readonly HtmlStringSanitizer _sanitizer;
        private readonly HtmlPreParser _preParser;

        public HtmlParser(HtmlStringSanitizer sanitizer, HtmlPreParser preParser)
        {
            _sanitizer = sanitizer ?? throw new ArgumentNullException(nameof(sanitizer));
            _preParser = preParser ?? throw new ArgumentNullException(nameof(preParser));
        }

        public ParsedHtmlNodeTree Parse(string[] lines)
            => ParseSanitized(_sanitizer.Sanitize(lines));

        public ParsedHtmlNodeTree Parse(string text)
            => ParseSanitized(_sanitizer.Sanitize(text));

        private ParsedHtmlNodeTree ParseSanitized(string text)
        {
            var preparse = _preParser.GetParsingData(text);
            var result = new ParsedHtmlNodeTree
            {
                ContainsDocTypeDeclaration = preparse.ContainsDocTypeDeclaration,
                DocTypeValues = preparse.DocTypeValues,
            };
            //TODO: once that is done, build the node tree from the data
            foreach (var nodeData in preparse.Data)
            {
                if (nodeData.ParsedDataType == ParsingDataType.Tag)
                {
                    var newNode = new HtmlNode();
                }
                else if (nodeData.ParsedDataType == ParsingDataType.Text)
                {
                    var textNode = new HtmlNode(nodeData.Text);
                }
                else
                {
                    throw new NotImplementedException($"No handling exists for {nodeData.ParsedDataType}.");
                }
            }
            return result;
        }
    }
}