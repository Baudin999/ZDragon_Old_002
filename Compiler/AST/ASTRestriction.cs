using System;
using System.Collections.Generic;

namespace Compiler.AST
{
    public class ASTRestriction
    {
        public string Key { get; }
        public string Value { get; }
        public ASTRestriction(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public static IEnumerable<ASTRestriction> CreateRestrictions(Parser parser)
        {
            parser.TryConsume(TokenType.And, out Token t);
            while (!(t is null))
            {
                var word = parser.Consume(TokenType.Word);
                var value = parser.Consume(TokenType.Number);
                yield return new ASTRestriction(word.Value, value.Value.ToString());

                parser.TryConsume(TokenType.And, out t);
            }
        }
    }
}
