using SSClasses;
using SSHPW.Extensions;
using SSHPW.HttpValues;

namespace SSSP.ProjectValues
{
    public static class HtmlContentTools
    {
        public static HtmlNode TextOnlyNode(string text) => new(text);

        public static HtmlNode NodeReplacementNode(string key)
            => new()
            {
                TagName = CustomTagNames.NodeReplacement,
                Attributes = new List<HtmlNodeAttribute>
                {
                    new HtmlNodeAttribute
                    {
                        Name = key,
                        IsImplicitTrue = true,
                    },
                },
            };

        public static HtmlNode TextReplacementNode(string key, string value = null)
        {
            var nodeAttribute = value.IsNullEmptyOrWhiteSpace()
                ? new HtmlNodeAttribute
                {
                    Name = key,
                    IsImplicitTrue = true,
                }
                : new HtmlNodeAttribute
                {
                    Name = key,
                    Value = value,
                    QuotesAroundValue = true,
                };
            return new()
            {
                TagName = CustomTagNames.TextReplacement,
                Attributes = new List<HtmlNodeAttribute>
                {
                    nodeAttribute,
                },
            };
        }

        public static HtmlNode StyleSheetLink(string href)
            => new()
            {
                TagName = "link",
                Attributes = new List<HtmlNodeAttribute>
                {
                    new HtmlNodeAttribute
                    {
                        Name = "rel",
                        Value = "stylesheet",
                        QuotesAroundValue = true,
                    },
                    new HtmlNodeAttribute
                    {
                        Name = "href",
                        Value = href,
                        QuotesAroundValue = true,
                    },
                },
            };
    }
}
