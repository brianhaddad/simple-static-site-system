// See https://aka.ms/new-console-template for more information
using SSClasses;
using SSHFH;
using SSHFH.Tools;
using SSHPW;
using SSSP;

var siteProject = new SimpleStaticSiteProject(new SuperSimpleHtmlFileHandler(new FileIo(), new SuperSimpleHtmlParserWriter()));

var testProjectName = "SiteProject";
var envDir = Environment.CurrentDirectory;
var projName = "simple-static-site-system";
var testProjectPath = envDir.Substring(0, envDir.IndexOf(projName) + projName.Length) + @"\client-site-prototype\SiteProject";

Console.WriteLine(testProjectPath);
Console.WriteLine("Write test files? [Y/n]");

var userEntry = Console.ReadLine();
if (userEntry == "Y")
{
    siteProject.CreateNew(testProjectPath, testProjectName);
    siteProject.AddGlobalProjectValue("Site Title", "My Simple Static Site");
    var page = new PageDefinition
    {
        PageTitle = "Test Page",
        PageLayoutTemplate = "MainLayout.sht",
    };
    siteProject.AddPage(page);
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