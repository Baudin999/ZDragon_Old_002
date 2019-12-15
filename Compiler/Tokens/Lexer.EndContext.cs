using System;
namespace Compiler
{
    public static partial class TokenLexers
    {
        public static bool EndContext(Input input, int depth = 2)
        {
            if (Char2.IsNewLine(input.Current))
            {
                var index = 0;
                while (input.HasPeek(index) && input.Peek(index) == '↓')
                {
                    index++;
                }

                var contextEnded = index == depth && input.HasPeek(index) && input.Peek(index) != '→';
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
