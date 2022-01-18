namespace SSHPW.Classes
{
    public class HtmlNode : INode
    {
        public string TagName { get; set; }
        public List<HtmlNodeAttribute> Attributes { get; set; }
        public bool IsSelfClosing { get; set; }
        public List<INode> Children { get; set; }
    }
}
