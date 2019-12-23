using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTests
{
    public class MoveModuleInProject : BaseFileWatcherTest
    {
        public MoveModuleInProject(ITestOutputHelper output) : base(output, "MoveModuleInProject") { }

        [Fact]
        public async Task MoveModule()
        {
            try
            {
                var module = await project.CreateModule("Test");
                Assert.True(File.Exists(path("Test.car")));
                Assert.NotNull(module);
                Assert.Equal("Test", module.Name);
                Assert.Equal(this.path("Test.car"), module.FilePath);

                var newModule = await project.MoveModule(module.Name, "Other");
                var oldModule = project.FindModule("Test");
                Assert.Null(oldModule);
                Assert.NotNull(newModule);
            }
            catch (Exception ex)
            {
                Console.WriteLine(dir + ": " + ex.Message);
            }
        }
    }
}
