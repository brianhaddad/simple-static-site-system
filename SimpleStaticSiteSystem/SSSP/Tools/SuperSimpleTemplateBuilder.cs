﻿using SSClasses;
using SSHFH;
using SSHPW.Extensions;
using SSSP.Exceptions;

namespace SSSP.Tools
{
    public class SuperSimpleTemplateBuilder : ISuperSimpleTemplateBuilder
    {
        private readonly ISuperSimpleHtmlFileHandler _fileHandler;

        private const string BUILD_SUBDIR = @"\build";
        private const string PATH_SEPARATOR = @"\";

        private string currentBuildPath = "";
        private HtmlNode nav;

        private string TemplatePath => currentBuildPath + PATH_SEPARATOR + "templates";
        private string ContentPath => currentBuildPath + PATH_SEPARATOR + "content";
        private string SnippetsPath => currentBuildPath + PATH_SEPARATOR + "snippets";

        public SuperSimpleTemplateBuilder(ISuperSimpleHtmlFileHandler fileHandler)
        {
            _fileHandler = fileHandler ?? throw new ArgumentNullException(nameof(fileHandler));
        }

        public void Build(StaticSiteProject project, string path)
        {
            currentBuildPath = path;
            var results = new List<HtmlFile>();

            var navFile = _fileHandler.ReadFile(TemplatePath, "Nav.sht");
            nav = PerformBuildActions(navFile.RootNode, project, new PageDefinition());
            
            foreach (var page in project.PageDefinitions)
            {
                if (!_fileHandler.FileExists(TemplatePath, page.PageLayoutTemplate))
                {
                    throw new BuildException($"{page.PageLayoutTemplate} template not found for page: {page.PageTitle}");
                }
                var fullPath = currentBuildPath + BUILD_SUBDIR;
                if (page.PageSubdirectory is not null)
                {
                    fullPath += PATH_SEPARATOR + page.PageSubdirectory;
                }
                var pageHtml = new HtmlFile
                {
                    Path = fullPath,
                    FileName = page.IsIndex ? "index.htm" : MakeFilename(page.PageTitle) + ".htm",
                    HtmlDocument = _fileHandler.ReadFile(TemplatePath, page.PageLayoutTemplate),
                };

                pageHtml.HtmlDocument.RootNode = PerformBuildActions(pageHtml.HtmlDocument.RootNode, project, page);

                results.Add(pageHtml);
            }

            foreach (var result in results)
            {
                _fileHandler.WriteFile(result);
            }
        }

        private HtmlNode PerformBuildActions(HtmlNode node, StaticSiteProject project, PageDefinition page)
        {
            if (node.TagName?.ToUpper() == "TITLE")
            {
                node.Children.Insert(0, new HtmlNode(page.PageTitle + " - "));
            }

            if (node.TagName?.ToUpper() == "LINK")
            {
                var rel = node.Attributes?.FirstOrDefault(a => a.Name.ToUpper() == "REL");
                if (rel is not null && rel.Value.ToUpper().Contains("STYLESHEET"))
                {
                    var href = node.Attributes.FirstOrDefault(a => a.Name.ToUpper() == "HREF");
                    if (href is not null)
                    {
                        var filename = href.Value;
                        var stylesPath = "styles";
                        var hrefPath = stylesPath + "/" + filename;
                        if (page.PageSubdirectory is not null && page.PageSubdirectory.Length > 0)
                        {
                            //TODO: verify this works!
                            node.Attributes[node.Attributes.IndexOf(href)].Value = "../" + hrefPath;
                        }
                        else
                        {
                            node.Attributes[node.Attributes.IndexOf(href)].Value = hrefPath;
                        }
                        var requiredFileExists = _fileHandler.FileExists(currentBuildPath + BUILD_SUBDIR + PATH_SEPARATOR + stylesPath, filename);
                        if (!requiredFileExists &&
                            !_fileHandler.CopyFile(filename, currentBuildPath + PATH_SEPARATOR + stylesPath, currentBuildPath + BUILD_SUBDIR + PATH_SEPARATOR + stylesPath))
                        {
                            throw new BuildException($"Could not copy file: {filename}");
                        }
                        //TODO: need to scan the CSS file and replace global values like colors and stuff for the project
                    }
                }
            }

            if (node.TagName?.ToUpper() == "TEXT-REPLACEMENT")
            {
                if (node.Attributes?.Count(x => x.Name == "key") == node.Attributes?.Count)
                {
                    var globalKey = node.Attributes.First(x => x.Name == "key").Value;
                    var newValue = project.GlobalProjectValues.GetValueOrDefault(globalKey);
                    if (newValue is null)
                    {
                        throw new BuildException($"Unable to find global value for key: {globalKey}");
                    }
                    return new HtmlNode(newValue);
                }
                if (node.Attributes?.Count(x => x.Name == "page-title") == node.Attributes?.Count)
                {
                    return new HtmlNode(page.PageTitle);
                }
                if (node.Attributes?.Count(x => x.Name == "year") == node.Attributes?.Count)
                {
                    var date = DateTime.Now;
                    return new HtmlNode(date.Year.ToString());
                }
            }

            if (node.TagName?.ToUpper() == "NODE-REPLACEMENT" || node.Attributes?.Count(x => x.Name == "node-replacement") == 1)
            {
                if (node.Attributes?.Count(x => x.Name == "page-content") == node.Attributes?.Count)
                {
                    var content = _fileHandler.ReadFile(ContentPath, MakeFilename(page.PageTitle) + ".shc");
                    return PerformBuildActions(content.RootNode, project, page);
                }

                if (node.Attributes?.Count(x => x.Name == "Nav") == node.Attributes?.Count)
                {
                    return nav;
                }

                if (node.Attributes?.Count(x => x.Name == "page-link") == node.Attributes?.Count)
                {
                    var hrefPath = page.IsIndex ? "index.htm" : MakeFilename(page.PageTitle) + ".htm";
                    if (page.PageSubdirectory is not null && page.PageSubdirectory.Length > 0)
                    {
                        hrefPath = page.PageSubdirectory + "/" + hrefPath;
                    }
                    var href = new HtmlNodeAttribute
                    {
                        Name = "href",
                        Value = hrefPath,
                        QuotesAroundValue = true,
                    };
                    var displayText = new HtmlNode(page.PageTitle);
                    var linkNode = new HtmlNode
                    {
                        TagName = "A",
                        Attributes = new List<HtmlNodeAttribute> { href },
                        Children = new List<HtmlNode> { displayText },
                    };
                    return linkNode;
                }

                if (node.Attributes?.Count(x => x.Name == "node-replacement") == 1
                    && node.Children?.Count(c => c.TagName?.ToUpper() == "NAV-LINKS") == node.Children?.Count)
                {
                    //TODO: this needs to handle grouping by subdirectory and building up the node tree for the nav
                    var attribute = node.Attributes.First(x => x.Name == "node-replacement");
                    node.Attributes.Remove(attribute);
                    var baseLinkSnip = "Nav_links.shs";
                    var menuData = node.Children.First().Attributes.ToArray();
                    var canNest = menuData.Any(x => x.Name == "nesting" && x.IsImplicitTrue);
                    node.Children.Clear();
                    foreach (var p in project.PageDefinitions)
                    {
                        if (canNest && p.PageSubdirectory is not null && p.PageSubdirectory.Length > 0)
                        {
                            //TODO: nest.
                            //But... will need to make sure pages in same directory end up in same directory...
                        }
                        var childSnip = _fileHandler.ReadFile(SnippetsPath, baseLinkSnip);
                        var c = PerformBuildActions(childSnip.RootNode, project, p);
                        node.Children.Add(c);
                    }
                    return node;
                }

                //TODO: this is temporary. Eventually should probably throw a build error if we get this far.
                return new HtmlNode
                {
                    TagName = node.Attributes?.FirstOrDefault().Name,
                    ForceSeparateCloseTagForEmptyNode = true,
                };
            }

            if (node.Children?.Count > 0)
            {
                for (var i = 0; i < node.Children.Count; i++)
                {
                    node.Children[i] = PerformBuildActions(node.Children[i], project, page);
                }
            }
            return node;
        }

        private string MakeFilename(string title)
            => title.RegexReplace("[^a-zA-Z0-9]", "-").ToLower();
    }
}
