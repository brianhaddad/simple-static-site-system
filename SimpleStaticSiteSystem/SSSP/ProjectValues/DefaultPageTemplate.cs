using SSClasses;

namespace SSSP.ProjectValues
{
    //TODO: how many of the tag names and stuff here belong in global values?
    public static class DefaultPageTemplate
    {
        public static HtmlDocument GetDocument()
        {
            var document = new HtmlDocument
            {
                ContainsDocTypeDeclaration = true,
                DocTypeValues = new List<string>
                {
                    "template",
                },
                RootNode = new HtmlNode
                {
                    TagName = "html",
                    Children = new List<HtmlNode>
                    {
                        new HtmlNode
                        {
                            TagName = "head",
                            Children = new List<HtmlNode>
                            {
                                new HtmlNode
                                {
                                    TagName = "title",
                                    Children = new List<HtmlNode>
                                    {
                                        HtmlContentTools.TextReplacementNode(ReplacementKeys.PageTitle),
                                        HtmlContentTools.TextOnlyNode(" - "),
                                        HtmlContentTools.TextReplacementNode("key", GlobalValueKeys.SiteTitle),
                                    },
                                },
                                HtmlContentTools.StyleSheetLink("normalize.css"), //TODO: do these file names belong in a global as well?
                                HtmlContentTools.StyleSheetLink("main.css"), //or can we just automatically handle them?
                            },
                        },
                        new HtmlNode
                        {
                            TagName = "body",
                            Children = new List<HtmlNode>
                            {
                                new HtmlNode
                                {
                                    TagName = "header",
                                    Children = new List<HtmlNode>
                                    {
                                        new HtmlNode
                                        {
                                            TagName = "h1",
                                            Children = new List<HtmlNode>
                                            {
                                                HtmlContentTools.TextReplacementNode(ReplacementKeys.PageTitle),
                                            },
                                        },
                                    },
                                },
                                HtmlContentTools.NodeReplacementNode(ReplacementKeys.Nav),
                                HtmlContentTools.NodeReplacementNode(ReplacementKeys.PageContent),
                                new HtmlNode
                                {
                                    TagName = "footer",
                                    Children = new List<HtmlNode>
                                    {
                                        new HtmlNode
                                        {
                                            TagName = "p",
                                            Children = new List<HtmlNode>
                                            {
                                                HtmlContentTools.TextOnlyNode("&copy; "),
                                                HtmlContentTools.TextReplacementNode(ReplacementKeys.Year),
                                                HtmlContentTools.TextOnlyNode(" - "),
                                                HtmlContentTools.TextReplacementNode("key", GlobalValueKeys.Author),
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            };
            return document;
        }
    }
}
