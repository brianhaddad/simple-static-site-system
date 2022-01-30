﻿using SSClasses;

namespace SSHFH
{
    public interface ISuperSimpleHtmlFileHandler
    {
        HtmlDocument ReadFile(string path, string fileName);
        void WriteFile(HtmlFile file);
        void WriteFile(string path, string fileName, HtmlDocument document);
        T ReadObject<T>(string path, string filename);
        void SaveObject<T>(string path, string filename, T obj);
    }
}
