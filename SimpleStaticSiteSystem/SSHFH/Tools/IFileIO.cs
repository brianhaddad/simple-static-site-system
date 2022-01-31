namespace SSHFH.Tools
{
    public interface IFileIo
    {
        bool FileExists(string filePath);
        bool FileExists(string path, string filename);
        string[] GetFileLines(string path, string filename);
        string[] GetFilesInDirectory(string path = "", string filetypeFilter = "*.*");
        void LaunchWithDefaultEditor(string path, string filename);
        bool MoveFile(string filename, string oldPath, string newPath);
        bool CopyFile(string filename, string existingPath, string newPath);
        void WriteFileLines(string path, string filename, string[] lines);
        void WriteToBinaryFile<T>(string path, string filename, T objectToWrite);
        T ReadFromBinaryFile<T>(string path, string filename);
    }
}