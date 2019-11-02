using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTAlias : IASTNode
    {
        public string Name { get; private set; } = "";
        public IEnumerable<ASTTypeDefinition> Type { get; private set; } = Enumerable.Empty<ASTTypeDefinition>();
        public IEnumerable<ASTRestriction> Restrictions { get; private set; } = Enumerable.Empty<ASTRestriction>();
        public IEnumerable<ASTAnnotation> Annotations { get; set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<ASTDirective> Directives { get; set; } = Enumerable.Empty<ASTDirective>();

        public ASTAlias() {
            
        }

        public static (List<ASTError>, ASTAlias) Parse(
                IParser parser,
                IEnumerable<ASTAnnotation> annotations,
                IEnumerable<ASTDirective> directives)
        {
            List<ASTError> errors = new List<ASTError>();
            ASTAlias result = new ASTAlias
            {
                Annotations = annotations,
                Directives = directives
            };

            if (parser.HasNext()) parser.Next();
            var nameId = parser.Consume(TokenType.Identifier);
            result.Name = nameId.Value;
            parser.Consume(TokenType.Equal);
            result.Type = ASTTypeDefinition.ParseType(parser).ToList();
            result.Restrictions = ASTRestriction.CreateRestrictions(parser, TokenType.KW_Alias).ToList();
            parser.TryConsume(TokenType.EndStatement);
            parser.Consume(TokenType.ContextEnded);

            return (errors, result);
        }

    }
}
