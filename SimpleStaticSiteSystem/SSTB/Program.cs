// See https://aka.ms/new-console-template for more information
using SSClasses;
using SSHFH;
using SSHFH.Tools;
using SSHPW;
using SSSP;
using SSSP.ProjectValues;

var siteProject = new SimpleStaticSiteProject(new SuperSimpleHtmlFileHandler(new FileIo(), new SuperSimpleHtmlParserWriter()));

var testProjectName = "SiteProject";
var envDir = Environment.CurrentDirectory;
//Warning: this assumes you are running the solution out of the default repo folder name and not a custom folder name.
var projName = "simple-static-site-system";
var testProjectPath = envDir[..(envDir.IndexOf(projName) + projName.Length)] + @"\client-site-prototype";

//Curiosity:
var testProjectRoot = Path.GetPathRoot(testProjectPath);
if (testProjectRoot is not null)
{
    var testPathMinusRoot = testProjectPath.Substring(testProjectRoot.Length);
    var testPath = testProjectRoot + testPathMinusRoot[..testPathMinusRoot.IndexOf(Path.DirectorySeparatorChar)];
    Console.WriteLine("Short path first:");
    Console.WriteLine(Path.GetRelativePath(testPath, testProjectPath));
    Console.WriteLine("Long path first:");
    Console.WriteLine(Path.GetRelativePath(testProjectPath, testPath));
}

//Begin for realsies
Console.WriteLine(testProjectPath);
Console.WriteLine("Write test files? [Y/n]");

var userEntry = Console.ReadLine();
if (userEntry == "Y")
{
    //TODO: implement these steps in the new project wizard.
    //First step: need a filename and a directory.
    siteProject.CreateNew(testProjectPath, testProjectName);

    //Next step: get a site title and an author name.
    siteProject.SetGlobalProjectValue(GlobalValueKeys.SiteTitle, "My Simple Static Site");
    siteProject.SetGlobalProjectValue(GlobalValueKeys.Author, "Brian Haddad");

    //Default: add a dev build target. (maybe do this during first step?)
    //Optional: get build target data for the actual production site:
    var buildDefinition = new BuildTargetDefinition
    {
        TargetBaseUrl = Path.Combine(testProjectPath, testProjectName).Replace("\\", "/") + "/build/dev",
    };
    siteProject.AddBuildTarget("dev", buildDefinition);

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
        var result = siteProject.Build("dev");
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