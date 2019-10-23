using System;
namespace Compiler
{
    public static partial class TokenLexers
    {
        public static bool EndContext(Input input)
        {
            if (Char2.IsNewLine(input.Current()))
            {
                var index = 0;
                while (input.HasPeek(index) && input.Peek(index) == '↓')
                {
                    index++;
                }

                var contextEnded = index == 2 && input.HasPeek(index) && input.Peek(index) != '→';
                if (contextEnded)
                {
                    for (var i = 0; i < index; ++i) input.Next();
                }

                return contextEnded;
            }
            else { return false; }
        }
    }
}
