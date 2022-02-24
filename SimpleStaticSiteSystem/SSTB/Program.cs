// See https://aka.ms/new-console-template for more information
using SSClasses;
using SSHFH;
using SSHFH.Tools;
using SSHPW;
using SSSP;

var siteProject = new SimpleStaticSiteProject(new SuperSimpleHtmlFileHandler(new FileIo(), new SuperSimpleHtmlParserWriter()));

var testProjectName = "SiteProject";
var envDir = Environment.CurrentDirectory;
//Warning: this assumes you are running the solution out of the default repo folder name and not a custom folder name.
var projName = "simple-static-site-system";
var testProjectPath = envDir.Substring(0, envDir.IndexOf(projName) + projName.Length) + @"\client-site-prototype\SiteProject";

Console.WriteLine(testProjectPath);
Console.WriteLine("Write test files? [Y/n]");

var userEntry = Console.ReadLine();
if (userEntry == "Y")
{
    //TODO: implement these steps in the new project wizard.
    //First step: need a filename and a directory.
    siteProject.CreateNew(testProjectPath, testProjectName);

    //Next step: get a site title and an author name.
    siteProject.AddGlobalProjectValue("Site Title", "My Simple Static Site");
    siteProject.AddGlobalProjectValue("Author", "Brian Haddad");

    //Default: add a dev build target. (maybe do this during first step?)
    //Optional: get build target data for the actual production site:
    siteProject.AddBuildTarget("dev", testProjectPath.Replace("\\", "/") + "/build/dev");

    //Next step: build an index/home page?
    //Add/define the templates. These will need to be written out to their locations...
    var sortOrder = 0;
    var homePage = new PageDefinition
    {
        IsIndex = true,
        PageTitle = "Test Page",
        PageLayoutTemplate = "MainLayout.sht",
        NavMenuSortIndex = sortOrder,
    };
    siteProject.AddPage(homePage);
    sortOrder++;

    var directoryPageOne = new PageDefinition
    {
        PageSubdirectory = "Nested Test",
        PageTitle = "Nested Page One",
        PageLayoutTemplate = "MainLayout.sht",
        NavMenuSortIndex = sortOrder,
    };
    siteProject.AddPage(directoryPageOne);
    sortOrder++;

    var directoryPageTwo = new PageDefinition
    {
        PageSubdirectory = "Nested Test",
        PageTitle = "Nested Page Two",
        PageLayoutTemplate = "MainLayout.sht",
        NavMenuSortIndex = sortOrder,
    };
    siteProject.AddPage(directoryPageTwo);
    sortOrder++;

    var aboutPage = new PageDefinition
    {
        PageTitle = "About Me",
        PageLayoutTemplate = "MainLayout.sht",
        NavMenuSortIndex = sortOrder,
    };
    siteProject.AddPage(aboutPage);
    sortOrder++;

    var result = siteProject.Save();
    if (!result.Success)
    {
        Console.WriteLine(result.Message);
    }
    else
    {
        Console.WriteLine("Project created successfully.");
    }
}

Console.WriteLine("Run on test files? [Y/n]");

userEntry = Console.ReadLine();
if (userEntry == "Y")
{
    var opened = siteProject.Open(testProjectPath, testProjectName);
    if (!opened.Success)
    {
        Console.WriteLine(opened.Message);
    }
    else
    {
        var result = siteProject.Compile("dev");
        if (!result.Success)
        {
            Console.WriteLine(result.Message);
        }
        else
        {
            Console.WriteLine("Project compiled successfully.");
        }
    }
}