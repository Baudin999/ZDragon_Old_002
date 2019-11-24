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
            var errors = new List<ASTError>();
            foreach (IASTNode node in ParseTree)
            {
                if (node is ASTData)
                {
                    foreach (var option in ((ASTData)node).Options)
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
                else if (node is ASTAlias alias)
                {
                    // here we'll resolve generic aliasses
                    var _mod = alias.Type.First().Value;
                    if (_mod == "List" || _mod == "Maybe" || alias.Type.Count() == 1)
                    {
                        yield return alias;
                    } else
                    {
                        // it's a generic type!
                        var clone = (FindNode(_mod) as ASTType)?.Clone();
                        if (clone is null)
                        {
                            errors.Add(new ASTError("Cannot resolve generic type", null));
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
                            yield return clone;
                        }
                    }
                }
                else if (node is ASTType t)
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
