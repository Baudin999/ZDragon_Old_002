using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CLI;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTests
{
    public class ProjectFileWatcher: IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly string dir = Path.GetFullPath("ProjectFileWatcher", Directory.GetCurrentDirectory());
        private readonly Project project;
        public ProjectFileWatcher(ITestOutputHelper output)
        {
            try
            {
                Directory.Delete(dir, true);
            } catch (Exception) {
                Debug.WriteLine("Delete Directory failed in unit test.");
            }

            this.output = output;
            project = new Project(dir);
            Task.Run(() =>
            {
                project.Watch();
            });

            
        }

        [Fact]
#pragma warning disable IDE0051 // Remove unused private members
        private void CreateModule()
#pragma warning restore IDE0051 // Remove unused private members
        {
            project.CreateModule("Test");
            Assert.True(File.Exists(path("Test.car")));
        }


        public void Dispose()
        {
            output.WriteLine("q");
            project.Dispose();
            Directory.Delete(dir, true);
        }

        private string path(string add)
        {
            return Path.GetFullPath(add, dir);
        }
    }
}
