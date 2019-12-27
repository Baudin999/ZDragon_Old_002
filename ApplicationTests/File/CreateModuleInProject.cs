using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CLI;
using Project;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTests
{
    public class CreateModuleInProject : BaseFileWatcherTest
    {
        public CreateModuleInProject(ITestOutputHelper output) : base(output, "CreateModuleInProject") { }

        [Fact]
        public async Task CreateModule()
        {
            try
            {
                var module = await project.CreateModule("Test", "Code");
                Assert.True(File.Exists(path("Test.car")));
                Assert.NotNull(module);
                Assert.Equal("Test", module.Name);
                Assert.Equal(this.path("Test.car"), module.FilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(dir + ": " + ex.Message);
            }
        }
    }
}
