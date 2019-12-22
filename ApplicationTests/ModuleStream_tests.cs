using System;
using System.Threading.Tasks;
using CLI;
using Xunit;

namespace ApplicationTests
{

    public class ModuleStream_tests : IDisposable
    {
        ModuleStream moduleStream;

        public ModuleStream_tests()
        {
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
        }
    }
}
