using SSClasses;
using SSHFH;
using SSHPW.Extensions;
using SSSP.Exceptions;

namespace SSSP.Tools
{
    public class SuperSimpleTemplateBuilder : ISuperSimpleTemplateBuilder
    {
        private readonly ISuperSimpleHtmlFileHandler _fileHandler;

        public SuperSimpleTemplateBuilder(ISuperSimpleHtmlFileHandler fileHandler)
        {
            _fileHandler = fileHandler ?? throw new ArgumentNullException(nameof(fileHandler));
        }

        public void Build(StaticSiteProject project, string path)
        {
            var results = new List<HtmlFile>();

            foreach (var page in project.PageDefinitions)
            {
                if (!_fileHandler.FileExists(path + @"\Templates", page.PageLayoutTemplate))
                {
                    throw new BuildException($"{page.PageLayoutTemplate} template not found for page: {page.PageTitle}");
                }
                var pageHtml = new HtmlFile
                {
                    Path = path,
                    FileName = MakeFilename(page.PageTitle),
                    HtmlDocument = _fileHandler.ReadFile(path + @"\Templates", page.PageLayoutTemplate),
                };

                pageHtml.HtmlDocument.RootNode = PerformNodeReplacements(pageHtml.HtmlDocument.RootNode, page);

                results.Add(pageHtml);
            }

            foreach (var result in results)
            {
                _fileHandler.WriteFile(result);
            }
        }

        private HtmlNode PerformNodeReplacements(HtmlNode node, PageDefinition page)
        {
            if (node.TagName?.ToUpper() == "TITLE")
            {
                node.Children.Insert(0, new HtmlNode(page.PageTitle + " - "));
            }

            if (node.TagName == "text-replacement" && node.Attributes?.Count(x => x.Name == "key") == 1)
            {
                var newValue = node.Attributes.First(x => x.Name == "key").Value;
                return new HtmlNode(newValue);
            }

            if (node.Children?.Count() > 0)
            {
                for (var i = 0; i < node.Children.Count(); i++)
                {
                    node.Children[i] = PerformNodeReplacements(node.Children[i], page);
                }
            }
            return node;
        }

        private string MakeFilename(string title)
            => title.RegexReplace("[^a-zA-Z0-9]", "-").ToLower() + ".htm";
    }
}
