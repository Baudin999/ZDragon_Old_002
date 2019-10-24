using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        internal static Token Comment(Input input)
        {
            return TakeUntill(input, '}', TokenType.Comment);
        }
    }
}
