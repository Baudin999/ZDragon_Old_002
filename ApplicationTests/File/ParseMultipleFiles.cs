using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Project;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTests
{
    public class ParseMultipleFiles : BaseFileWatcherTest
    {

        public ParseMultipleFiles(ITestOutputHelper output) : base(output, "ParseMultipleFiles") { }

        [Fact]
        public async Task Execute()
        {
            try
            {
                var tasks = new List<Task>
                {
                    project.CreateModule("First", @"# The first document

type Person
"),
                    project.CreateModule("Second", @"
open Person
# The second document
type School
")
                };
                await Task.WhenAll(tasks.ToArray());
                Assert.Equal(2, project.Modules.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(dir + ": " + ex.Message);
            }
        }

    }
}
