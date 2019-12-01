using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.AST
{
    public class ASTTypeField : IASTNode, IRestrictable, IElement, INamable, ICloneable
    {
        public string Name { get; }
        public string Module { get;  }
        public IEnumerable<ASTAnnotation> Annotations { get; }
        public IEnumerable<ASTDirective> Directives { get; }

        public IEnumerable<ASTTypeDefinition> Type { get; }
        public IEnumerable<ASTRestriction> Restrictions { get; private set; }

        public ASTTypeField(
            string name,
            string module,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives,
            IEnumerable<ASTTypeDefinition> types,
            IEnumerable<ASTRestriction> restrictions
            ) {
            this.Name = name;
            this.Module = module;
            this.Annotations = annotations;
            this.Directives = directives;
            this.Type = types;
            this.Restrictions = restrictions;
        }

        public static (IEnumerable<IASTError>, ASTTypeField) Parse(IParser parser, string module = "")
        {
            var (d_errors, directives) = ASTDirective.Parse(parser);
            var annotations = ASTAnnotation.Parse(parser).ToList();

            var pluck = parser.TryConsume(TokenType.KW_Pluck);
            var name = "";
            if (pluck is null)
            {
                name = parser.Consume(TokenType.Identifier).Value;
                Token Separator = parser.Consume(TokenType.Separator);
            }

            var types = ASTTypeDefinition.Parse(parser, module);
            var restrictions = ASTRestriction.CreateRestrictions(parser, TokenType.KW_Type).ToList();
            parser.Consume(TokenType.EndStatement);


            if (pluck is null)
            {
                return (d_errors, new ASTTypeField(
                    name,
                    module,
                    annotations,
                    directives,
                    types,
                    restrictions
                    ));
            } else
            {
                return (d_errors, new ASTPluckedField(
                    name,
                    module,
                    annotations,
                    directives,
                    types,
                    restrictions
                    ));
            }
        }

        public object Clone()
        {
            return new ASTTypeField(
                (string)this.Name.Clone(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Annotations.ToList()),
                ObjectCloner.CloneList(this.Directives.ToList()),
                ObjectCloner.CloneList(this.Type.ToList()),
                ObjectCloner.CloneList(this.Restrictions.ToList())
                );
        }

        public void SetRestriction(string key, string value, ASTRestriction original)
        {
            var originalRestriction = Restrictions.FirstOrDefault(r => r.Key == key);
            if (!(originalRestriction is null))
            {
                originalRestriction.ChangeValue(value);
            }
            else
            {
                Restrictions = Restrictions.Concat(new ASTRestriction[] { original });
            }
        }
    }

    public class ASTPluckedField : ASTTypeField
    {
        public ASTPluckedField(
            string name,
            string module,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives,
            IEnumerable<ASTTypeDefinition> types,
            IEnumerable<ASTRestriction> restrictions) : base(name, module, annotations, directives, types, restrictions)
        {
        }
    }
}
