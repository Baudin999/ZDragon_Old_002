using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        public static Token Operators(Input input, string source)
        {
            int start = input.Position;
            int startColumn = input.Column;
            int startLine = input.Line;

            var builder = new StringBuilder();
            builder.Append(input.Current);
            for (int i = 0; i < source.Length -1; ++i)
            {
                builder.Append(input.Next());
            }

            var type = source switch {
                "->" => TokenType.Op_Next,
                "::" => TokenType.Op_Def,
                _ => TokenType.Operator
            };

            input.Next();


            return new Token
            {
                StartIndex = start,
                StartColumn = startColumn,
                StartLine = startLine,
                EndIndex = input.Position,
                EndColumn = input.Column,
                EndLine = input.Line,
                Value = builder.ToString(),
                TokenType = type
            };
        }

    }
}
