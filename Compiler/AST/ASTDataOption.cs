using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTDataOption : IASTNode, ICloneable
    {
        public string Name { get; set; } = "";
        public IEnumerable<ASTAnnotation> Annotations { get; set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<string> Parameters { get; set; } = Enumerable.Empty<string>();
        public ASTDataOption() { }
        public static IEnumerable<ASTDataOption> Parse(IParser parser)
        {
            var annotations = ASTAnnotation.Parse(parser);
            parser.TryConsume(TokenType.Or, out Token? t);
            while (!(t is null))
            {

                var id = parser.Consume(TokenType.Identifier);
                var parameters = parser.ConsumeWhile(TokenType.GenericParameter);

                ASTDataOption r = new ASTDataOption();
                r.Name = id.Value;
                r.Parameters = parameters.Select(p => p.Value).ToList();
                r.Annotations = annotations;

                yield return r;

                annotations = ASTAnnotation.Parse(parser);
                parser.TryConsume(TokenType.Or, out t);
            }
        }

        public object Clone()
        {
            return new ASTDataOption
            {
                Name = (string)this.Name.Clone(),
                Parameters = ObjectCloner.CloneList(this.Parameters.ToList()),
                Annotations = ObjectCloner.CloneList(this.Annotations.ToList()),
            };
        }
    }
}
