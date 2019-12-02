using System;
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

        public (IEnumerable<IASTError>, IEnumerable<IASTNode>) Resolve()
        {
            var errors = new List<IASTError>();
            var nodes = new List<IASTNode>();
            foreach (var node in ParseTree)
            {
                if (node is ASTData dataNode)
                {
                    foreach (var option in dataNode.Options)
                    {
                        var existingNode = FindNode(option.Name);
                        if (existingNode is null)
                        {
                            nodes.Add(new ASTType(
                                option.Name,
                                dataNode.Module,
                                option.Parameters,
                                Enumerable.Empty<string>(),
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
                    var (resolve_alias_errors, resolvedAlias) = ResolveAlias(alias);
                    errors.AddRange(resolve_alias_errors);
                    nodes.Add(resolvedAlias);
                }
                else if (node is ASTType t)
                {
                    var (resolve_errors, resolvedType) = ResolveType(t);
                    errors.AddRange(resolve_errors);
                    nodes.Add(resolvedType);
                }
                else
                {
                    nodes.Add(node);
                }

            }
            return (errors, nodes.ToList());
        }

        private IASTNode FindNode(string name)
        {
            return ParseTree.FirstOrDefault(n =>
            {
                // Oh my dotnet, what have you done!!
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                return n != null && !(n is ASTImport) && n is INamable && (n as INamable).Name == name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            });
        }

        private (IEnumerable<IASTError>, ASTTypeField) PluckField(ASTPluckedField field)
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

                return (Enumerable.Empty<ASTError>(), clone);
            } else
            {
                var error = new ASTError("No valid field to pluck from.", "Invalid Syntax");
                return (new[] { error }, field);
            }
        }

        private (IEnumerable<IASTError>, IASTNode) ResolveAlias(ASTAlias alias)
        {
            // here we'll resolve generic aliasses
            var _mod = alias.Type.First().Value;
            if (_mod == "List" || _mod == "Maybe" || Parser.BaseTypes.Contains(_mod))
            {
                // non generic type...
                // we do not allow generic Lists or Maybes like:
                //
                // alias ConcreteFoo = List Foo String
                //
                return (Enumerable.Empty<IASTError>(), alias);
            }
            else if (!Parser.BaseTypes.Contains(_mod) && alias.Type.Count() == 1)
            {
                // Clearly we're in a case like:
                // alias Foo = Bar;
                // let's create a new type cloned from the old one...
                var errors = new List<IASTError>();
                var source = FindNode(_mod) as ASTType;
                if (source is null)
                {
                    errors.Add(new ASTError($"Cannot find type {_mod} to rename. You can only alias existing types.", "Invalid Syntax"));
                    return (errors, alias);
                }
                else
                {
                    var newType = new ASTType(
                        alias.Name,
                        alias.Module,
                        Enumerable.Empty<string>(),
                        Enumerable.Empty<string>(),
                        ObjectCloner.CloneList(source.Fields),
                        ObjectCloner.CloneList(alias.Annotations),
                        ObjectCloner.CloneList(alias.Directives)
                        );
                    return (errors, newType);
                }

            }
            else if (alias.Type.Count() > 1)
            {
                var errors = new List<IASTError>();
                var clone = FindNode(_mod) as ASTType;
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
                    var resolvedFields = clone.Fields.Select(field =>
                    {
                        var fieldTypes = field.Type.Select(t =>
                        {
                            if (t.IsGeneric)
                            {
                                var index = clone.Parameters.ToList().IndexOf(t.Value) + 1;
                                return (ASTTypeDefinition)alias.Type.ElementAt(index).Clone();
                            }
                            return (ASTTypeDefinition)t.Clone();
                        });

                        return new ASTTypeField(
                            (string)field.Name.Clone(),
                            (string)field.Module.Clone(),
                            ObjectCloner.CloneList(field.Annotations),
                            ObjectCloner.CloneList(field.Directives),
                            fieldTypes,
                            ObjectCloner.CloneList(field.Restrictions)
                            );
                    });

                    var newType = new ASTType(
                        (string)alias.Name.Clone(),
                        (string)alias.Module.Clone(),
                        ObjectCloner.CloneList(clone.Parameters),
                        ObjectCloner.CloneList(clone.Extensions),
                        resolvedFields,//ObjectCloner.CloneList(clone.Fields),
                        ObjectCloner.CloneList(alias.Annotations),
                        ObjectCloner.CloneList(alias.Directives)
                        );

                    return (errors, newType);
                }

                return (errors, alias);
            } else
            {
                return (Enumerable.Empty<IASTError>(), alias);
            }
        }

        private (IEnumerable<IASTError>, ASTType) ResolveType(ASTType t)
        {
            var errors = new List<IASTError>();
            var extended_fields = t.Extensions.SelectMany(e =>
            {
                var t = FindNode(e) as ASTType;
                if (t is null) return Enumerable.Empty<ASTTypeField>();
                return ObjectCloner.CloneList(t.Fields);
            });

            var fields = t.Fields.Select(f =>
            {
                if (f is ASTPluckedField field)
                {
                    var (errors, pluckedField) = PluckField(field);
                    errors = errors.Concat(errors);
                    return pluckedField;
                }
                return f;
            });

            return (errors, t.Clone(extended_fields.Concat(fields)));
        }
    }
}
