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
                yield return node;
            }
        }

        private IASTNode FindNode(string name)
        {
            return ParseTree.FirstOrDefault(n => {
                return n is INamable && (n as INamable).Name == name;
            });
        }
    }
}
