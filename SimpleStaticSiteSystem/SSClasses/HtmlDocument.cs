namespace SSClasses
{
    public class HtmlDocument
    {
        public bool ContainsDocTypeDeclaration { get; set; }
        public List<string> DocTypeValues { get; set; }
        public HtmlNode RootNode { get; set; }
    }
}
