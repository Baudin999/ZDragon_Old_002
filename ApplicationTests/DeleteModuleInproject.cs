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
    public class DeleteModuleInproject : BaseFileWatcherTest
    {
        public DeleteModuleInproject(ITestOutputHelper output) : base(output, "DeleteModuleInproject") { }

        [Fact]
        public async Task CreateModule()
        {
            try
            {
                var module = await project.CreateModule("Test");
                var filePath = module.FilePath.Clone().ToString();
                var outPath = module.OutPath.Clone().ToString();

                Assert.True(File.Exists(path("Test.car")));
                Assert.NotNull(module);
                Assert.Equal("Test", module.Name);
                Assert.Equal(this.path("Test.car"), module.FilePath);

                var deleteResult = await project.DeleteModule("Test");

                await Task.Delay(100);
                Assert.True(deleteResult);
                Assert.False(File.Exists(filePath));
                Assert.False(Directory.Exists(outPath));
            } catch (Exception ex)
            {
                Console.WriteLine(dir + ": " + ex.Message);
            }
        }
    }
}
