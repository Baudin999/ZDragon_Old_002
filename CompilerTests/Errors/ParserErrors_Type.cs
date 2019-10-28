
using System.Linq;
using Compiler;
using Xunit;

namespace CompilerTests.Errors
{
    public class ParserErrors_Type
    {
        [Fact]
        public void NoBodyException()
        {
            var code = @"
type Person =
";
            var tokens = new Lexer().Lex(code);
            var parser = new Parser(tokens);
            var parseTree = parser.Parse().ToList();

            Assert.Single(parser.Errors);


        }
    }
}
