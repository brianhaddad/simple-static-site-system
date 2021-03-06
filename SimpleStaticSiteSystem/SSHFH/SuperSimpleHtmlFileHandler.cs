using SSClasses;
using SSHFH.Tools;
using SSHPW;

namespace SSHFH
{
    //TODO: This should not be a light wrapper or pass-through for the file IO. Give it more purpose!
    //Need to define a clear language for working with the html files that justifies its existence,
    //or just remove the layer and use the file IO in the other classes directly.
    public class SuperSimpleHtmlFileHandler : ISuperSimpleHtmlFileHandler
    {
        private readonly IFileIo _fileIO;
        private readonly ISuperSimpleHtmlParserWriter _htmlParserWriter;

        public SuperSimpleHtmlFileHandler(IFileIo fileIo, ISuperSimpleHtmlParserWriter sshpw)
        {
            _fileIO = fileIo ?? throw new ArgumentNullException(nameof(fileIo));
            _htmlParserWriter = sshpw ?? throw new ArgumentNullException(nameof(sshpw));
        }

        public HtmlDocument ReadFile(string path, string fileName)
        {
            if (!_fileIO.FileExists(path, fileName))
            {
                throw new FileNotFoundException($"{fileName} not found in {path}.");
            }
            var fileLines = _fileIO.GetFileLines(path, fileName);
            return _htmlParserWriter.Parse(fileLines);
        }

        public void WriteFile(HtmlFile file) => WriteFile(file.Path, file.FileName, file.HtmlDocument);

        //TODO: do I really need this one?
        public void WriteFile(string path, string fileName, HtmlDocument document)
        {
            var fileLines = _htmlParserWriter.Stringify(document);
            _fileIO.WriteFileLines(path, fileName, fileLines);
        }

        public T ReadObject<T>(string path, string filename)
            => _fileIO.ReadFromBinaryFile<T>(path, filename);

        public void SaveObject<T>(string path, string filename, T obj)
            => _fileIO.WriteToBinaryFile(path, filename, obj);

        public bool FileExists(string path, string fileName)
            => _fileIO.FileExists(path, fileName);

        public bool CopyFile(string filename, string existingPath, string newPath)
            => _fileIO.CopyFile(filename, existingPath, newPath);

        public void RemoveDirectoryIfExists(string path)
        {
            if (_fileIO.DirectoryExists(path))
            {
                _fileIO.DeleteDirectory(path, true);
            }
        }

        public void CreateDirectory(string path) => _fileIO.CreateDirectory(path);
    }
}