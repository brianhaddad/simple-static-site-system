using SSClasses;
using SSHFH;
using SSSP.Classes;
using SSSP.Tools;

namespace SSSP
{
    public class SimpleStaticSiteProject : ISimpleStaticSiteProject
    {
        private readonly ISuperSimpleHtmlFileHandler _fileHandler;
        private readonly ISuperSimpleTemplateBuilder _templatebuilder;
        private string CurrentFileName = "";
        private string CurrentPath = "";
        private StaticSiteProject CurrentProject;

        private bool DirtyUnsavedFile = false;

        private const string ProjectFileExtension = ".ssc";
        private string FullProjectFilename
            => CurrentFileName.EndsWith(ProjectFileExtension)
                ? CurrentFileName
                : CurrentFileName + ProjectFileExtension;

        public SimpleStaticSiteProject(ISuperSimpleHtmlFileHandler fileHandler)
        {
            _fileHandler = fileHandler ?? throw new ArgumentNullException(nameof(fileHandler));
            _templatebuilder = new SuperSimpleTemplateBuilder(fileHandler);
        }

        //TODO: maybe some kind of eventing and a way to register methods to run as event handlers?
        //When the DirtyUnsavedFile value changes, some things will need to be run in the interface
        //to enable or disable buttons.
        public bool UnsavedChanges => DirtyUnsavedFile;

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
                _fileHandler.SaveObject(CurrentPath, FullProjectFilename, CurrentProject);
                DirtyUnsavedFile = false;
                return FileActionResult.Successful();
            }
            catch (Exception ex)
            {
                return FileActionResult.Failed($"Could not save {CurrentPath + FullProjectFilename}. {ex.Message}");
            }
        }

        public FileActionResult AddGlobalProjectValue(string key, string value)
        {
            if (CurrentProject.GlobalProjectValues is null)
            {
                CurrentProject.GlobalProjectValues = new Dictionary<string, string>();
            }
            if (CurrentProject.GlobalProjectValues.ContainsKey(key))
            {
                return FileActionResult.Failed($"Variable value for '{key}' already set.");
            }
            CurrentProject.GlobalProjectValues.Add(key, value);
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

        public FileActionResult AddBuildTarget(string env, string baseUrl)
        {
            if (CurrentProject.SiteBuildTargets is null)
            {
                CurrentProject.SiteBuildTargets = new Dictionary<string, string>();
            }
            if (CurrentProject.SiteBuildTargets.ContainsKey(env))
            {
                return FileActionResult.Failed($"Environment data for '{env}' already exists.");
            }
            CurrentProject.SiteBuildTargets.Add(env, baseUrl);
            DirtyUnsavedFile = true;
            return FileActionResult.Successful();
        }
    }
}