using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Lexer
    {

        private bool ignoreWhiteSpace = true;

        private string prepareSource(string source)
        {
            source = new Regex(@"\r\n|\n").Replace(source, "↓");
            source = new Regex(@"[\s\t]+↓").Replace(source, "↓");
            source = new Regex("    |\t").Replace(source, "→");
            Console.WriteLine(source);
            return source;
        }

        public IEnumerable<Token> Lex(string code)
        {
            string preparedSource = prepareSource(code);
            var input = new Input(preparedSource);
            var context = false;

            while (input.HasNext())
            {
                if (input.IsKeyword("type") ||
                    input.IsKeyword("alias") ||
                    input.IsKeyword("data") ||
                    input.IsKeyword("choice"))
                {
                    context = true;
                    Console.WriteLine("Starting Context");
                    yield return TokenLexers.Word(input);
                }
                else if (!context && Char.IsLetter(input.Current()))
                {
                    yield return TokenLexers.TakeUntillEndOfContext(input);
                }
                else if (context && Char2.IsNewLine(input.Current()) && TokenLexers.EndContext(input))
                {
                    // do nothing but end Context...
                    Console.WriteLine("Ending Context " + input.Current().ToString());
                    context = false;
                }
                else if (context && Char.IsUpper(input.Current()))
                {
                    yield return TokenLexers.Identifier(input);
                }
                else if ( context && Char.IsLower(input.Current()))
                {
                    yield return TokenLexers.Word(input);
                }
                else if (input.Current() == '#')
                {
                    yield return TokenLexers.Chapter(input);
                }
                else if (input.Current() == '@')
                {
                    yield return TokenLexers.Annotation(input);
                }
                else if (input.Current() == '%')
                {
                    yield return TokenLexers.Directive(input);
                }
                else if (input.Current() == ';')
                {
                    yield return TokenLexers.Take(input, TokenType.EndStatement);
                }
                else if (Char2.Indent(input.Current()))
                {
                    yield return TokenLexers.Take(input, TokenType.Ident);
                }
                else if (Char2.Equal(input.Current()))
                {
                    yield return TokenLexers.Take(input, TokenType.Equal);
                }
                else if (Char2.Or(input.Current()))
                {
                    yield return TokenLexers.Take(input, TokenType.Or);
                }
                else if (Char2.Separator(input.Current()))
                {
                    yield return TokenLexers.Take(input, TokenType.Separator);
                }
                else if (Char2.IsNewLine(input.Current()))
                {
                    var newline = TokenLexers.Take(input, TokenType.NewLine);
                    newline.Value = "↓";
                    yield return newline;
                }
                else if (Char.IsWhiteSpace(input.Current())) {
                    var whiteSpace = TokenLexers.Whitespace(input);
                    if (!ignoreWhiteSpace)
                    {
                        yield return whiteSpace;
                    }
                }
                else if (!context)
                {
                    yield return TokenLexers.TakeUntillEndOfContext(input);
                }
                else
                {
                    input.Next();
                }
            }
        }

        
    }

    public class TokenLexers
    {

        public static Token TakeUntillEndOfContext(Input input)
        {
            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            StringBuilder builder = new StringBuilder();
            while (input.HasNext() && !EndContext(input)) 
            {
                builder.Append(input.Current());
                input.Next();
            }
            return new Token()
            {
                StartIndex = start,
                StartColumn = startColumn,
                StartLine = startLine,
                EndIndex = input.Position,
                EndColumn = input.Column,
                EndLine = input.Line,
                Value = builder.ToString(),
                TokenType = TokenType.Paragraph
            };
        }

        public static bool EndContext(Input input)
        {
            if (Char2.IsNewLine(input.Current()))
            {
                var index = 0;
                while(input.HasPeek(index) && input.Peek(index) == '↓') {
                    index++;
                }

                var contextEnded = index == 2 && input.HasPeek(index) && input.Peek(index) != '→';
                if (contextEnded)
                {
                    for (var i = 0; i < index; ++i) input.Next();
                }

                return contextEnded;
            } else { return false; }
        }

        public static Token Identifier(Input input)
        {
            if (!Char.IsUpper(input.Current()))
            {
                throw new InvalidOperationException("Identifiers start with an uppercase.");
            }

            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            StringBuilder builder = new StringBuilder();
            while (input.HasNext() && (Char.IsLetter(input.Current()) || Char.IsNumber(input.Current())))
            {
                builder.Append(input.Current());
                input.Next();
            }
            return new Token()
            {
                StartIndex = start,
                StartColumn = startColumn,
                StartLine = startLine,
                EndIndex = input.Position,
                EndColumn = input.Column,
                EndLine = input.Line,
                Value = builder.ToString(),
                TokenType = TokenType.Identifier
            };
        }

        public static Token Word(Input input)
        {
            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            StringBuilder builder = new StringBuilder();
            while (input.HasNext() && (Char.IsLetter(input.Current()) || Char.IsNumber(input.Current())))
            {
                builder.Append(input.Current());
                input.Next();
            }

            var word = builder.ToString();
            var type = TokenType.Word;
            if (word == "type") type = TokenType.KW_Type;
            if (word == "alias") type = TokenType.KW_Alias;
            if (word == "data") type = TokenType.KW_Data;
            if (word == "choice") type = TokenType.KW_Choice;
            if (word == "flow") type = TokenType.KW_Flow;
            if (word == "component") type = TokenType.KW_Component;
            if (word == "view") type = TokenType.KW_View;

            return new Token()
            {
                StartIndex = start,
                StartColumn = startColumn,
                StartLine = startLine,
                EndIndex = input.Position,
                EndColumn = input.Column,
                EndLine = input.Line,
                Value = builder.ToString(),
                TokenType = type
            };
        }

        public static Token Whitespace(Input input)
        {
            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            while (input.HasNext() && Char.IsWhiteSpace(input.Current()))
            {
                input.Next();
            }

            return new Token()
            {
                StartIndex = start,
                StartColumn = startColumn,
                StartLine = startLine,
                EndIndex = input.Position,
                EndColumn = input.Column,
                EndLine = input.Line,
                Value = " ",
                TokenType = TokenType.WhiteSpace
            };
        }

        public static Token Annotation(Input input) => TakeLine(input, TokenType.Annotation);
        public static Token Directive(Input input) => TakeLine(input, TokenType.Directive);
        public static Token Chapter(Input input) => TakeLine(input, TokenType.Chapter);

        public static Token TakeLine(Input input, TokenType type = TokenType.Other)
        {
            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            StringBuilder builder = new StringBuilder();
            builder.Append(input.Current());

            while (input.HasNext() && !Char2.IsNewLine(input.Next()))
            {
                builder.Append(input.Current());
            }
            return new Token()
            {
                StartIndex = start,
                StartColumn = startColumn,
                StartLine = startLine,
                EndIndex = input.Position,
                EndColumn = input.Column,
                EndLine = input.Line,
                Value = builder.ToString(),
                TokenType = type
            };
        }

        internal static Token Take(Input input, TokenType type)
        {
            var c = input.Current();
            input.Next();

            return new Token()
            {
                StartIndex = input.Position - 1,
                StartColumn = input.Column - 1,
                StartLine = input.Line,
                EndIndex = input.Position,
                EndColumn = input.Column,
                EndLine = input.Line,
                Value = c.ToString(),
                TokenType = type
            };
        }
    }



    public static class Char2
    {
        public static bool IsNewLine(char c)
        {
            return c == '↓';
        }

        public static bool Equal(char c)
        {
            return c == '=';
        }

        public static bool Separator(char c)
        {
            return c == ':';
        }

        public static bool Or(char c)
        {
            return c == '|';
        }

        public static bool And(char c)
        {
            return c == '&';
        }

        public static bool Indent(char c)
        {
            return c == '→';
        }
    }
}

