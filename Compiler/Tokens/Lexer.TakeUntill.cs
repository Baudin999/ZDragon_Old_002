using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        internal static Token TakeUntill(Input input, char end, TokenType type)
        {
            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            var builder = new StringBuilder();
            builder.Append(input.Current);
            while (input.HasNext() && input.Next() != end)
            {
                builder.Append(input.Current);
            }
            builder.Append(input.Current);
            if (input.HasNext()) input.Next();

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
    }
}
