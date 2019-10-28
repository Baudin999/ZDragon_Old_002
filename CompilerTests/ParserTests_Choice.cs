using System;
using System.Linq;
using Compiler;
using Compiler.AST;
using Xunit;

namespace CompilerTests
{
    public class ParserTests_Choice
    {
        [Fact]
        public void BasicChoiceTest()
        {
            var code = @"
choice Gender =
    | ""Male""
    | ""Female""
    | ""Other""
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parseTree);

            ASTChoice choice = parseTree[0] as ASTChoice;
            Assert.Equal("Gender", choice.Name);
            Assert.Equal(3, choice.Options.Count);
            Assert.Equal("Male", choice.Options[0].Value);
            Assert.Equal("Female", choice.Options[1].Value);
            Assert.Equal("Other", choice.Options[2].Value);

        }


        [Fact]
        public void ChoicesAnnotations()
        {
            var code = @"
choice Gender =
    | ""Male""
    | ""Female""

    @ Non-binary option
    | ""Other""
";
            var tokens = new Lexer().Lex(code);
            var parseTree = new Parser(tokens).Parse().ToList();
            Assert.NotNull(parseTree);
            Assert.Single(parseTree);

            ASTChoice choice = parseTree[0] as ASTChoice;
            ASTOption option = choice.Options[2];
            Assert.Equal("Other", option.Value);
            Assert.Single(option.Annotations);
            Assert.Equal("Non-binary option", option.Annotations.First().Value);
        }
    }
}
