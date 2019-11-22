using System.Linq;
using System.Collections.Generic;
using Compiler.AST;

namespace Compiler
{
    public class Resolver
    {
        public IEnumerable<IASTNode> ParseTree { get; }

        public Resolver(IEnumerable<IASTNode> parseTree)
        {
            this.ParseTree = parseTree;
        }

        public IEnumerable<IASTNode> Resolve()
        {
            List<ASTError> errors = new List<ASTError>();
            foreach (IASTNode node in ParseTree)
            {
                if (node is ASTData)
                {
                    foreach (ASTDataOption option in ((ASTData)node).Options)
                    {
                        var existingNode = FindNode(option.Name);
                        if (existingNode is null)
                        {
                            yield return new ASTType(
                                option.Name,
                                option.Parameters,
                                Enumerable.Empty<ASTTypeField>(),
                                Enumerable.Empty<ASTAnnotation>(),
                                Enumerable.Empty<ASTDirective>());
                        }
                    }
                    yield return node;
                }
                else if (node is ASTType)
                {
                    ASTType t = (ASTType)node;
                    t.Extensions.ToList().ForEach(e =>
                    {
                        var extendedFrom = FindNode(e) as ASTType;
                        if (extendedFrom is null)
                        {
                            //throw new System.Exception($"Cannot find type {e} to extend from");
                        }
                        else
                        {
                            var clones = extendedFrom.Fields.Select(f => (ASTTypeField)f.Clone()).ToList();
                            t.AddFields(clones);
                        }
                    });

                    t.Fields = t.Fields.ToList().Select(f =>
                    {
                        return f switch
                        {
                            ASTPluckedField field => PluckField(field),
                            ASTTypeField field => field,
                            _ => throw new InvalidTokenException("Not a field type")
                        };
                    });
                    yield return t;
                }
                else
                {
                    yield return node;
                }
            }
        }

        private IASTNode FindNode(string name)
        {
            return ParseTree.FirstOrDefault(n =>
            {
                // Oh my dotnet, what have you done!!
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                return n != null && n is INamable && (n as INamable).Name == name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            });
        }

        private ASTTypeField PluckField(ASTPluckedField field)
        {

            var _ref = field.Type.First();
            var _field = field.Type.Last();
            var referencedNode = FindNode(_ref.Value);
            var referencedField = (referencedNode as ASTType)?.Fields.FirstOrDefault(_f => _f.Name == _field.Value);

            if (referencedField != null)
            {
                var clone = (ASTTypeField)referencedField.Clone();
                field.Restrictions.ToList().ForEach(r =>
                {
                    clone.SetRestriction(r.Key, r.Value, r);
                });

                return clone;
            }

            return field;

            //throw new InvalidTokenException("Not a referenced field to pluck from.");
        }
    }
}
