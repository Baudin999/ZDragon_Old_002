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

            var index = -1;

            while (!input.AtEnd)
            {
                var c = input.Current;
                if (index != input.Position)
                {
                    index = input.Position;
                }
                else
                {
                    throw new InvalidOperationException("BOO");
                }
                if (Char.IsLetter(c))
                {
                    var takeWhile = input.TakeWhile(ch => Char.IsLetterOrDigit(ch));
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
                        StartIndex = input.Position,
                        EndIndex = input.Position + 1,
                        StartLine = input.Line,
                        EndLine = input.Line,
                        Value = " ",
                        TokenType = TokenType.WhiteSpace
                    };
                } else
                {
                    yield return new Token()
                    {
                        StartIndex = input.Position,
                        EndIndex = input.Position + 1,
                        StartLine = input.Line,
                        EndLine = input.Line,
                        Value = c.ToString(),
                        TokenType = TokenType.Other
                    };
                }
                input = input.Next();

            }
        }

        
    }
}
