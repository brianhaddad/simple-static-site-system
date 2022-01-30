// See https://aka.ms/new-console-template for more information
using SSClasses;
using SSHFH;
using SSHFH.Tools;
using SSHPW;
using SSTB;

var fileHandler = new SuperSimpleHtmlFileHandler(new FileIo(), new SuperSimpleHtmlParserWriter());

Console.WriteLine("Write test files? [Y/n]");

var userEntry = Console.ReadLine();
if (userEntry == "Y")
{
    var project = new StaticSiteProject
    {
        GlobalProjectValues = new Dictionary<string, string>
        {
            { "Site Title", "My Simple Static Site" }
        }
    };

    fileHandler.SaveObject("/", "SiteProject.ssc", project);
}

Console.WriteLine("Run on test files? [Y/n]");

userEntry = Console.ReadLine();
if (userEntry == "Y")
{
    var builder = new SuperSimpleTemplateBuilder(fileHandler);
    builder.Build("/", "SiteProject.ssc"); //TODO: rework to pass in the full project directory
}