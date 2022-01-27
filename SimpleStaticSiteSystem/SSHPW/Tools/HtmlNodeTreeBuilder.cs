using SSHPW.Classes;
using SSHPW.Classes.Enums;
using SSHPW.Exceptions;

namespace SSHPW.Tools
{
    public class HtmlNodeTreeBuilder : IHtmlNodeTreeBuilder
    {
        private const string DOCTYPE = "!DOCTYPE";
        private readonly List<NodeParsingData> Data = new();
        private int _index = 0;

        public bool ContainsDocTypeDeclaration => Data.FirstOrDefault().TagName.ToUpper() == DOCTYPE;
        public List<string> DocTypeValues => Data.FirstOrDefault().Attributes.Select(x => x[0]).ToList();

        public void SetData(List<NodeParsingData> data)
        {
            Data.Clear();
            Data.AddRange(data);
            _index = 0;
            if (ContainsDocTypeDeclaration)
            {
                _index = 1;
            }
        }

        public HtmlNode BuildNodeTree() => Build(new HtmlNode());

        public HtmlNode BuildNodeTree(List<NodeParsingData> data)
        {
            Data.Clear();
            Data.AddRange(data);
            _index = 0;
            if (ContainsDocTypeDeclaration)
            {
                _index = 1;
            }
            return Build(new HtmlNode());
        }

        private HtmlNode Build(HtmlNode node)
        {
            var ppd = Data.ElementAt(_index);
            if (ppd is null || ppd.IsClosingTag || ppd.ParsedDataType != ParsingDataType.Tag)
            {
                throw new HtmlParsingErrorException("Unexpected error in the preparsed data or the node construction logic.");
            }
            node.TagName = ppd.TagName;
            if (ppd.Attributes is not null)
            {
                node.Attributes = BuildAttributes(ppd);
            }
            var waitForClosingTag = !ppd.IsSelfClosing;
            var indexOfOpenTag = _index;
            while (waitForClosingTag)
            {
                _index++;
                if (_index >= Data.Count)
                {
                    throw new HtmlParsingErrorException($"No closing tag found for {Data.ElementAt(indexOfOpenTag).TagName} tag.");
                }
                ppd = Data.ElementAt(_index);
                waitForClosingTag = !ppd.IsClosingTag || ppd.TagName != node.TagName;
                if (!waitForClosingTag)
                {
                    if (_index - indexOfOpenTag == 1)
                    {
                        node.ForceSeparateCloseTagForEmptyNode = true;
                    }
                    continue;
                }
                if (!ppd.IsSelfClosing && ppd.IsClosingTag)
                {
                    throw new HtmlParsingErrorException($"No opening tag found for {ppd.TagName} tag.");
                }
                else
                {
                    if (node.Children is null)
                    {
                        node.Children = new List<HtmlNode>();
                    }
                    if (ppd.ParsedDataType == ParsingDataType.Text)
                    {
                        node.Children.Add(new HtmlNode(ppd.Text));
                    }
                    if (ppd.ParsedDataType == ParsingDataType.Tag)
                    {
                        node.Children.Add(Build(new HtmlNode()));
                    }
                }
            }
            return node;
        }

        private List<HtmlNodeAttribute> BuildAttributes(NodeParsingData ppd)
        {
            var result = new List<HtmlNodeAttribute>();
            foreach (var attr in ppd.Attributes)
            {
                var value = attr.Length == 2 ? attr[1] : "";
                var quotes = false;
                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    quotes = true;
                    value = value.Substring(1, value.Length - 2);
                }
                result.Add(new HtmlNodeAttribute
                {
                    IsImplicitTrue = attr.Length == 1,
                    Name = attr[0],
                    QuotesAroundValue = quotes,
                    Value = value,
                });
            }
            return result;
        }
    }
}
