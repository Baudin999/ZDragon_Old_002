using System;
using System.Linq;
using Compiler;
using Compiler.AST;
using Xunit;

namespace CompilerTests
{
    public class AliasParserTests
    {
        [Fact]
        public void BasicAliasTest()
        {
            var code = @"
alias Name = String;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parseTree);


            ASTAlias alias = parseTree[0] as ASTAlias;
            Assert.Equal("Name", alias.Name);
        }

        [Fact]
        public void RestrictionsTest()
        {
            var code = @"
alias Name = String
    & min 2
    & max 34
;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parseTree);


            ASTAlias alias = parseTree[0] as ASTAlias;
            Assert.Equal(2, alias.Restrictions.Count());
            
        }

        
    }
}
