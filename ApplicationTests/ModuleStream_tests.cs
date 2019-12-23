using System;
using System.Threading.Tasks;
using Project;
using Xunit;

namespace ApplicationTests
{

    public class ModuleStream_tests : IDisposable
    {
        ModuleStream moduleStream;

        public ModuleStream_tests()
        {
            Console.WriteLine("STARTING: ModuleStream");
            moduleStream = new ModuleStream();
        }

        [Fact]
        public Task CreateModuleStream()
        {
            var tcs = new TaskCompletionSource<bool>();
            moduleStream.Subscribe("Logger", message =>
            {
                Assert.NotNull(message);
                Assert.Equal("First Message", message.ModuleName);
                Assert.Equal("Path", message.FileFullPath);
                Assert.Equal(MessageType.ModuleChanged, message.MessageType);

                tcs.TrySetResult(true);
            });

            moduleStream.Publish(new ModuleStreamMessage("First Message", "Path", MessageType.ModuleChanged));

            return tcs.Task;
        }

        public void Dispose()
        {
            moduleStream.Dispose();
            Console.WriteLine("CLEANUP: ModuleStream");
        }
    }
}
