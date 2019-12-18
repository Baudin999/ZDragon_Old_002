using System;
namespace Compiler
{
    public static partial class TokenLexers
    {
        internal static Token TakeNewLine(Input input)
        {
            var result = new Token()
            {
                StartIndex = input.Position,
                StartColumn = input.Column,
                StartLine = input.Line,
                EndIndex = input.Position + 1,
                EndColumn = 0,
                EndLine = input.Line + 1,
                Value = Environment.NewLine,
                TokenType = TokenType.NewLine
            };
            input.Next();
            return result;
        }
    }
}
