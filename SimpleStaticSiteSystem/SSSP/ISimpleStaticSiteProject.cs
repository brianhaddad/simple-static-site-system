using SSClasses;
using SSSP.Classes;

namespace SSSP
{
    public interface ISimpleStaticSiteProject
    {
        FileActionResult CreateNew(string path, string projectName, bool forceCreate = false);
        FileActionResult Open(string path, string projectName, bool forceOpen = false);
        FileActionResult Save();
        FileActionResult Compile(string env);

        FileActionResult AddGlobalProjectValue(string key, string value);
        FileActionResult AddBuildTarget(string env, string baseUrl);
        FileActionResult AddPage(PageDefinition pageDefinition);

        bool UnsavedChanges { get; }
    }
}
