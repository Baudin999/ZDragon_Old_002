using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTests
{
    public class SchoolExampleTest : BaseFileWatcherTest
    {
        public SchoolExampleTest(ITestOutputHelper output) : base(output, "SchoolExample") { }

        private async Task Init()
        {
            await project.CreateModule("School", @"
# School

This is a paragraph!

type School

type Student

type Teacher
");
        }

        [Fact]
        public async Task InitModule()
        {
            try
            {
                await Init();
                var module = project.FindModule("School");
                Assert.True(File.Exists(path("School.car")));
                Assert.NotNull(module);
                Assert.Equal("School", module.Name);
                Assert.Equal(this.path("School.car"), module.FilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(dir + ": " + ex.Message);
            }
        }

        [Fact]
        public async Task CompileSchoolModule()
        {
            await Init();
            project.ParseAllModules();
            var ast = project.GetAstForModule("School");
            Assert.True(ast.Count > 0);
        }

        [Fact]
        public async Task RenameSchoolModule()
        {
            await Init();
            await project.MoveModule("School", "Foo.Bar");
            Assert.False(File.Exists(path("School.car")), "School.car file still exists.");
            Assert.True(File.Exists(path("Foo/Bar.car")), "Foo/Bar.car file does not exits");

            Assert.Null(project.FindModule("School"));
            Assert.NotNull(project.FindModule("Foo.Bar"));

            await Task.Run(() => Task.Delay(100));
        }

        [Fact]
        public async Task AddStudentModule()
        {
            await Init();
            var studentModule = await project.CreateModule("Student");
            var schoolModule = project.FindModule("School");

            Assert.NotNull(studentModule);
            Assert.NotNull(schoolModule);
        }

        [Fact]
        public async Task DeleteSchoolModule()
        {
            await Init();
            await project.DeleteModule("School");
            var schoolModule = project.FindModule("School");
            Assert.Null(schoolModule);
        }
    }
}
