using Project;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApplicationTests.InMemoryTests
{
    public class BasicInMemoryTest
    {
        [Fact]
        public async Task CreateInMemoryProject()
        
        {
            IProject project = ProjectContext.InitInMemory();
            IModule module = await project.CreateModule("Test", "");
            Assert.NotNull(module);
            IModule test = project.FindModule("Test");
            Assert.NotNull(test);
        }

        [Fact]
        public async Task CreateALotOfModules()

        {
            IProject project = ProjectContext.InitInMemory();
            IModule test = await project.CreateModule("Test", "");
            IModule other = await project.CreateModule("Other", "");
            IModule something = await project.CreateModule("Something", "");
            IModule foo = await project.CreateModule("Foo", "");
            IModule bar = await project.CreateModule("Bar", "");

            var instance = Project.FileSystems.MemorySystem.Instance;

            Assert.NotNull(test);
            Assert.NotNull(other);
            Assert.NotNull(something);
            Assert.NotNull(foo);
            Assert.NotNull(bar);

            // I automatically generate 3 more modules
            Assert.True(instance.Count > 4);


        }
    }
}
