using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        public static Token Identifier(Input input)
        {
            if (!Char.IsUpper(input.Current))
            {
                throw new InvalidOperationException("Identifiers start with an uppercase.");
            }

            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            StringBuilder builder = new StringBuilder();
            while (input.HasNext() && (char.IsLetter(input.Current) || char.IsNumber(input.Current)))
            {
                builder.Append(input.Current);
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

        public static Token QualifiedIdentifier(Input input)
        {
            if (!Char.IsUpper(input.Current))
            {
                throw new InvalidOperationException("Identifiers start with an uppercase.");
            }

            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            StringBuilder builder = new StringBuilder();
            while (input.HasNext() && (char.IsLetter(input.Current) || char.IsNumber(input.Current) || input.Current == '.'))
            {
                builder.Append(input.Current);
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
                TokenType = TokenType.QualifiedIdentifier
            };
        }
    }
}
