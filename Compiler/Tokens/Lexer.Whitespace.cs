using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        public static Token Whitespace(Input input)
        {
            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            while (input.HasNext() && Char.IsWhiteSpace(input.Current))
            {
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
                Value = " ",
                TokenType = TokenType.WhiteSpace
            };
        }
    }
}
