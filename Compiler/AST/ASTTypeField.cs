using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Compiler.AST
{
    public class ASTTypeField : IASTNode, IRestrictable, IElement, INamable, ICloneable, IEqualityComparer<ASTTypeField>
    {
        public string Name { get; }
        public string Module { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }
        public IEnumerable<ASTDirective> Directives { get; }

        public IEnumerable<ASTTypeDefinition> Type { get; }
        public IEnumerable<ASTRestriction> Restrictions { get; private set; }
        public FieldOrigin Origin { get; }

        public ASTTypeField(
            string name,
            string module,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives,
            IEnumerable<ASTTypeDefinition> types,
            IEnumerable<ASTRestriction> restrictions,
            FieldOrigin origin = FieldOrigin.Original
            )
        {
            this.Name = name;
            this.Module = module;
            this.Annotations = annotations;
            this.Directives = directives;
            this.Type = types;
            this.Restrictions = restrictions;
            this.Origin = origin;
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
                parser.Consume(TokenType.Separator);
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
                    restrictions,
                    FieldOrigin.Original
                    ));
            }
            else
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

        public object Clone(FieldOrigin origin)
        {
            if (this is ASTPluckedField)
            {
                return new ASTPluckedField(
                (string)this.Name.Clone(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Annotations.ToList()),
                ObjectCloner.CloneList(this.Directives.ToList()),
                ObjectCloner.CloneList(this.Type.ToList()),
                ObjectCloner.CloneList(this.Restrictions.ToList()),
                origin
                );
            }
            else
            {
                return new ASTTypeField(
                    (string)this.Name.Clone(),
                    (string)this.Module.Clone(),
                    ObjectCloner.CloneList(this.Annotations.ToList()),
                    ObjectCloner.CloneList(this.Directives.ToList()),
                    ObjectCloner.CloneList(this.Type.ToList()),
                    ObjectCloner.CloneList(this.Restrictions.ToList()),
                    origin
                    );
            }
        }
        public object Clone()
        {
            if (this is ASTPluckedField)
            {
                return new ASTPluckedField(
                (string)this.Name.Clone(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Annotations.ToList()),
                ObjectCloner.CloneList(this.Directives.ToList()),
                ObjectCloner.CloneList(this.Type.ToList()),
                ObjectCloner.CloneList(this.Restrictions.ToList()),
                this.Origin
                );
            }
            else
            {
                return new ASTTypeField(
                    (string)this.Name.Clone(),
                    (string)this.Module.Clone(),
                    ObjectCloner.CloneList(this.Annotations.ToList()),
                    ObjectCloner.CloneList(this.Directives.ToList()),
                    ObjectCloner.CloneList(this.Type.ToList()),
                    ObjectCloner.CloneList(this.Restrictions.ToList()),
                    this.Origin
                    );
            }
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

        public bool Equals([AllowNull] ASTTypeField x, [AllowNull] ASTTypeField y) => x.Name == y.Name;
        public int GetHashCode([DisallowNull] ASTTypeField obj) => ((object)obj).GetHashCode();

        public override string ToString() => $"{Name}: {String.Join(" ", Type)};";
    }

    public class ASTPluckedField : ASTTypeField
    {
        public ASTPluckedField(
            string name,
            string module,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives,
            IEnumerable<ASTTypeDefinition> types,
            IEnumerable<ASTRestriction> restrictions,
            FieldOrigin origin = FieldOrigin.Original) : base(name, module, annotations, directives, types, restrictions, origin)
        {
        }
    }
}
