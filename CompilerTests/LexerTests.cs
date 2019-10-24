using System.Collections.Generic;
using Compiler;
using Xunit;
using System.Linq;
using Xunit.Abstractions;

namespace CompilerTests
{
    public class LexerTests
    {
        private readonly ITestOutputHelper output;

        public LexerTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void LargeTest()
        {
            var lexer = new Lexer();
            var result = lexer.Lex(Examples.Example1);
            
            Assert.NotNull(lexer);
            Assert.NotNull(result);
            Assert.Equal(64, result.Count());
        }

        [Fact]
        public void LexStringString()
        {
            var lexer = new Lexer();
            var result = lexer.Lex(@" type ""Huis""");

            Assert.NotNull(lexer);
            Assert.NotNull(result);

            foreach (var r in result)
            {
                this.output.WriteLine(r.ToString());
            }
            
        }

        [Fact]
        public void CanLexIndentation()
        {
            var lexer = new Lexer();
            var result = lexer.Lex(@"
type Person =
    FirstName: String
");
            Assert.NotNull(result);
            Assert.Equal(11, result.Count());
            
        }

        [Fact]
        public void StartAndEndContext()
        {
            var code = @"type";
            var lexer = new Lexer();
            var result = lexer.Lex(code).ToList();
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Equal(result[0].TokenType, TokenType.ContextStarted);
            Assert.Equal(result[1].TokenType, TokenType.KW_Type);
            Assert.Equal(result[2].TokenType, TokenType.ContextEnded);

            /*
            Keywords result in us entering a context. A context is ended
            when we have a NewLine immediately followed by a Letter or Digit.
            If the newline is followed by a TAB or four spaces we are still
            within the context.
            */
        }

        [Fact]
        public void FailContext01()
        {
            var code = @"
type Person =
    FirstName: String;
LastName: String;

";
            var lexer = new Lexer();
            var result = lexer.Lex(code).ToList();
            Assert.NotNull(result);
            Assert.Equal(14, result.Count());
            Assert.Equal(result[11].TokenType, TokenType.ContextEnded);
            Assert.Equal(result[12].TokenType, TokenType.Paragraph);
            Assert.Equal(result[12].Value, "LastName: String;↓");

        }

        [Fact]
        public void StringParagraph()
        {
            var code = @" ""string"" ";
            var lexer = new Lexer();
            var result = lexer.Lex(code).ToList();
            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
            Assert.Equal(result[0].TokenType, TokenType.Paragraph);

            /*
            Because the string is not defined in a context we expect
            it to be a paragraph. This is by design. We'll want everything
            to default to a pragraph.
            */
        }


        [Fact]
        public void String()
        {
            var code = @"type ""string"" ";
            var lexer = new Lexer();
            var result = lexer.Lex(code).ToList();
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
            Assert.Equal(result[2].TokenType, TokenType.String);

            /*
            We write 'type' in there because strings are only parsed
            as strings when we are in a context. The 'type' keyword
            initialized a context.
            */
        }

        [Fact]
        public void NumberParagraph()
        {
            var code = @"45";
            var lexer = new Lexer();
            var result = lexer.Lex(code).ToList();
            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
            Assert.Equal(result[0].TokenType, TokenType.Paragraph);
        }


        [Fact]
        public void Number()
        {
            var code = @"type 45.24";
            var lexer = new Lexer();
            var result = lexer.Lex(code).ToList();
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
            Assert.Equal(result[2].TokenType, TokenType.Number);
        }
    }
}

