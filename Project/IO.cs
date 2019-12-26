using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace Project
{
    internal static class IO
    {
        internal static string ReadFileText(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return "";
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

        internal static async Task SaveFile(string filePath, string source)
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

        internal static async Task SaveFile(string fileName, string directoryName, string source)
        {
            try
            {
                Directory.CreateDirectory(directoryName);
                await SaveFile(fileName, source);
            }
            catch (IOException ioe)
            {
                Console.WriteLine("ReadModuleText: Caught Exception reading file [{0}]", ioe);
                throw ioe;
            }
        }

        internal static async Task DeleteFile(string fileName)
        {
            await Task.Run(() => File.Delete(fileName));
        }

        internal static async Task DeleteDirectory(string directoryName)
        {
            await Task.Run(() => Directory.Delete(directoryName, true));
        }
    }
}
