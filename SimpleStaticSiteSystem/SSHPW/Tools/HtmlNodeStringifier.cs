using SSHPW.Classes;
using SSHPW.Classes.Enums;
using SSHPW.Extensions;

namespace SSHPW.Tools
{
    public class HtmlNodeStringifier : IHtmlNodeStringifier
    {
        private readonly List<string> Lines = new();
        private readonly HtmlStringificationOptions Options;

        public HtmlNodeStringifier(HtmlStringificationOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public string[] Stringify(HtmlDocument htmlDoc)
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
            var indent = Indent(indentLevel);
            if (ContainsImmediateChildTextNode(node))
            {
                if (node.Children.Count == 1 && node.Children.First().IsMultilineTextOnlyNode)
                {
                    Lines.Add(indent + OpenTag(node));
                    var childLines = node.Children.First().Text.SplitByNewline();
                    var childIndent = Indent(indentLevel + 1);
                    foreach (var line in childLines)
                    {
                        Lines.Add(childIndent + line);
                    }
                    Lines.Add(indent + CloseTag(node));
                }
                else
                {
                    Lines.Add(indent + ProcessInlineNodes(node));
                }
            }
            else if (node.IsCommentNode)
            {
                Lines.Add(indent + CommentTag(node));
            }
            else if (node.IsSelfClosing)
            {
                Lines.Add(indent + SelfClosedTag(node));
            }
            else if (node.ForceSeparateCloseTagForEmptyNode)
            {
                Lines.Add(indent + ProcessInlineNodes(node));
            }
            else
            {
                Lines.Add(indent + OpenTag(node));
                foreach (var child in node.Children)
                {
                    Process(child, indentLevel + 1);
                }
                Lines.Add(indent + CloseTag(node));
            }
        }

        private string ProcessInlineNodes(HtmlNode node)
        {
            if (node.IsCommentNode)
            {
                return CommentTag(node);
            }
            if (node.IsSelfClosing)
            {
                return SelfClosedTag(node);
            }
            if (node.ForceSeparateCloseTagForEmptyNode)
            {
                return OpenTag(node) + CloseTag(node);
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

        private string CommentTag(HtmlNode node) => $"<!-- {node.Text} -->";

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

        private void AddDoctypeDeclaration(HtmlDocument htmlDoc)
        {
            if (htmlDoc.ContainsDocTypeDeclaration && htmlDoc.DocTypeValues != null && htmlDoc.DocTypeValues.Count > 0)
            {
                //Note: this doesn't currently support the entire doctype standard.
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
