using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Compiler.AST
{
    public class ASTAlias : IASTNode, IRestrictable, IElement, INamable, IRootNode, ICloneable
    {
        public string Name { get; set; } = "";
        public IEnumerable<ASTTypeDefinition> Type { get; set; } = Enumerable.Empty<ASTTypeDefinition>();
        public IEnumerable<ASTRestriction> Restrictions { get; set; } = Enumerable.Empty<ASTRestriction>();
        public IEnumerable<ASTAnnotation> Annotations { get; set; } = Enumerable.Empty<ASTAnnotation>();
        public IEnumerable<ASTDirective> Directives { get; set; } = Enumerable.Empty<ASTDirective>();

        public ASTAlias() { }

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

        public object Clone()
        {
            return new ASTAlias
            {
                Name = (string)this.Name.Clone(),
                Type = ObjectCopier.CopyList<ASTTypeDefinition>(this.Type.ToList()),
                Restrictions = ObjectCopier.CopyList<ASTRestriction>(this.Restrictions.ToList()),
                Annotations = ObjectCopier.CopyList<ASTAnnotation>(this.Annotations.ToList()),
                Directives = ObjectCopier.CopyList<ASTDirective>(this.Directives.ToList())
            };
        }
    }
}
