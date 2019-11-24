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

            var pluck = parser.TryConsume(TokenType.KW_Pluck);
            if (!(pluck is null))
            {
                result = new ASTPluckedField();//.Parse(parser);
            }
            else
            {
                result.Name = parser.Consume(TokenType.Identifier).Value;
                Token Separator = parser.Consume(TokenType.Separator);
            }

            result.Type = ASTTypeDefinition.Parse(parser).ToList();
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
                Annotations = ObjectCloner.CloneList(this.Annotations.ToList()),
                Restrictions = ObjectCloner.CloneList(this.Restrictions.ToList())
            };
        }

        public void SetRestriction(string key, string value, ASTRestriction original)
        {
            var originalRestriction = Restrictions.FirstOrDefault(r => r.Key == key);
            if (!(originalRestriction is null))
            {
                originalRestriction.Value = value;
            }
            else
            {
                Restrictions = Restrictions.Concat(new ASTRestriction[] { original });
            }
        }
    }

    public class ASTPluckedField : ASTTypeField { }
}
