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
            var builder = NewMethod();
            while ((char.IsLetter(input.Current) || char.IsNumber(input.Current)) || input.Current == '_')
            {
                builder.Append(input.Current);
                if (input.HasNext()) input.Next();
                else break;
            }

            var word = builder.ToString();
            var type = TokenType.Word;
            if (word == "type") type = TokenType.KW_Type;
            if (word == "alias") type = TokenType.KW_Alias;
            if (word == "data") type = TokenType.KW_Data;
            if (word == "choice") type = TokenType.KW_Choice;
            if (word == "extends") type = TokenType.KW_Extends;
            if (word == "flow") type = TokenType.KW_Flow;
            if (word == "component") type = TokenType.KW_Component;
            if (word == "view") type = TokenType.KW_View;
            if (word == "open") type = TokenType.KW_Open;
            if (word == "importing") type = TokenType.KW_Importing;
            if (word == "aggregate") type = TokenType.KW_Importing;
            if (word == "entity") type = TokenType.KW_Importing;
            if (word == "pluck") type = TokenType.KW_Pluck;
            if (word == "compose") type = TokenType.KW_Compose;
            if (word == "loop") type = TokenType.KW_Loop;

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

        private static StringBuilder NewMethod() => new StringBuilder();
    }
}
