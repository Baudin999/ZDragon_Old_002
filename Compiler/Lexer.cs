using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Compiler
{
    public class Lexer
    {

        private bool ignoreWhiteSpace = true;

        private string prepareSource(string source)
        {
            source = new Regex(" +\n").Replace(source, "\n");
            source = new Regex("    |\t").Replace(source, "→");
            return source;
        }

        public IEnumerable<Token> Lex(string code)
        {

            IInput input = new Input(prepareSource(code));

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
                }
                else if (c == '\r')
                {
                    if (input.Peek().Get() == '\n')
                    {
                        input = input.Next();
                    }
                    yield return new Token()
                    {
                        StartIndex = input.Position,
                        EndIndex = input.Position,
                        StartLine = input.Line,
                        EndLine = input.Line + 1,
                        TokenType = TokenType.NewLine
                    };
                }
                else if (c == '\n') {
                    yield return new Token()
                    {
                        StartIndex = input.Position,
                        EndIndex = input.Position,
                        StartLine = input.Line,
                        EndLine = input.Line + 1,
                        TokenType = TokenType.NewLine
                    };
                } else if (Char.IsWhiteSpace(c))
                {
                    if (!ignoreWhiteSpace)
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
                    }
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
