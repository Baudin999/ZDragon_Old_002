using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTData : IASTNode, INamable
    {
        public string Name { get; private set; } = "";
        public IEnumerable<string> Parameters { get; private set; } = Enumerable.Empty<string>();
        public IEnumerable<ASTAnnotation> Annotations { get; private set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<ASTDirective> Directives { get; private set; } = Enumerable.Empty<ASTDirective>();
        public IEnumerable<ASTDataOption> Options { get; private set; } = Enumerable.Empty<ASTDataOption>();

        public static (List<ASTError>, ASTData) Parse(
                IParser parser,
                IEnumerable<ASTAnnotation> annotations,
                IEnumerable<ASTDirective> directives)
        {
            List<ASTError> errors = new List<ASTError>();
            ASTData result = new ASTData
            {
                Annotations = annotations,
                Directives = directives
            };

            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            result.Name = nameId.Value;
            result.Parameters = parser.ConsumeWhile(TokenType.GenericParameter).Select(p => p.Value).ToList();
            parser.Consume(TokenType.Equal);

            result.Options = ASTDataOption.Parse(parser).ToList();
            
            parser.Consume(TokenType.ContextEnded);

            return (errors, result);
        }

        
    }
}
