using System;
using System.IO;
using System.Threading.Tasks;

namespace Project.FileSystems
{
    public class FileSystem : IFileSystem
    {
        public string ReadFileText(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath)) return "";
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (IOException ioe)
            {
                Console.WriteLine("ReadModuleText: Caught Exception reading file [{0}]", ioe);
                throw ioe;
            }
        }

        public async Task SaveFile(string filePath, string source)
        {
            try
            {
                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sr = new StreamWriter(fs))
                    {
                        await sr.WriteAsync(source);
                        sr.Flush();
                        sr.Close();
                        sr.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (IOException ioe)
            {
                Console.WriteLine("ReadModuleText: Caught Exception reading file [{0}]", ioe);
                throw ioe;
            }
        }

        public async Task SaveFile(string fileName, string directoryName, string source)
        {
            try
            {
                var fullPath = Path.Combine(directoryName, fileName);
                Directory.CreateDirectory(directoryName);
                await SaveFile(fullPath, source);
            }
            catch (IOException ioe)
            {
                Console.WriteLine("ReadModuleText: Caught Exception reading file [{0}]", ioe);
                throw ioe;
            }
        }

        public async Task DeleteFile(string fileName)
        {
            try
            {
                await Task.Run(() => System.IO.File.Delete(fileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to delete file: " + fileName);
                Console.WriteLine(ex.Message);
            }
        }

        public async Task DeleteDirectory(string path)
        {
            try
            {

                await Task.Run(() => Directory.Delete(path, true));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to delete directory: " + path);
                Console.WriteLine(ex.Message);
            }
        }

        public async Task CreateDirectory(string path)
        {
            try
            {
                await Task.Run(() => Directory.CreateDirectory(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to create directory: " + path);
                Console.WriteLine(ex.Message);
            }
        }

        public async Task GetAllFiles(string path, string extension, Action<string> handler)
        {
            try
            {
                var allfiles = await Task.Run(() => Directory.GetFiles(path, extension, SearchOption.AllDirectories));
                foreach (var file in allfiles)
                {
                    handler(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to get all files: " + path);
                Console.WriteLine(ex.Message);
            }
        }
    }
}
