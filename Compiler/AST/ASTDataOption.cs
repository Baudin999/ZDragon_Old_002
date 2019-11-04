using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTDataOption : IASTNode
    {
        public string Name { get; private set; } = "";
        public IEnumerable<ASTAnnotation> Annotations { get; private set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<string> Parameters { get; private set; } = Enumerable.Empty<string>();

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
    }
}
