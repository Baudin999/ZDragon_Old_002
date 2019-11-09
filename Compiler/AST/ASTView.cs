using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTView : IASTNode, INamable
    {
        public string Name { get; private set; } = "";
        public IEnumerable<string> Nodes { get; private set; } = Enumerable.Empty<string>();
        public IEnumerable<ASTAnnotation> Annotations { get; private set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<ASTDirective> Directives { get; private set; } = Enumerable.Empty<ASTDirective>();
        
        public static (List<ASTError>, ASTView) Parse(
                IParser parser,
                IEnumerable<ASTAnnotation> annotations,
                IEnumerable<ASTDirective> directives)
        {
            List<ASTError> errors = new List<ASTError>();
            ASTView result = new ASTView
            {
                Annotations = annotations,
                Directives = directives
            };

            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            result.Name = nameId.Value;
            parser.Consume(TokenType.Equal);
            result.Nodes = parser.ConsumeWhile(TokenType.Identifier).Select(p => p.Value).ToList();
            parser.Consume(TokenType.ContextEnded);

            return (errors, result);
        }

        
    }
}
