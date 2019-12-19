using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Compiler.AST
{
    public class ASTTypeField : IASTNode, IRestrictable, IElement, ITypeble, INamable, ICloneable, IEqualityComparer<ASTTypeField>
    {
        public Token? Token { get; } = Token.Empty();
        public ASTName ASTName { get; }
        public string Name { get { return this.ASTName.Name; } }
        public string Module { get; }
        public IEnumerable<ASTAnnotation> Annotations { get; }
        public IEnumerable<ASTDirective> Directives { get; }

        public IEnumerable<ASTTypeDefinition> Types { get; }
        public IEnumerable<ASTRestriction> Restrictions { get; private set; }
        public FieldOrigin Origin { get; }

        public ASTTypeField(
            ASTName name,
            string module,
            IEnumerable<ASTAnnotation> annotations,
            IEnumerable<ASTDirective> directives,
            IEnumerable<ASTTypeDefinition> types,
            IEnumerable<ASTRestriction> restrictions,
            FieldOrigin origin = FieldOrigin.Original
            )
        {
            this.ASTName = name;
            this.Module = module;
            this.Annotations = annotations;
            this.Directives = directives;
            this.Types = types;
            this.Restrictions = restrictions;
            this.Origin = origin;
        }

        public static (IEnumerable<IASTError>, ASTTypeField) Parse(IParser parser, string module = "")
        {
            var (d_errors, directives) = ASTDirective.Parse(parser);
            var annotations = ASTAnnotation.Parse(parser).ToList();

            var pluck = parser.TryConsume(TokenType.KW_Pluck);
            
            if (pluck is null)
            {
                var name = ASTName.Parse(parser);
                parser.Consume(TokenType.Separator);
                var types = ASTTypeDefinition.Parse(parser, module);
                var restrictions = ASTRestriction.CreateRestrictions(parser, TokenType.KW_Type).ToList();
                parser.Consume(TokenType.EndStatement);
                return (d_errors, new ASTTypeField(
                    name,
                    module,
                    annotations,
                    directives,
                    types,
                    restrictions,
                    FieldOrigin.Original
                    ));
            } else
            {
                var types = ASTTypeDefinition.Parse(parser, module);
                var restrictions = ASTRestriction.CreateRestrictions(parser, TokenType.KW_Type).ToList();
                parser.Consume(TokenType.EndStatement);
                return (d_errors, new ASTPluckedField(
                    new ASTName("", pluck),
                    module,
                    annotations,
                    directives,
                    types,
                    restrictions
                    ));
            }
        }

        public string? OfType(bool nullIfBaseType = true)
        {
            string _type;
            if (this is ASTPluckedField)
            {
                _type = this.Types.First().Value;
            }
            else
            {
                _type = this.Types.Last().Value;
            }


            if (nullIfBaseType)
            {
                return Parser.BaseTypes.Contains(_type) ? null : _type;
            }
            else
            {
                return _type;
            }
        }

        public object Clone(FieldOrigin origin)
        {
            if (this is ASTPluckedField)
            {
                return new ASTPluckedField(
                this.ASTName.Clone<ASTName>(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Annotations.ToList()),
                ObjectCloner.CloneList(this.Directives.ToList()),
                ObjectCloner.CloneList(this.Types.ToList()),
                ObjectCloner.CloneList(this.Restrictions.ToList()),
                origin
                );
            }
            else
            {
                return new ASTTypeField(
                    this.ASTName.Clone<ASTName>(),
                    (string)this.Module.Clone(),
                    ObjectCloner.CloneList(this.Annotations.ToList()),
                    ObjectCloner.CloneList(this.Directives.ToList()),
                    ObjectCloner.CloneList(this.Types.ToList()),
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
                this.ASTName.Clone<ASTName>(),
                (string)this.Module.Clone(),
                ObjectCloner.CloneList(this.Annotations.ToList()),
                ObjectCloner.CloneList(this.Directives.ToList()),
                ObjectCloner.CloneList(this.Types.ToList()),
                ObjectCloner.CloneList(this.Restrictions.ToList()),
                this.Origin
                );
            }
            else
            {
                return new ASTTypeField(
                    this.ASTName.Clone<ASTName>(),
                    (string)this.Module.Clone(),
                    ObjectCloner.CloneList(this.Annotations.ToList()),
                    ObjectCloner.CloneList(this.Directives.ToList()),
                    ObjectCloner.CloneList(this.Types.ToList()),
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
        public int GetHashCode([DisallowNull] ASTTypeField obj) => obj.GetHashCode();

        public override string ToString() => $"{Name}: {String.Join(" ", Types)};";
    }

    public class ASTPluckedField : ASTTypeField
    {
        public ASTPluckedField(
            ASTName name,
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
