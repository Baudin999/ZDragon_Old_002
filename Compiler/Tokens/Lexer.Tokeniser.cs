using System;
using System.Text;

namespace Compiler
{
    public partial class TokenLexers
    {

        public static Token Annotation(Input input) => TakeLine(input, TokenType.Annotation);
        public static Token Directive(Input input) => TakeLine(input, TokenType.Directive);
        public static Token Chapter(Input input) => TakeLine(input, TokenType.Chapter);

        public static Token TakeLine(Input input, TokenType type = TokenType.Other)
        {
            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            var builder = new StringBuilder();
            builder.Append(input.Current);

            while (input.HasNext() && !Char2.IsNewLine(input.Next()))
            {
                builder.Append(input.Current);
            }
             var result = new Token
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
            if (input.HasNext()) input.Next();
            return result;
        }

        internal static Token Take(Input input, TokenType type)
        {
            var result = new Token
            {
                StartIndex = input.Position,
                StartColumn = input.Column,
                StartLine = input.Line,
                EndIndex = input.Position + 1,
                EndColumn = input.Column + 1,
                EndLine = input.Line,
                Value = input.Current.ToString(),
                TokenType = type
            };
            input.Next();
            return result;
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
