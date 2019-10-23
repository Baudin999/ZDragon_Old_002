using System.Collections.Generic;
using Compiler;
using Xunit;
using System.Linq;

namespace CompilerTests
{
    public class LexerTests
    {
        [Fact]
        public void LargeTest()
        {
            var lexer = new Lexer();
            var result = lexer.Lex(Examples.Example1);
            
            Assert.NotNull(lexer);
            Assert.NotNull(result);
            Assert.Equal(51, result.Count());
        }

        [Fact]
        public void SimpleType()
        {
            var lexer = new Lexer();
            var result = lexer.Lex("type Person;");

            Assert.NotNull(lexer);
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void CanLexIndentation()
        {
            var lexer = new Lexer();
            var result = lexer.Lex(@"
type Person =
    FirstName: String
");
            Assert.NotNull(result);
            Assert.Equal(10, result.Count());
            
        }
    }
}

