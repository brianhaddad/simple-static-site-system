using SSClasses;
using SSSP.Classes;

namespace SSSP
{
    public interface ISimpleStaticSiteProject
    {
        FileActionResult CreateNew(string path, string projectName, bool forceCreate = false);
        FileActionResult Open(string path, string projectName, bool forceOpen = false);
        FileActionResult Save();
        FileActionResult Compile();

        FileActionResult AddGlobalProjectValue(string key, string value);
        FileActionResult AddPage(PageDefinition pageDefinition);
    }
}
