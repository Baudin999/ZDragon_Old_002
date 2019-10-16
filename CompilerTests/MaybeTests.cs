using System;
using Xunit;
using Compiler;

namespace CompilerTests
{
    public class MaybeTests
    {
        [Fact]
        public void CreateJust()
        {
            Just<string> some = new Just<string>("Carlos");
            Assert.NotNull(some);
            Assert.Equal("Carlos", some.Get());
        }

        [Fact]
        public void CreateNothing()
        {
            Nothing<string> nothing = new Nothing<string>();
            Assert.NotNull(nothing);
            Assert.Throws<InvalidOperationException>(nothing.Get);
        }
    }
}
