using SSHPW.Classes;
using SSHPW.Classes.Enums;

namespace SSHPW
{
    public class HtmlNodeStringifier
    {
        private readonly List<string> Lines = new List<string>();
        private readonly HtmlStringificationOptions Options;

        public HtmlNodeStringifier(HtmlStringificationOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public string[] Stringify(ParsedHtmlNodeTree htmlDoc)
        {
            AddDoctypeDeclaration(htmlDoc);
            return Stringify(htmlDoc.RootNode);
        }

        public string[] Stringify(HtmlNode htmlNode)
        {
            var indentLevel = 0;
            Process(htmlNode, indentLevel);
            var result = Lines.ToArray();
            Lines.Clear();
            return result;
        }

        private void Process(HtmlNode node, int indentLevel)
        {
            //TODO: have to do the thing for the current node at the current indent level
            if (ContainsImmediateChildTextNode(node))
            {
                Lines.Add(Indent(indentLevel) + ProcessInlineNodes(node));
            }
            else if (node.IsSelfClosing)
            {
                Lines.Add(Indent(indentLevel) + SelfClosedTag(node));
            }
            else
            {
                Lines.Add(Indent(indentLevel) + OpenTag(node));
                foreach (var child in node.Children)
                {
                    //TODO: this is just the basic idea, not the end goal!
                    Process(child, indentLevel + 1);
                }
                Lines.Add(Indent(indentLevel) + CloseTag(node));
            }
        }

        private string ProcessInlineNodes(HtmlNode node)
        {
            if (node.IsSelfClosing)
            {
                return SelfClosedTag(node);
            }
            if (node.IsTextOnlyNode)
            {
                return node.Text;
            }
            var children = node.Children.Select(x => ProcessInlineNodes(x)).ToArray().Join("");
            return $"{OpenTag(node)}{children}{CloseTag(node)}";
        }

        private string SelfClosedTag(HtmlNode node)
            => $"<{Casify(node.TagName)}{AttributesString(node.Attributes)} />";

        private string OpenTag(HtmlNode node)
            => $"<{Casify(node.TagName)}{AttributesString(node.Attributes)}>";

        private string AttributesString(List<HtmlNodeAttribute> attributes)
        {
            if (attributes is null || attributes.Count == 0)
            {
                return "";
            }
            return " " + attributes.Select(x => AttributeString(x)).ToArray().Join(" ");
        }

        private string AttributeString(HtmlNodeAttribute attribute)
        {
            if (attribute.IsImplicitTrue)
            {
                return attribute.Name;
            }
            var result = attribute.Name + "=";
            if (attribute.QuotesAroundValue)
            {
                return result += $"\"{attribute.Value}\"";
            }
            return result + attribute.Value;
        }

        private string CloseTag(HtmlNode node) => $"</{Casify(node.TagName)}>";

        private void AddDoctypeDeclaration(ParsedHtmlNodeTree htmlDoc)
        {
            if (htmlDoc.ContainsDocTypeDeclaration && htmlDoc.DocTypeValues != null && htmlDoc.DocTypeValues.Count > 0)
            {
                //TODO: this doesn't currently support the entire doctype standard.
                var tag = $"<!DOCTYPE {htmlDoc.DocTypeValues.Join(" ")}>";
                Lines.Add(tag);
            }
        }

        private bool ContainsImmediateChildTextNode(HtmlNode htmlNode) => htmlNode.Children?.Any(x => x.IsTextOnlyNode) ?? false;
        private string Casify(string text)
            => Options.TagCaseBehavior switch
            {
                TagCaseOptions.LowerCase => text.ToLower(),
                TagCaseOptions.UpperCase => text.ToUpper(),
                TagCaseOptions.DoNotAlter => text,
                _ => throw new NotImplementedException($"No behavior defined for {Options.TagCaseBehavior}"),
            };

        private string Indent(int indentLevel) => Options.IndentString.Repeat(indentLevel);
    }
}
