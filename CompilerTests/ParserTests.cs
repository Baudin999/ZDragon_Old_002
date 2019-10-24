using System;
using Xunit;
using Compiler;
using System.Linq;
using Compiler.AST;
using System.Collections.Generic;

namespace CompilerTests
{
    public class ParserTests
    {
        [Fact]
        public void BasicParserTest()
        {
            var code = @"
type Person =
    FirstName: String;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse();
            Assert.NotNull(parseTree);

            List<object> list = parseTree.ToList();
            Assert.Equal(1, list.Count());
            Assert.Equal("Person", (list[0] as ASTType).Name);
        }
    }
}
