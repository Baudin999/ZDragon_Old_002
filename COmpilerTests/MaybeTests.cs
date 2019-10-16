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
            var some = new Just<string>("Carlos");
            Assert.NotNull(some);
            Assert.Equal("Carlos", some.Get());
        }

        [Fact]
        public void CreateNothing()
        {
            var nothing = new Nothing<string>();
            Assert.NotNull(nothing);
            Assert.Throws<InvalidOperationException>(nothing.Get);
        }


    }
}
