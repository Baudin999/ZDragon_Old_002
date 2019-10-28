using System;
using System.Collections.Generic;
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
            Assert.Equal(new List<ASTTypeDefinition>() {new ASTTypeDefinition("String") }, alias.Type);
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

        [Fact]
        public void ListAlias()
        {
            var code = @"
alias Name = List String;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parseTree);

            ASTAlias alias = parseTree[0] as ASTAlias;
            Assert.Equal(new List<ASTTypeDefinition>() { new ASTTypeDefinition("List"), new ASTTypeDefinition("String") }, alias.Type);

        }


    }
}
