using System.Collections.Generic;
using Compiler;
using Xunit;
using System.Linq;

namespace CompilerTests
{
    public class LexerTests
    {
        [Fact]
        public void Test1()
        {
            Lexer lexer = new Lexer();
            IEnumerable<Token> result = lexer.Lex("type Person = Foo;");
            Assert.NotNull(lexer);
            Assert.NotNull(result);
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void CanLexIndentation()
        {
            Lexer lexer = new Lexer();
            IEnumerable<Token> result = lexer.Lex(@"
type Person =
    FirstName: String
");
            Assert.NotNull(result);
            
        }
    }
}

