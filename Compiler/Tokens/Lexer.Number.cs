using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        public static Token Number(Input input)
        {

            if (!char.IsNumber(input.Current))
            {
                throw new InvalidOperationException("Not a number.");
            }

            var hasDot = false;

            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            var builder = new StringBuilder();
            while (input.HasNext())
            {
                if (char.IsNumber(input.Current))
                {
                    builder.Append(input.Current);
                }
                else if (!hasDot && input.Current == '.')
                {
                    builder.Append(input.Current);
                    hasDot = true;
                } else
                {
                    break;
                }
                input.Next();
            }
            return new Token
            {
                StartIndex = start,
                StartColumn = startColumn,
                StartLine = startLine,
                EndIndex = input.Position,
                EndColumn = input.Column,
                EndLine = input.Line,
                Value = builder.ToString(),
                TokenType = TokenType.Number
            };
        }
    }
}
