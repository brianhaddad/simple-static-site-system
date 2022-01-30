using SSClasses;
using SSHFH;

namespace SSTB
{
    public class SuperSimpleTemplateBuilder
    {
        private readonly ISuperSimpleHtmlFileHandler _fileHandler;

        public SuperSimpleTemplateBuilder(ISuperSimpleHtmlFileHandler fileHandler)
        {
            _fileHandler = fileHandler ?? throw new ArgumentNullException(nameof(fileHandler));
        }

        public void Build(string path, string projectFileName)
        {
            var project = _fileHandler.ReadObject<StaticSiteProject>(path, projectFileName);

            var results = new List<HtmlFile>();

            //TODO: build the final file(s) from the project/directory data/contents.

            foreach (var result in results)
            {
                _fileHandler.WriteFile(result);
            }
        }
    }
}
