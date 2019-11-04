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
                }
                yield return node;
            }
        }

        private IASTNode FindNode(string name)
        {
            return ParseTree.FirstOrDefault(n =>
            {
                return n is INamable && (n as INamable).Name == name;
            });
        }
    }
}
