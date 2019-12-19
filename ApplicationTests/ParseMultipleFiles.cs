using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CLI;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTests
{
    public class ParseMultipleFiles : IDisposable
    {
        
        private readonly string dir = Path.GetFullPath("ParseMultipleFiles", Directory.GetCurrentDirectory());
        private readonly Project project;
        public ParseMultipleFiles()
        {
            try
            {
                Directory.Delete(dir, true);
            } catch (Exception) {
                Debug.WriteLine("Delete Directory failed in unit test.");
            }
            Directory.CreateDirectory(dir);
            File.WriteAllText(path("First.car"), @"# The first document

type Person
");
            File.WriteAllText(path("Second.car"), @"
open Person
# The second document
type School
");

            project = new Project(dir);
        }

        [Fact]
#pragma warning disable IDE0051 // Remove unused private members
        private void CreateModule()
#pragma warning restore IDE0051 // Remove unused private members
        {
            Assert.Equal(2, project.Modules.Count);
            var second = project.FindModule("Second");
            Assert.Equal(3, second.Generator.AST.Count);
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
