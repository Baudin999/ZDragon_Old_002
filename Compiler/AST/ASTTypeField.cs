using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTTypeField : IASTNode, IRestrictable, IElement, INamable
    {
        public string Name { get; private set; } = "";
        public IEnumerable<ASTTypeDefinition> Type { get; private set; } = Enumerable.Empty<ASTTypeDefinition>();
        public IEnumerable<ASTAnnotation> Annotations { get; private set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<ASTRestriction> Restrictions { get; private set; } = Enumerable.Empty<ASTRestriction>();

        public ASTTypeField() { }
        public ASTTypeField(IParser parser)
        {
            this.Restrictions = new List<ASTRestriction>();
            this.Annotations = ASTAnnotation.Parse(parser).ToList();
            this.Name = parser.Consume(TokenType.Identifier).Value;
            Token Separator = parser.Consume(TokenType.Separator);
            this.Type = ASTTypeDefinition.ParseType(parser).ToList();
            this.Restrictions = ASTRestriction.CreateRestrictions(parser, TokenType.KW_Type).ToList();
            parser.Consume(TokenType.EndStatement);
        }

        public ASTTypeField Clone()
        {
            return new ASTTypeField
            {
                  Name = this.Name,
                  Type = this.Type.Select(t => new ASTTypeDefinition(t.Value)),
                  Annotations = this.Annotations.Select(a => new ASTAnnotation(a.Value)),
                  Restrictions = this.Restrictions.Select(r => r.Clone())
            };
        }
    }
}
