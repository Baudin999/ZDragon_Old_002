using System;
namespace Compiler
{
    public static partial class TokenLexers
    {
        internal static Token TakeIndent(Input input)
        {
            input.Next();

            return new Token()
            {
                StartIndex = input.Position - 1,
                StartColumn = input.Column - 1,
                StartLine = input.Line - 1,
                EndIndex = input.Position,
                EndColumn = 0,
                EndLine = input.Line,
                Value = "   ",
                TokenType = TokenType.NewLine
            };
        }
    }
}
