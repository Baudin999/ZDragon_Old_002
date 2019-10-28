using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;
using Xunit;

namespace CompilerTests
{
    public class DirectiveTest
    {

        [Fact]
        public void BasicParserTest()
        {
            var code = @"

% XSDElement: Person
type Person
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse();
            Assert.NotNull(parseTree);

            
        }



        /**
         * Directives are little parts which tell the flow
         * of the application where to go. The Directives
         *
         * % key : value
         *
         * Example:
         * % api: /people/{id}
         *
         * Example:
         * % element : People
         */
    }
}
