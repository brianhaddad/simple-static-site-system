using SSClasses;
using SSHFH;
using SSHPW.Extensions;
using SSHPW.HttpValues;
using SSSP.Exceptions;
using SSSP.ProjectValues;

namespace SSSP.Tools
{
    public class SuperSimpleTemplateBuilder : ISuperSimpleTemplateBuilder
    {
        private readonly ISuperSimpleHtmlFileHandler _fileHandler;
        //TODO: this file has a lot of random strings scattered throughout
        //Some of these values will be required in another class probably...
        private const string WEB_PATH_SEPARATOR = @"/";

        private string currentBuildPath = "";
        private string currentBaseUrl = "";
        private string currentEnv = "";
        private HtmlNode nav;

        private string FullBuildToPath => Path.Combine(currentBuildPath, ProjectFolders.Build, currentEnv);
        private string TemplateSourcePath => Path.Combine(currentBuildPath, ProjectFolders.Templates);
        private string ContentSourcePath => Path.Combine(currentBuildPath, ProjectFolders.Content);
        private string SnippetsSourcePath => Path.Combine(currentBuildPath, ProjectFolders.Snippets);
        private string StylesSourcePath => Path.Combine(currentBuildPath, ProjectFolders.Styles);

        public SuperSimpleTemplateBuilder(ISuperSimpleHtmlFileHandler fileHandler)
        {
            _fileHandler = fileHandler ?? throw new ArgumentNullException(nameof(fileHandler));
        }

        public void Build(StaticSiteProject project, string path, string env)
        {
            currentBuildPath = path;
            currentEnv = env;
            if (!project.SiteBuildTargets.ContainsKey(currentEnv))
            {
                throw new BuildException($"No build data for '{currentEnv}' environment.");
            }

            _fileHandler.RemoveDirectoryIfExists(FullBuildToPath);

            currentBaseUrl = project.SiteBuildTargets[currentEnv];
            var results = new List<HtmlFile>();

            //TODO: nav can be shared between pages now due to decision to implement absolute URLs,
            //but what if we want to indicate in the menu what page the visitor is on?
            //In fact, do we need a larger system to add situational modifications to highlight components?
            //This could be used by the tool to highlight a panel being edited, for example.
            var navFile = _fileHandler.ReadFile(TemplateSourcePath, "Nav" + ProjectFileTypes.TemplateFileType); //TODO: always be named Nav?
            var sortedPageDefinitions = project.PageDefinitions.OrderBy(x => x.NavMenuSortIndex).ToList();
            var navProject = new StaticSiteProject
            {
                GlobalProjectValues = project.GlobalProjectValues,
                PageDefinitions = sortedPageDefinitions,
            };
            nav = PerformBuildActions(navFile.RootNode, navProject, new PageDefinition());
            
            foreach (var page in project.PageDefinitions)
            {
                if (!_fileHandler.FileExists(TemplateSourcePath, page.PageLayoutTemplate))
                {
                    throw new BuildException($"{page.PageLayoutTemplate} template not found for page: {page.PageTitle}");
                }
                var fullPath = FullBuildToPath;
                if (page.PageSubdirectory is not null)
                {
                    fullPath = Path.Combine(fullPath, PathText(page.PageSubdirectory));
                }
                var pageHtml = new HtmlFile
                {
                    Path = fullPath,
                    FileName = page.IsIndex ? ProjectFileTypes.IndexHtm : MakeFilename(page.FileName, ProjectFileTypes.Htm),
                    HtmlDocument = _fileHandler.ReadFile(TemplateSourcePath, page.PageLayoutTemplate),
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
            if (node.TagName?.ToUpper() == "LINK")
            {
                var rel = node.Attributes?.FirstOrDefault(a => a.Name.ToUpper() == "REL");
                if (rel is not null && rel.Value.ToUpper().Contains("STYLESHEET"))
                {
                    var href = node.Attributes.FirstOrDefault(a => a.Name.ToUpper() == "HREF");
                    if (href is not null)
                    {
                        //TODO: this system won't handle external style sheets that are linked online.
                        //Need a way to check the current value to make sure it's one we should translate.
                        var filename = href.Value;
                        var stylesPath = ProjectFolders.Styles;
                        var hrefPath = currentBaseUrl + WEB_PATH_SEPARATOR + stylesPath + WEB_PATH_SEPARATOR + filename;
                        node.Attributes[node.Attributes.IndexOf(href)].Value = hrefPath;
                        var fileAlreadyCopiedToOutput = _fileHandler.FileExists(Path.Combine(FullBuildToPath, stylesPath), filename);
                        if (!fileAlreadyCopiedToOutput &&
                            !_fileHandler.CopyFile(filename, StylesSourcePath, Path.Combine(FullBuildToPath, stylesPath)))
                        {
                            throw new BuildException($"Could not copy file: {filename}");
                        }
                        //TODO: need to scan the CSS file and replace global values like colors and stuff for the project
                    }
                }
            }

            if (node.TagName?.ToUpper() == CustomTagNames.TextReplacement.ToUpper())
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
                if (node.Attributes?.Count(x => x.Name.ToUpper() == ReplacementKeys.PageTitle.ToUpper()) == node.Attributes?.Count)
                {
                    return new HtmlNode(page.PageTitle);
                }
                if (node.Attributes?.Count(x => x.Name.ToUpper() == ReplacementKeys.Year.ToUpper()) == node.Attributes?.Count)
                {
                    var date = DateTime.Now;
                    return new HtmlNode(date.Year.ToString());
                }
                if (node.Attributes?.Count(x => x.Name.ToUpper() == ReplacementKeys.NodeDirectory.ToUpper()) == node.Attributes?.Count)
                {
                    var display = page.PageSubdirectory.Contains("/")
                        ? page.PageSubdirectory.Substring(page.PageSubdirectory.LastIndexOf("/"))
                        : page.PageSubdirectory;
                    return new HtmlNode(display);
                }
            }

            if (node.TagName?.ToUpper() == CustomTagNames.NodeReplacement.ToUpper()
                || node.Attributes?.Count(x => x.Name.ToUpper() == ReplacementKeys.NodeReplacement.ToUpper()) == 1)
            {
                if (node.Attributes?.Count(x => x.Name.ToUpper() == ReplacementKeys.PageContent.ToUpper()) == node.Attributes?.Count)
                {
                    var content = _fileHandler.ReadFile(ContentSourcePath, MakeFilename(page.FileName, ProjectFileTypes.ContentFileType));
                    return PerformBuildActions(content.RootNode, project, page);
                }

                if (node.Attributes?.Count(x => x.Name.ToUpper() == ReplacementKeys.Nav.ToUpper()) == node.Attributes?.Count)
                {
                    return nav;
                }

                if (node.Attributes?.Count(x => x.Name.ToUpper() == ReplacementKeys.PageLink.ToUpper()) == node.Attributes?.Count)
                {
                    var hrefPath = currentBaseUrl + WEB_PATH_SEPARATOR;
                    if (page.PageSubdirectory is not null && page.PageSubdirectory.Length > 0)
                    {
                        hrefPath += PathText(page.PageSubdirectory) + WEB_PATH_SEPARATOR;
                    }
                    hrefPath += page.IsIndex ? ProjectFileTypes.IndexHtm : MakeFilename(page.FileName, ProjectFileTypes.Htm);
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

                if (node.Attributes?.Count(x => x.Name.ToUpper() == CustomTagNames.NodeReplacement.ToUpper()) == 1
                    && node.Children?.Count(c => c.TagName?.ToUpper() == CustomTagNames.NavLinks.ToUpper()) == node.Children?.Count)
                {
                    //TODO: test this with a more complex folder structure
                    //TODO: take care of some of the remaining magic strings
                    var attribute = node.Attributes.First(x => x.Name.ToUpper() == CustomTagNames.NodeReplacement.ToUpper());
                    node.Attributes.Remove(attribute);
                    var baseLinkSnip = "Nav_link" + ProjectFileTypes.SnippetFileType;
                    var menuData = node.Children.First().Attributes.ToArray();
                    var canNest = menuData.Any(x => x.Name == "nesting" && x.IsImplicitTrue);
                    node.Children.Clear();
                    var completed = new List<PageDefinition>();
                    var currentSubDirectory = project.PageDefinitions.ElementAt(0).PageSubdirectory ?? "";
                    foreach (var p in project.PageDefinitions)
                    {
                        if (!completed.Contains(p))
                        {
                            var thisSubdirectory = p.PageSubdirectory ?? "";
                            if (canNest && thisSubdirectory.Length > currentSubDirectory.Length)
                            {
                                //TODO: this file contains a couple issues
                                //see notes in Nav_folder.shs for more details
                                //already created IValueProducer to solve this problem
                                var folderFile = _fileHandler.ReadFile(SnippetsSourcePath, "Nav_folder" + ProjectFileTypes.SnippetFileType);
                                var filtered = project.PageDefinitions
                                    .Where(x => x.PageSubdirectory?.StartsWith(p.PageSubdirectory) ?? false)
                                    .OrderBy(x => x.NavMenuSortIndex)
                                    .ToList();
                                var navProject = new StaticSiteProject
                                {
                                    GlobalProjectValues = project.GlobalProjectValues,
                                    PageDefinitions = filtered,
                                };
                                completed.AddRange(filtered);
                                var f = PerformBuildActions(folderFile.RootNode, navProject, p);
                                node.Children.Add(f);
                            }
                            else
                            {
                                var childSnip = _fileHandler.ReadFile(SnippetsSourcePath, baseLinkSnip);
                                var c = PerformBuildActions(childSnip.RootNode, project, p);
                                node.Children.Add(c);
                            }
                        }
                    }
                    return node;
                }

                return new HtmlNode($" {node.TagName} with {node.Attributes?.FirstOrDefault().Name} value was unhandled. ", true);
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

        private string MakeFilename(string title, string extension) => title + extension;

        private string PathText(string text) => text.RegexReplace(@"[^a-zA-Z0-9\/]", "");
    }
}
