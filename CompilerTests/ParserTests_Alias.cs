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


            var alias = parseTree[0] as ASTAlias;
            Assert.Equal("Name", alias.Name);
            Assert.Equal(Helpers.ToTypeDefinition(new[] { "String" }), alias.Types.ToList());
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


            var alias = parseTree[0] as ASTAlias;
            Assert.Equal(2, alias.Restrictions.Count());

        }

        [Fact]
        public void ListAlias()
        {
            var code = @"
alias Name = List String;
";
            var g = new ASTGenerator(code);
            Assert.NotNull(g.AST);
            Assert.Empty(g.Errors);
            Assert.IsType<ASTAlias>(g.AST.First());
            var alias = (ASTAlias)g.AST.First();
            Assert.Equal(Helpers.ToTypeDefinition(new[] { "List", "String" }), alias.Types.ToList());

        }


        [Fact]
        public void AliasAnnotations()
        {
            var code = @"
@ This alias represents a name
alias Name = String;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parseTree);
            var alias = parseTree[0] as ASTAlias;
            Assert.Empty(alias.Restrictions);
            Assert.Single(alias.Annotations.ToList());
            Assert.Equal("This alias represents a name", alias.Annotations.First().Value);
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

            var alias = parseTree[0] as ASTAlias;

            foreach (ASTRestriction restriction in alias.Restrictions)
            {
                Assert.Single(restriction.Annotations);
                Assert.Equal(TokenType.Number, restriction.Token.TokenType);
            }

        }

        [Fact]
        public void MaybeAlias()
        {

            var code = @"
@ A name might be null. We will represent this as a
@ Maybe type. 
alias Name = Maybe String;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parseTree);
            ASTAlias alias = parseTree[0] as ASTAlias;
            Assert.Empty(alias.Restrictions);
            Assert.Equal(2, alias.Annotations.Count());
            Assert.Equal(Helpers.ToTypeDefinition(new[] { "Maybe", "String" }), alias.Types);
        }

        [Fact]
        public void PatternRestrictions()
        {
            var code = @"
alias Name = String
    & pattern /[A-Z][a-z]{1,30}/
;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parseTree);

            ASTAlias alias = parseTree[0] as ASTAlias;
            Assert.Single(alias.Restrictions);
            ASTRestriction restriction = alias.Restrictions.First();
            Assert.Equal(TokenType.Pattern, restriction.Token.TokenType);
        }

        [Fact]
        public void StringRestrictions()
        {
            var code = @"
alias Name = String
    & default ""Other thing""
;
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parseTree);

            var alias = parseTree[0] as ASTAlias;
            Assert.Single(alias.Restrictions);
            ASTRestriction restriction = alias.Restrictions.First();
            Assert.Equal(TokenType.String, restriction.Token.TokenType);

        }

        [Fact]
        public void RenameType()
        {
            var code = @"
type School =
    Name: String;

alias FooBar = School;
";
            var g = new ASTGenerator(code);
            var fooBar = g.Find("FooBar");
            Assert.True(fooBar is ASTType);

            var t = (ASTType)fooBar;
            Assert.Single(t.Fields);

            var name = t.Fields.First();
            Assert.Equal("Name", name.Name);
            Assert.Equal("String", name.Types.First().Value);
        }

        [Fact]
        public void AliasOfAView()
        {
            var code = @"
type School =
    Name: String;

view SchoolView =
    School

alias AnotherSchoolView = SchoolView;
";
            var g = new ASTGenerator(code);
            Assert.Empty(g.Errors);

            var school = g.Find<ASTType>("School");
            var schoolView = g.Find<ASTView>("SchoolView");
            var anotherShoolView = g.Find<ASTView>("AnotherSchoolView");

            Assert.NotNull(school);
            Assert.NotNull(schoolView);
            Assert.Null(anotherShoolView);

            Assert.Equal(2, g.AST.Count);
        }
    }
}
