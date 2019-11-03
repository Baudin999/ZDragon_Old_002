
using System.Linq;
using Compiler;
using Compiler.AST;
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
            _ = parser.Parse().ToList();
            Assert.Single(parser.Errors);
        }

        [Fact]
        public void WrongIndentation()
        {
            var code = @"
type School =
Name: String;
";
            var tokens = new Lexer().Lex(code);
            var parser = new Parser(tokens);
            _ = parser.Parse().ToList();

            Assert.Single(parser.Errors);
        }

        [Fact]
        public void WrongRestrictionIndentation()
        {
            var code = @"
type School =
    Name: String
& min 12;
";
            var tokens = new Lexer().Lex(code);
            var parser = new Parser(tokens);
            _ = parser.Parse().ToList();

            Assert.Single(parser.Errors);
        }
    }
}
