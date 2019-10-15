using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Compiler
{
    public class Lexer
    {
        private Regex LETTER = new Regex("[a-z][A-Z]");

        public IEnumerable<Token> Lex(string code)
        {
            IInput input = new Input(code);

            while (!input.AtEnd)
            {
                var c = input.Current;
                if (!Char.IsWhiteSpace(c))
                {
                    var takeWhile = input.TakeWhile(ch => !Char.IsWhiteSpace(ch));
                    input = takeWhile.Input;
                    yield return new Token()
                    {
                        StartIndex = takeWhile.StartIndex,
                        EndIndex = takeWhile.EndIndex,
                        StartLine = takeWhile.StartLine,
                        EndLine = takeWhile.EndLine,
                        Value = takeWhile.Value,
                        TokenType = TokenType.Word
                    };
                } else if (Char.IsWhiteSpace(c))
                {
                    yield return new Token()
                    {
                        Value = " ",
                        TokenType = TokenType.WhiteSpace
                    };
                }

                input = input.Next();
            }
        }

        
    }
}
