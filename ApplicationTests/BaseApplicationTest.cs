using System;
using System.IO;
using System.Threading.Tasks;
using Project;
using Xunit.Abstractions;

namespace ApplicationTests
{
    public class BaseFileWatcherTest : IDisposable
    {
        protected readonly ITestOutputHelper output;
        protected readonly string dir;
        protected readonly FileProject project;

        public BaseFileWatcherTest(ITestOutputHelper _output, string _dir)
        {
            dir = Path.GetFullPath(_dir, Directory.GetCurrentDirectory());
            Console.WriteLine("STARTING: " + dir);
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
            output = _output;
            project = new FileProject(dir);
            project.Watch();
        }

        public void Dispose()
        {
            project.Dispose();
            try
            {
                Directory.Delete(dir, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("CLEANUP: " + dir);
        }

        protected string path(string add)
        {
            return Path.GetFullPath(add, dir);
        }
    }
}
