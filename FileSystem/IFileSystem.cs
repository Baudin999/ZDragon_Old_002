using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Project.FileSystems
{
    public interface IFileSystem
    {
        FileSystemType FileSystemType { get;  }
        string ReadFileText(string filePath);
        Task SaveFile(string filePath, string source);
        Task SaveFile(string fileName, string directoryName, string source);
        Task DeleteFile(string fileName);
        Task DeleteDirectory(string path);
        Task CreateDirectory(string path);
        Task GetAllFiles(string path, string extension, Action<string> handler);
        bool HasFile(string path);
    }

    public enum FileSystemType { 
        InMemory,
        PhysicalFile,
        Database
    }

}
