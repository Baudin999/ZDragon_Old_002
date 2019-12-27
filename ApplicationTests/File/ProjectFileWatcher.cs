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
    public class ProjectFileWatcher : BaseFileWatcherTest
    {

        public ProjectFileWatcher(ITestOutputHelper output) : base(output, "ProjectFileWatcher") { }

        [Fact]
        public async Task CreateModule()
        {
            try
            {
                await project.CreateModule("Test", "");
                Assert.True(File.Exists(path("Test.car")));
            }
            catch (Exception ex)
            {
                Console.WriteLine(dir + ": " + ex.Message);
            }
        }
    }
}
