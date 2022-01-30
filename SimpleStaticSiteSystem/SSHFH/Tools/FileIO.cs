using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace SSHFH.Tools
{
    public class FileIo : IFileIo
    {

        private string FileFullPath(string path, string filename) => path + "\\" + filename;

        public string[] GetFilesInDirectory(string path = "", string filetypeFilter = "*.*")
        {
            Directory.CreateDirectory(path);
            var directory = new DirectoryInfo(path);
            var files = directory.GetFiles(filetypeFilter);
            var fileList = new List<string>();
            foreach (var file in files)
            {
                fileList.Add(file.Name);
            }
            return fileList.ToArray();
        }

        public string[] GetFileLines(string path, string filename)
        {
            Directory.CreateDirectory(path);
            if (!FileExists(path, filename))
            {
                return Array.Empty<string>();
            }
            var lines = new List<string>();
            using (var reader = new StreamReader(FileFullPath(path, filename)))
            {
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine() ?? "");
                }
            }
            return lines.ToArray();
        }

        public void LaunchWithDefaultEditor(string path, string filename) => Process.Start("notepad.exe", FileFullPath(path, filename));

        public void WriteFileLines(string path, string filename, string[] lines)
        {
            Directory.CreateDirectory(path);
            using var writer = new StreamWriter(FileFullPath(path, filename), false);
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        public bool MoveFile(string filename, string oldPath, string newPath)
        {
            var oldFile = FileFullPath(oldPath, filename);
            var newFile = FileFullPath(newPath, filename);
            if (FileExists(newFile) || !FileExists(oldFile))
            {
                return false;
            }
            Directory.CreateDirectory(newPath);
            File.Move(oldFile, newFile);
            return FileExists(newFile);
        }

        public void WriteToBinaryFile<T>(string path, string filename, T objectToWrite)
        {
            Directory.CreateDirectory(path);
            using var stream = File.Open(FileFullPath(path, filename), FileMode.Create);
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }

        public T ReadFromBinaryFile<T>(string path, string filename)
        {
            Directory.CreateDirectory(path);
            using var stream = File.Open(FileFullPath(path, filename), FileMode.Open);
            var binaryFormatter = new BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }

        public bool FileExists(string path, string filename) => FileExists(FileFullPath(path, filename));
        public bool FileExists(string filePath) => File.Exists(filePath);
    }
}