namespace SSHPW.Classes
{
    public class ParsedHtmlNodeTree
    {
        public bool ContainsDocTypeDeclaration { get; set; }
        public List<string> DocTypeValues { get; set; }
        public HtmlNode RootNode { get; set; }
    }
}
