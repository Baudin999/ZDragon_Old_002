using System;
using System.Collections.Generic;
using System.Linq;
using Compiler;
using Compiler.AST;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_Alias
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
            Assert.Equal(Helpers.ToTypeDefinition(new[] { "String" }), alias.Type);
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
            Assert.Equal(Helpers.ToTypeDefinition(new[] { "List", "String" }), alias.Type);

        }


        [Fact]
        public void RestrictionCommentsInAlias()
        {
            var code = @"
alias Name = String

    @ Might need to be 1
    & min 2

    @ Will change to 30 later on
    & max 23
;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parseTree);

            ASTAlias alias = parseTree[0] as ASTAlias;
            alias.Restrictions.ForEach(restriction =>
            {
                Assert.Single(restriction.Annotations);
            });

        }


    }
}
