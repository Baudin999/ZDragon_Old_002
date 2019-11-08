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
            Assert.Equal(65, result.Count());
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
            Assert.Equal(12, result.Count());
            
        }

        [Fact]
        public void StartAndEndContext()
        {
            var code = @"type";
            var lexer = new Lexer();
            var result = lexer.Lex(code).ToList();
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
            Assert.Equal(TokenType.ContextStarted, result[0].TokenType);
            Assert.Equal(TokenType.KW_Type, result[1].TokenType);
            Assert.Equal(TokenType.ContextEnded, result[2].TokenType);

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
            Assert.Equal(15, result.Count());
            Assert.Equal(TokenType.ContextEnded, result[11].TokenType);
            Assert.Equal(TokenType.Paragraph, result[12].TokenType);
            Assert.Equal("LastName: String;↓", result[12].Value);

        }

        [Fact]
        public void StringParagraph()
        {
            var code = @" ""string"" ";
            var lexer = new Lexer();
            var result = lexer.Lex(code).ToList();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(TokenType.Paragraph, result[0].TokenType);

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
            Assert.Equal(5, result.Count());
            Assert.Equal(TokenType.String, result[2].TokenType);

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
            Assert.Equal(2, result.Count());
            Assert.Equal(TokenType.Paragraph, result[0].TokenType);
        }


        [Fact]
        public void Number()
        {
            var code = @"type 45.24";
            var lexer = new Lexer();
            var result = lexer.Lex(code).ToList();
            Assert.NotNull(result);
            Assert.Equal(5, result.Count());
            Assert.Equal(TokenType.Number, result[2].TokenType);
        }

        [Fact]
        public void GenericParameter()
        {
            string code = "type 'a";
            var lexer = new Lexer();
            var result = lexer.Lex(code).ToList();
            Assert.NotNull(result);
            Assert.Equal(5, result.Count());
            Assert.Equal(TokenType.GenericParameter, result[2].TokenType);
            Assert.Equal("'a", result[2].Value);
        }
    }
}

