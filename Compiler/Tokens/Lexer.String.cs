using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        internal static Token String(Input input)
        {
            return TakeUntill(input, '"', TokenType.String);
        }
    }
}
