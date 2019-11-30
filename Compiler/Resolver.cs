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

        public (IEnumerable<ASTError>, IEnumerable<IASTNode>) Resolve()
        {
            var errors = new List<ASTError>();
            var nodes = new List<IASTNode>();
            foreach (IASTNode node in ParseTree)
            {
                if (node is ASTData)
                {
                    foreach (var option in ((ASTData)node).Options)
                    {
                        var existingNode = FindNode(option.Name);
                        if (existingNode is null)
                        {
                            nodes.Add(new ASTType(
                                option.Name,
                                option.Parameters,
                                Enumerable.Empty<ASTTypeField>(),
                                Enumerable.Empty<ASTAnnotation>(),
                                Enumerable.Empty<ASTDirective>())
                                );
                        }
                    }
                    nodes.Add(node);
                }
                else if (node is ASTAlias alias)
                {
                    ResolveAlias(alias, nodes, errors);
                }
                else if (node is ASTType t)
                {
                    ResolveType(t, nodes, errors);
                }
                else
                {
                    nodes.Add(node);
                }

            }
            return (errors, nodes);
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

        private ASTTypeField PluckField(ASTPluckedField field, List<ASTError> errors)
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
            } else
            {
                errors.Add(new ASTError("No valid field to pluck from.", "Invalid Syntax", null));
            }

            return field;
        }

        private void ResolveAlias(ASTAlias alias, List<IASTNode> nodes, List<ASTError> errors)
        {
            // here we'll resolve generic aliasses
            var _mod = alias.Type.First().Value;
            if (_mod == "List" || _mod == "Maybe" || alias.Type.Count() == 1)
            {
                // non generic type...
                // we do not allow generic Lists or Maybes like:
                //
                // alias ConcreteFoo = List Foo String
                //
                nodes.Add(alias);
            }
            else
            {
                var clone = (FindNode(_mod) as ASTType)?.Clone();
                if (clone is null)
                {
                    errors.Add(new ASTError("Cannot resolve generic type", "Invalid Syntax", null));
                }
                else if (clone.Parameters.Count() != alias.Type.Count() - 1)
                {
                    errors.Add(new ASTError("Not resolving all generic parameters.", "Invalid Syntax", null));
                }
                else
                {
                    clone.Name = alias.Name;
                    clone.Fields.ToList().ForEach(field =>
                    {
                        field.Type = field.Type.Select(t =>
                        {
                            if (t.IsGeneric)
                            {
                                var index = clone.Parameters.ToList().IndexOf(t.Value) + 1;
                                return alias.Type.ElementAt(index);
                            }
                            else return t;
                        }).ToList();
                    });
                    nodes.Add(clone);
                }
            }
        }

        private void ResolveType(ASTType t, List<IASTNode> nodes, List<ASTError> errors)
        {
            t.Extensions.ToList().ForEach(e =>
            {
                if (!(FindNode(e) is ASTType extendedFrom))
                {
                    // TODO: Handle error gracefully.
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
                    ASTPluckedField field => PluckField(field, errors),
                    ASTTypeField field => field,
                    _ => throw new InvalidTokenException("Not a field type")
                };
            });
            nodes.Add(t);
        }
    }
}
