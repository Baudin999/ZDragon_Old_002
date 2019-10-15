using System;
using Compiler;
using Xunit;

namespace CompilerTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var lexer = new Lexer();
            Assert.NotNull(lexer);
        }
    }
}

