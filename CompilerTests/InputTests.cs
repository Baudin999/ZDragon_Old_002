using Compiler;
using Xunit;
using System;
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
            Input input = new Input("type Person");
            while (input.HasNext())
            {
                Assert.NotNull(input.Current());
                input.Next();
            }
        }
    }
}
