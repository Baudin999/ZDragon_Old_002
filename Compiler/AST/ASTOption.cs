using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTOption : IASTNode, ICloneable
    {
        public string Value { get;  }
        public IEnumerable<ASTAnnotation> Annotations { get; }
        public ASTOption() { }
        public ASTOption(string value, IEnumerable<ASTAnnotation> annotations)
        {
            this.Value = value;
            this.Annotations = annotations;
        }

        public static IEnumerable<ASTOption> Parse(IParser parser)
        {
            var annotations = ASTAnnotation.Parse(parser);
            parser.TryConsume(TokenType.Or, out Token? t);
            while (!(t is null))
            {
                var value = parser.Consume(TokenType.String);
                yield return new ASTOption(value.Value.Substring(1, value.Value.Length - 2), annotations);

                annotations = ASTAnnotation.Parse(parser);
                parser.TryConsume(TokenType.Or, out t);
            }
        }

        public object Clone()
        {
            return new ASTOption(
                (string)this.Value.Clone(),
                ObjectCloner.CloneList(this.Annotations.ToList()));
        }
    }
}
