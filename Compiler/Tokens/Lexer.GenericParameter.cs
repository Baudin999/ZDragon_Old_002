using System;
using System.Text;

namespace Compiler
{
    public static partial class TokenLexers
    {
        public static Token GenericParameter(Input input)
        {
            input.Next();
            var start = input.Position;
            var startColumn = input.Column;
            var startLine = input.Line;

            StringBuilder builder = new StringBuilder();
            while ((char.IsLetter(input.Current()) || char.IsNumber(input.Current())) || input.Current() == '_')
            {
                builder.Append(input.Current());
                if (input.HasNext()) input.Next();
                else break;
            }

            var word = builder.ToString();
            var type = TokenType.GenericParameter;
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
