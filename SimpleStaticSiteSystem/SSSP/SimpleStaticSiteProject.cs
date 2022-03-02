using SSClasses;
using SSHFH;
using SSHPW.Extensions;
using SSSP.Classes;
using SSSP.ProjectValues;
using SSSP.Tools;

namespace SSSP
{
    public class SimpleStaticSiteProject : ISimpleStaticSiteProject
    {
        public string UserSelectedFolderLocation { get; set; }
        public string UserSelectedFileName { get; set; }

        private readonly ISuperSimpleHtmlFileHandler _fileHandler;
        private readonly ISuperSimpleTemplateBuilder _templatebuilder;
        private string CurrentFileName = "";
        private string CurrentPath = "";
        private StaticSiteProject CurrentProject;

        private bool dirtyUnsavedFile = false;
        
        private List<HtmlFile> unwrittenHtmlFiles = new();

        private string FullProjectFilename
            => CurrentFileName.EndsWith(ProjectFileTypes.ProjectConfigType)
                ? CurrentFileName
                : CurrentFileName + ProjectFileTypes.ProjectConfigType;

        public SimpleStaticSiteProject(ISuperSimpleHtmlFileHandler fileHandler)
        {
            _fileHandler = fileHandler ?? throw new ArgumentNullException(nameof(fileHandler));
            _templatebuilder = new SuperSimpleTemplateBuilder(fileHandler);
        }

        //TODO: maybe some kind of eventing and a way to register methods to run as event handlers?
        //When the DirtyUnsavedFile value changes, some things will need to be run in the interface
        //to enable or disable buttons.
        public bool UnsavedChanges => dirtyUnsavedFile;
        public Dictionary<string, string> GlobalProjectValues => CurrentProject?.GlobalProjectValues ?? new();

        public FileActionResult Compile(string env)
        {
            try
            {
                _templatebuilder.Build(CurrentProject, CurrentPath, env);
                return FileActionResult.Successful();
            }
            catch (Exception ex)
            {
                return FileActionResult.Failed($"Unable to compile. {ex.Message}");
            }
        }

        public FileActionResult CreateNew(string path, string projectName, bool forceCreate = false)
        {
            //TODO: also check to see if file already exists?
            if (dirtyUnsavedFile && !forceCreate)
            {
                // Attempting to create new when the class was already managing an existing and unsaved file.
                return FileActionResult.Failed("Dirty unsaved file.");
            }

            path = Path.Combine(path, projectName);

            var badPath = path.IsInvalidFilePath();
            var badFileName = projectName.IsInvalidFileName();
            var goodValues = !badPath && !badFileName;
            if (!goodValues)
            {
                var message = "Bad data!";
                if (badPath)
                {
                    var removePathCharacters = Path.GetInvalidPathChars().Where(c => path.Contains(c));
                    message += "\nThe path contains illegal characters. Please remove the following characters:";
                    message += "\n" + string.Join(", ", removePathCharacters);
                }
                if (badFileName)
                {
                    var removeFilenameCharacters = Path.GetInvalidFileNameChars().Where(c => projectName.Contains(c));
                    message += "\nThe file name contains illegal characters. Please remove the following characters:";
                    message += "\n" + string.Join(", ", removeFilenameCharacters);
                }
                return FileActionResult.Failed(message);
            }

            CurrentPath = path;
            CurrentFileName = projectName;
            CurrentProject = new StaticSiteProject();
            dirtyUnsavedFile = true;
            return FileActionResult.Successful();
        }

        public FileActionResult Open(string path, string projectName, bool forceOpen = false)
        {
            if (dirtyUnsavedFile && !forceOpen)
            {
                return FileActionResult.Failed("Dirty unsaved file.");
            }
            CurrentPath = path;
            CurrentFileName = projectName;
            try
            {
                CurrentProject = _fileHandler.ReadObject<StaticSiteProject>(CurrentPath, FullProjectFilename);
                return FileActionResult.Successful();
            }
            catch (Exception ex)
            {
                return FileActionResult.Failed($"Could not open {CurrentPath + FullProjectFilename}. {ex.Message}");
            }
        }

        public FileActionResult Save()
        {
            try
            {
                _fileHandler.SaveObject(CurrentPath, FullProjectFilename, CurrentProject);
                _fileHandler.CreateDirectory(Path.Combine(CurrentPath, ProjectFolders.Content));
                _fileHandler.CreateDirectory(Path.Combine(CurrentPath, ProjectFolders.Snippets));
                _fileHandler.CreateDirectory(Path.Combine(CurrentPath, ProjectFolders.Styles));
                _fileHandler.CreateDirectory(Path.Combine(CurrentPath, ProjectFolders.Templates));
                foreach (var file in unwrittenHtmlFiles)
                {
                    _fileHandler.WriteFile(file);
                }
                unwrittenHtmlFiles.Clear();
                dirtyUnsavedFile = false;
                return FileActionResult.Successful();
            }
            catch (Exception ex)
            {
                return FileActionResult.Failed($"Could not save {CurrentPath + FullProjectFilename}. {ex.Message}");
            }
        }

        public FileActionResult SetGlobalProjectValue(string key, string value)
        {
            if (CurrentProject.GlobalProjectValues is null)
            {
                CurrentProject.GlobalProjectValues = new Dictionary<string, string>();
            }
            if (CurrentProject.GlobalProjectValues.ContainsKey(key))
            {
                CurrentProject.GlobalProjectValues[key] = value;
            }
            else
            {
                CurrentProject.GlobalProjectValues.Add(key, value);
            }
            dirtyUnsavedFile = true;
            return FileActionResult.Successful();
        }

        public FileActionResult AddPage(PageDefinition pageDefinition)
        {
            if (CurrentProject.PageDefinitions is null)
            {
                CurrentProject.PageDefinitions = new List<PageDefinition>();
            }
            if (pageDefinition.FileName.IsNullEmptyOrWhiteSpace())
            {
                pageDefinition.FileName = pageDefinition.PageTitle.RegexReplace("[^a-zA-Z0-9]", "-");
            }
            //TODO: need to make sure this isn't a duplicate page.
            //If duplicate, remove old and replace with new or just throw error?
            //Also, the wizard should only be allowed to add one page, so maybe just clear out the page definition?
            //But that would leave an orphaned content file, so that needs to be cleaned up too...
            //But also the wizard hasn't written those files yet, so all we need to do is remove them from the
            //unwritten files list...
            CurrentProject.PageDefinitions.Add(pageDefinition);
            dirtyUnsavedFile = true;

            var templatePath = Path.Combine(CurrentPath, ProjectFolders.Templates);
            if (!_fileHandler.FileExists(templatePath, pageDefinition.PageLayoutTemplate)
                && !unwrittenHtmlFiles.Any(x => x.Path == templatePath && x.FileName == pageDefinition.PageLayoutTemplate))
            {
                var newTemplate = new HtmlFile
                {
                    HtmlDocument = DefaultPageTemplate.GetDocument(),
                    FileName = pageDefinition.PageLayoutTemplate,
                    Path = templatePath,
                };
                unwrittenHtmlFiles.Add(newTemplate);
                //TODO: if the file references any CSS files we need to handle those.
                //Thoughts: can we keep those as embedded resource files and just copy them into place?
                //Also the CSS stuff isn't finished.

                //TODO: also need to write out the nav snippets... Also those aren't quite finished.
            }
            // Add the page's content .shc file to the content folder via the unwritten HTML files list.
            var newContent = new HtmlFile
            {
                HtmlDocument = DefaultNewPageContent.GetContent(),
                FileName = pageDefinition.FileName + ProjectFileTypes.ContentFileType,
                Path = Path.Combine(CurrentPath, ProjectFolders.Content),
            };
            unwrittenHtmlFiles.Add(newContent);
            return FileActionResult.Successful();
        }

        public FileActionResult AddTemplate(HtmlFile template)
        {
            throw new NotImplementedException();
        }

        public FileActionResult AddContent(HtmlFile content)
        {
            throw new NotImplementedException();
        }

        public FileActionResult AddSnippet(HtmlFile snippet)
        {
            throw new NotImplementedException();
        }

        public FileActionResult AddBuildTarget(string env, string baseUrl)
        {
            if (CurrentProject.SiteBuildTargets is null)
            {
                CurrentProject.SiteBuildTargets = new Dictionary<string, string>();
            }
            if (CurrentProject.SiteBuildTargets.ContainsKey(env))
            {
                //TODO: might actually want to let the value be updated...
                //Maybe another force update bool value for the method?
                return FileActionResult.Failed($"Environment data for '{env}' already exists.");
            }
            CurrentProject.SiteBuildTargets.Add(env, baseUrl);
            dirtyUnsavedFile = true;
            return FileActionResult.Successful();
        }
    }
}