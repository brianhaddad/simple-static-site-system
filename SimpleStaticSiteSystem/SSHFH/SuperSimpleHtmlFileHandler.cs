using SSClasses;
using SSHFH.Tools;
using SSHPW;

namespace SSHFH
{
    public class SuperSimpleHtmlFileHandler : ISuperSimpleHtmlFileHandler
    {
        private readonly IFileIo fileIo;
        private readonly ISuperSimpleHtmlParserWriter htmlParserWriter;

        public SuperSimpleHtmlFileHandler(IFileIo fileIo, ISuperSimpleHtmlParserWriter sshpw)
        {
            this.fileIo = fileIo ?? throw new ArgumentNullException(nameof(fileIo));
            htmlParserWriter = sshpw ?? throw new ArgumentNullException(nameof(sshpw));
        }

        public HtmlDocument ReadFile(string path, string fileName)
        {
            var fileLines = fileIo.GetFileLines(path, fileName);
            return htmlParserWriter.Parse(fileLines);
        }

        public void WriteFile(string path, string fileName, HtmlDocument document)
        {
            var fileLines = htmlParserWriter.Stringify(document);
            fileIo.WriteFileLines(path, fileName, fileLines);
        }
    }
}