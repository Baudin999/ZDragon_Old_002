using System;
using System.Linq;
using Compiler;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_Data
    {
        [Fact]
        public void InvalidTokenExceptionOnOneLiner()
        {
            var code = @"
data Maybe 'a =
    | Just 'a
    | Nothing
";
            var tokens = new Lexer().Lex(code);
            var parser = new Parser(tokens);
            var parseTree = parser.Parse().ToList();

            Assert.NotNull(parseTree);
            Assert.Empty(parser.Errors);
            Assert.Single(parseTree);
        }


        [Fact]
        public void TestResolve()
        {
            var code = @"
data Maybe 'a =
    | Just 'a
    | Nothing
";
            var tokens = new Lexer().Lex(code);
            var parser = new Parser(tokens);
            var parseTree = parser.Parse().ToList();
            var resolver = new Resolver(parseTree);
            var newParseTree = resolver.Resolve().ToList();

            Assert.NotNull(parseTree);
            Assert.Empty(parser.Errors);

            Assert.Equal(3, newParseTree.Count);
        }

        [Fact]
        public void SimpleCustomerTest()
        {
            var code = @"
data Customer =
    | Person
    | Organisation
";
            var tokens = new Lexer().Lex(code);
            var parser = new Parser(tokens);
            var parseTree = parser.Parse().ToList();
            var resolver = new Resolver(parseTree);
            var newParseTree = resolver.Resolve().ToList();

            Assert.NotNull(parseTree);
            Assert.Empty(parser.Errors);

            Assert.Equal(3, newParseTree.Count);
        }
    }
}
