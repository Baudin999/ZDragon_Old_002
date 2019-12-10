using System;
using System.IO;
using System.Threading.Tasks;
using CLI;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTests
{
    public class ParseMultipleFiles : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly string dir = Path.GetFullPath("ParseMultipleFiles", Directory.GetCurrentDirectory());
        private readonly Project project;
        public ParseMultipleFiles(ITestOutputHelper output)
        {
            try
            {
                Directory.Delete(dir, true);
            } catch (Exception) { }
            Directory.CreateDirectory(dir);
            File.WriteAllText(path("First.car"), @"# The first document

type Person
");
            File.WriteAllText(path("Second.car"), @"
open Person
# The second document
type School
");

            this.output = output;
            project = new Project(dir);
        }

        [Fact]
        private void CreateModule()
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
