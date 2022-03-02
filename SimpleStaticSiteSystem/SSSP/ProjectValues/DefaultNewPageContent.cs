using SSClasses;

namespace SSSP.ProjectValues
{
    public static class DefaultNewPageContent
    {
        public static HtmlDocument GetContent()
            => new()
            {
                ContainsDocTypeDeclaration = true,
                DocTypeValues = new List<string>
                {
                    "content",
                },
                RootNode = new HtmlNode()
                {
                    TagName = "main",
                    Children = new List<HtmlNode>
                    {
                        new HtmlNode()
                        {
                            TagName = "p",
                            Children = new List<HtmlNode>
                            {
                                HtmlContentTools.TextOnlyNode("Insert your content here "),
                                HtmlContentTools.TextReplacementNode("key", GlobalValueKeys.Author),
                                HtmlContentTools.TextOnlyNode("."),
                            },
                        },
                    },
                },
            };
    }
}
