using Compiler;
using Xunit;
using Xunit.Abstractions;

namespace CompilerTests
{
    public class InputTests
    {

        private readonly ITestOutputHelper output;

        public InputTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void StreamThroughInput()
        {
            IInput input = new Input("type Person");
            Assert.Equal(11, input.Source.Length);
        }
    }
}
