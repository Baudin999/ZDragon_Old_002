using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        public static Token TakeUntillEndOfContext(Input input)
        {
            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            var builder = new StringBuilder();
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
            }.Normalize();
        }
    }
}
