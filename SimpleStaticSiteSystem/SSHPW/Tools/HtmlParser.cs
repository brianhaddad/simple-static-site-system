using SSHPW.Classes;

namespace SSHPW.Tools
{
    public class HtmlParser : IHtmlParser
    {
        private readonly IHtmlNodeTreeBuilder _nodeTreeBuilder;
        private readonly IHtmlPreParser _preParser;

        public HtmlParser(IHtmlPreParser preParser, IHtmlNodeTreeBuilder nodeTreeBuilder)
        {
            _preParser = preParser ?? throw new ArgumentNullException(nameof(preParser));
            _nodeTreeBuilder = nodeTreeBuilder ?? throw new ArgumentNullException(nameof(nodeTreeBuilder));
        }

        public HtmlDocument Parse(string text)
        {
            var preparse = _preParser.GetParsedSymbols(text);
            return Build(preparse);
        }

        public HtmlDocument Parse(string[] lines)
        {
            var preparse = _preParser.GetParsedSymbols(lines);
            return Build(preparse);
        }

        private HtmlDocument Build(List<NodeParsingData> preparse)
        {
            _nodeTreeBuilder.SetData(preparse);
            var result = new HtmlDocument
            {
                RootNode = _nodeTreeBuilder.BuildNodeTree()
            };
            if (_nodeTreeBuilder.ContainsDocTypeDeclaration)
            {
                result.ContainsDocTypeDeclaration = true;
                result.DocTypeValues = _nodeTreeBuilder.DocTypeValues;
            }
            return result;
        }
    }
}