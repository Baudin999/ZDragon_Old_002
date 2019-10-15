using System.Collections.Generic;
using Compiler;
using Xunit;
using System.Linq;

namespace CompilerTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var lexer = new Lexer();
            var result = lexer.Lex("type Person = Foo;");
            Assert.NotNull(lexer);
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
        }
    }
}

