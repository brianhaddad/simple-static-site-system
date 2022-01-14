namespace SSHPW.Classes
{
    public class HtmlNode
    {
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<HtmlNode> Children { get; set; }
    }
}
