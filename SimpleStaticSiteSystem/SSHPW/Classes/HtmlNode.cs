using SSHPW.Extensions;

namespace SSHPW.Classes
{
    public class HtmlNode
    {
        public string TagName { get; set; }
        public List<HtmlNodeAttribute> Attributes { get; set; }
        public List<HtmlNode> Children { get; set; }
        public bool IsTextOnlyNode { get; private set; } = false;
        public bool IsMultilineTextOnlyNode { get; private set;} = false;
        public bool ForceSeparateCloseTagForEmptyNode { get; set; } = false;
        public string Text { get; private set; }
        public bool IsSelfClosing => !IsTextOnlyNode && !ForceSeparateCloseTagForEmptyNode && (Children is null || Children.Count == 0);

        public HtmlNode() { }

        public HtmlNode(string text)
        {
            Text = text;
            IsTextOnlyNode = true;
            IsMultilineTextOnlyNode = Text.ContainsNewline();
        }
    }
}
