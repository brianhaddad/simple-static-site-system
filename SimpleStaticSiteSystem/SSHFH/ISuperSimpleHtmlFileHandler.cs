using SSClasses;

namespace SSHFH
{
    public interface ISuperSimpleHtmlFileHandler
    {
        HtmlDocument ReadFile(string path, string fileName);
        void WriteFile(string path, string fileName, HtmlDocument document);
    }
}
