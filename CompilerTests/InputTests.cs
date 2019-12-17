using Compiler;
using Xunit;
using System;
using Xunit.Abstractions;
using System.Linq;

namespace CompilerTests
{
    public class InputTests
    {

        private readonly ITestOutputHelper output;

        public InputTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void StreamThroughInput()
        {
            var input = new Input("type Person");
            while (input.HasNext())
            {
                input.Next();
            }
            Assert.True(!input.HasNext());
        }

        [Fact]
        public void TokenPositionSingleWord()
        {
            var lexer = new Lexer();
            var result = lexer.Lex(@"Word");
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var word = result.First();
            var EOF = result.Last();

            Assert.Equal(0, word.StartColumn);
            Assert.Equal(0, word.StartLine);
            Assert.Equal(4, word.EndColumn);
            Assert.Equal(0, word.EndLine);

        }

        [Fact]
        public void EndOfFilePosition()
        {
            var lexer = new Lexer();
            var result = lexer.Lex(@"
type Person =
    FirstName: String;
");
            Assert.NotNull(result);
            Assert.Equal(13, result.Count());

            var EOF = result.Last();
            Assert.Equal(3, EOF.StartLine);
            Assert.Equal(3, EOF.EndLine);
            Assert.Equal(0, EOF.StartColumn);
            Assert.Equal(0, EOF.EndColumn);
        }

        [Fact]
        public void EndOfFileEmptyPosition()
        {
            var lexer = new Lexer();
            var result = lexer.Lex(@"");

            var EOF = result.Last();
            Assert.Equal(0, EOF.StartLine);
            Assert.Equal(0, EOF.EndLine);
            Assert.Equal(0, EOF.StartColumn);
            Assert.Equal(0, EOF.EndColumn);
        }


        [Fact]
        public void ParagraphPositions()
        {
            var lexer = new Lexer();
            var result = lexer.Lex(@"This is a pragraph
which I want to test for the lenght
and the positions in the text file.");

            var paragraph = result.First();
            Assert.Equal(0, paragraph.StartLine);
            Assert.Equal(2, paragraph.EndLine);
            Assert.Equal(0, paragraph.StartColumn);
            Assert.Equal(35, paragraph.EndColumn);


            var EOF = result.Last();
            Assert.Equal(2, EOF.StartLine);
            Assert.Equal(2, EOF.EndLine);
            Assert.Equal(0, EOF.StartColumn);
            Assert.Equal(0, EOF.EndColumn);
        }

        [Fact]
        public void LexExample01()
        {
            var lexer = new Lexer();
            var result = lexer.Lex(@"
# Chapter One

type Person

## Chapter Two

alias Name = String;

").Where(n=> n.TokenType != TokenType.NewLine).ToList();


            var chapterOne = result[0];
            var person = result[1];
            var chapterTwo = result[2];
            var aliasOne = result[3];

            Assert.NotNull(chapterOne);
            Assert.Equal(1, chapterOne.StartLine);
            Assert.NotNull(person);
            Assert.NotNull(chapterTwo);
            Assert.NotNull(aliasOne);
        }
    }
}
