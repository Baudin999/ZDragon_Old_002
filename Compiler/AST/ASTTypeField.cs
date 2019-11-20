using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTTypeField : IASTNode, IRestrictable, IElement, INamable, ICloneable
    {
        public string Name { get; set; } = "";
        public IEnumerable<ASTTypeDefinition> Type { get; set; } = Enumerable.Empty<ASTTypeDefinition>();
        public IEnumerable<ASTAnnotation> Annotations { get; set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<ASTRestriction> Restrictions { get; set; } = Enumerable.Empty<ASTRestriction>();

        public ASTTypeField() { }
        public static ASTTypeField Parse(IParser parser)
        {
            var result = new ASTTypeField();
            result.Restrictions = new List<ASTRestriction>();
            result.Annotations = ASTAnnotation.Parse(parser).ToList();
            result.Name = parser.Consume(TokenType.Identifier).Value;
            Token Separator = parser.Consume(TokenType.Separator);
            result.Type = ASTTypeDefinition.ParseType(parser).ToList();
            result.Restrictions = ASTRestriction.CreateRestrictions(parser, TokenType.KW_Type).ToList();
            parser.Consume(TokenType.EndStatement);
            return result;
        }

        public object Clone()
        {
            return new ASTTypeField
            {
                  Name = this.Name,
                  Type = this.Type.Select(t => new ASTTypeDefinition(t.Value)),
                  Annotations = ObjectCopier.CopyList<ASTAnnotation>(this.Annotations.ToList()),
                  Restrictions = ObjectCopier.CopyList<ASTRestriction>(this.Restrictions.ToList())
            };
        }
    }
}
