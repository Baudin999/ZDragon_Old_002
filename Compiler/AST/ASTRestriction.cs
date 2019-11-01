using System;
using System.Collections.Generic;

namespace Compiler.AST
{
    public class ASTRestriction : IASTNode
    {
        public string Key { get; }
        public string Value { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }

        public ASTRestriction(string key, string value, IEnumerable<ASTAnnotation> annotations)
        {
            this.Key = key;
            this.Value = value;
            this.Annotations = annotations;
        }

        public static IEnumerable<ASTRestriction> CreateRestrictions(IParser parser)
        {
            var annotations = ASTAnnotation.Parse(parser);
            parser.TryConsume(TokenType.And, out Token? t);
            while (!(t is null))
            {
                var word = parser.Consume(TokenType.Word);
                var value = parser.Consume(TokenType.Number);
                yield return new ASTRestriction(word.Value, value.Value, annotations);

                annotations = ASTAnnotation.Parse(parser);
                parser.TryConsume(TokenType.And, out t);
            }
        }
    }
}
