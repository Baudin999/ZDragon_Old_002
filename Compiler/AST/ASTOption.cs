using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTOption : IASTNode, ICloneable
    {
        public string Value { get; }
        public string Module { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }

        public ASTOption(string value, string module, IEnumerable<ASTAnnotation> annotations)
        {
            this.Value = value;
            this.Module = module;
            this.Annotations = annotations;
        }

        public static IEnumerable<ASTOption> Parse(IParser parser, string module = "")
        {
            var annotations = ASTAnnotation.Parse(parser);
            parser.TryConsume(TokenType.Or, out Token? t);
            while (!(t is null))
            {
                var value = parser.Consume(TokenType.String);
                yield return new ASTOption(value.Value.Substring(1, value.Value.Length - 2), module, annotations);

                annotations = ASTAnnotation.Parse(parser);
                parser.TryConsume(TokenType.Or, out t);
            }
        }

        public object Clone()
        {
            return new ASTOption(
                (string)this.Value.Clone(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Annotations.ToList()));
        }
    }
}
