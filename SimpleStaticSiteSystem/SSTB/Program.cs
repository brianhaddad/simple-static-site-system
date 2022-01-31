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
    siteProject.CreateNew(testProjectPath, testProjectName);
    siteProject.AddGlobalProjectValue("Site Title", "My Simple Static Site");
    siteProject.AddGlobalProjectValue("Author", "Brian Haddad");
    var homePage = new PageDefinition
    {
        IsIndex = true,
        PageTitle = "Test Page",
        PageLayoutTemplate = "MainLayout.sht",
    };
    siteProject.AddPage(homePage);
    var aboutPage = new PageDefinition
    {
        PageTitle = "About Me",
        PageLayoutTemplate = "MainLayout.sht",
    };
    siteProject.AddPage(aboutPage);
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
        var result = siteProject.Compile();
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