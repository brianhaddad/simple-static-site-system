using SSClasses;
using SSSP.Classes;

namespace SSSP
{
    public interface ISimpleStaticSiteProject
    {
        FileActionResult CreateNew(string path, string projectName, bool forceCreate = false);
        FileActionResult Open(string path, string projectName, bool forceOpen = false);
        FileActionResult Save(); //TODO: separate wizard save function that returns IEnumerable<FileActionResult> and uses yield?
        FileActionResult Build(string env);

        FileActionResult SetGlobalProjectValue(string key, string value);
        FileActionResult AddBuildTarget(string env, BuildTargetDefinition buildDefinition);
        FileActionResult AddPage(PageDefinition pageDefinition);
        FileActionResult AddTemplate(HtmlFile template);
        FileActionResult AddContent(HtmlFile content);
        FileActionResult AddSnippet(HtmlFile snippet);

        //Dialog Helpers
        string UserSelectedFolderLocation { get; set; }
        string UserSelectedFileName { get; set; }

        //General Readonly Access
        bool UnsavedChanges { get; }
        Dictionary<string, string> GlobalProjectValues { get; }
        Dictionary<string, BuildTargetDefinition> ProjectBuildTargetDefinitions { get; }
        string[] PendingFilesAndDirectories { get; }
        bool ValidProjectLoaded { get; }
    }
}
