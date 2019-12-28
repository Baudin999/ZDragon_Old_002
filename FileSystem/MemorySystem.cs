using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.FileSystems
{
    public class MemorySystem : IFileSystem
    {
        public FileSystemType FileSystemType => FileSystemType.InMemory;
        private static readonly Lazy<Dictionary<string, string>> lazy =
            new Lazy<Dictionary<string, string>>(() => new Dictionary<string, string>());

        public static Dictionary<string, string> Instance { get { return lazy.Value; } }

        public string ReadFileText(string filePath)
        {
            if (!Instance.ContainsKey(filePath)) return "";
            return Instance[filePath];
        }

        public async Task SaveFile(string filePath, string source)
        {
            await Task.Run(() => Instance[filePath] = source);
        }

        public async Task SaveFile(string fileName, string directoryName, string source)
        {
            await Task.Run(() => Instance[directoryName + "/" + fileName] = source);
        }

        public async Task DeleteFile(string fileName)
        {
            await Task.Run(() => Instance.Remove(fileName));
        }

        public async Task DeleteDirectory(string directoryName)
        {
            var removals = Instance.Keys.Where(k => k.StartsWith(directoryName)).Select(k => Task.Run(() => Instance.Remove(k)));
            await Task.WhenAll(removals.ToList());
        }

        public async Task CreateDirectory(string directoryName)
        {
            await Task.Run(() => { });
        }

        public async Task GetAllFiles(string path, string extension, Action<string> handler)
        {
            await Task.Run(() =>
            {
                Instance.Keys.Where(k => k.EndsWith(extension)).ToList().ForEach(handler);
            }); 
        }

        public bool HasFile(string path)
        {
            return MemorySystem.Instance.ContainsKey(path);
        }
    }

    

}
