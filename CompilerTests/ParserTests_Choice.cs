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

            var choice = parseTree[0] as ASTChoice;
            Assert.Equal("Gender", choice.Name);
            Assert.Equal(3, choice.Options.Count());

            var choiceOptions = choice.Options.ToList();
            Assert.Equal("Male", choiceOptions[0].Value);
            Assert.Equal("Female", choiceOptions[1].Value);
            Assert.Equal("Other", choiceOptions[2].Value);

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

            var choice = parseTree[0] as ASTChoice;
            var option = choice.Options.Last();
            Assert.Equal("Other", option.Value);
            Assert.Single(option.Annotations);
            Assert.Equal("Non-binary option", option.Annotations.First().Value);
        }
    }
}
