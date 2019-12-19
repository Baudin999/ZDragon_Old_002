using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        public static Token GenericParameter(Input input)
        {

            if (input.Current != '\'')
            {
                throw new InvalidOperationException("Not a GenericParameter.");
            }

            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            var builder = new StringBuilder();
            builder.Append(input.Current);

            while (input.HasNext() && Char.IsLetter(input.Next()))
            {
                builder.Append(input.Current);
            }

            return new Token
            {
                StartIndex = start,
                StartColumn = startColumn,
                StartLine = startLine,
                EndIndex = input.Position,
                EndColumn = input.Column,
                EndLine = input.Line,
                Value = builder.ToString(),
                TokenType = TokenType.GenericParameter
            };
        }
    }
}
