using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        public static Token Word(Input input)
        {
            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            StringBuilder builder = new StringBuilder();
            while (input.HasNext() && (char.IsLetter(input.Current()) || char.IsNumber(input.Current())))
            {
                builder.Append(input.Current());
                input.Next();
            }

            var word = builder.ToString();
            var type = TokenType.Word;
            if (word == "type") type = TokenType.KW_Type;
            if (word == "alias") type = TokenType.KW_Alias;
            if (word == "data") type = TokenType.KW_Data;
            if (word == "choice") type = TokenType.KW_Choice;
            if (word == "flow") type = TokenType.KW_Flow;
            if (word == "component") type = TokenType.KW_Component;
            if (word == "view") type = TokenType.KW_View;

            return new Token()
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
