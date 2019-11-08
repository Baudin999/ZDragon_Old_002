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
                            throw new System.Exception($"Cannot find type {e} to extend from");
                        }

                        var clones = extendedFrom.Fields.Select(f => f.Clone()).ToList();
                        t.AddFields(clones);
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
    }
}
