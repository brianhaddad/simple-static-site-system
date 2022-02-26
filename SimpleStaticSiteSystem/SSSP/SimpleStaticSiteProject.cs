using SSClasses;
using SSHFH;
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

        private bool DirtyUnsavedFile = false;
        //TODO: allow wizard/user to define files that need to be written out
        //but delay writing the files until saved?
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
        public bool UnsavedChanges => DirtyUnsavedFile;
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
            if (DirtyUnsavedFile && !forceCreate)
            {
                //Attempting to create new when the class was already managing an existing and unsaved file.
                return FileActionResult.Failed("Dirty unsaved file.");
            }

            var illegalFileCharacters = Path.GetInvalidFileNameChars();
            var illegalPathCharacters = Path.GetInvalidPathChars();
            var badPath = illegalPathCharacters.Any(c => path.Contains(c));
            var badFileName = illegalFileCharacters.Any(c => projectName.Contains(c));
            var goodValues = !badPath && !badFileName;
            if (!goodValues)
            {
                var message = "Bad data!";
                if (badPath)
                {
                    var removePathCharacters = illegalPathCharacters.Where(c => path.Contains(c));
                    message += "\nThe path contains illegal characters. Please remove the following characters:";
                    message += "\n" + string.Join(", ", removePathCharacters);
                }
                if (badFileName)
                {
                    var removeFilenameCharacters = illegalFileCharacters.Where(c => projectName.Contains(c));
                    message += "\nThe file name contains illegal characters. Please remove the following characters:";
                    message += "\n" + string.Join(", ", removeFilenameCharacters);
                }
                return FileActionResult.Failed(message);
            }

            CurrentPath = path;
            CurrentFileName = projectName;
            CurrentProject = new StaticSiteProject();
            DirtyUnsavedFile = true;
            return FileActionResult.Successful();
        }

        public FileActionResult Open(string path, string projectName, bool forceOpen = false)
        {
            if (DirtyUnsavedFile && !forceOpen)
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
                //TODO: if the directory doesn't exist it's a firsttime create.
                //Need to create the folder structure in addition to saving the project file.
                _fileHandler.SaveObject(CurrentPath, FullProjectFilename, CurrentProject);
                DirtyUnsavedFile = false;
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
            DirtyUnsavedFile = true;
            return FileActionResult.Successful();
        }

        public FileActionResult AddPage(PageDefinition pageDefinition)
        {
            if (CurrentProject.PageDefinitions is null)
            {
                CurrentProject.PageDefinitions = new List<PageDefinition>();
            }
            CurrentProject.PageDefinitions.Add(pageDefinition);
            DirtyUnsavedFile = true;
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
            DirtyUnsavedFile = true;
            return FileActionResult.Successful();
        }
    }
}