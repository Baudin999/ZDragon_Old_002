using System;
using System.IO;
using CLI;
using Xunit;

namespace ApplicationTests
{
    public class CreateModuleInProject: IDisposable
    {
        readonly string dir = Path.GetFullPath("CreateModuleInProject", Directory.GetCurrentDirectory());
        readonly Project project;
        public CreateModuleInProject()
        {
            project = new Project(dir);
        }

        [Fact]
        public void CreateModule()
        {
            project.CreateModule("Test");
            Assert.True(File.Exists(path("Test.car")));

            /*
             * Modules are not automatically parsed and added to the project
             * when they are created. This gives you the option of using the
             * CLI as a very very simple "build my files" type of solution.
             */
            var module = project.FindModule("Test");
            Assert.Null(module);
        }


        public void Dispose()
        {
            project.Dispose();
            Directory.Delete(dir, true);
        }

        private string path(string add)
        {
            return Path.GetFullPath(add, dir);
        }
    }
}
