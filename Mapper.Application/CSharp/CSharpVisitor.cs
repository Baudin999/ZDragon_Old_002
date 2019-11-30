using System;
using System.Collections.Generic;
using Compiler;
using Compiler.AST;

namespace Mapper.Application.CSharp
{
    public class CSharpVisitor : DefaultVisitor<string>
    {
        private readonly string Namespace;
        public CSharpVisitor(IEnumerable<IASTNode> nodeTree) : base(nodeTree) { }
        public CSharpVisitor(IEnumerable<IASTNode> nodeTree, string _namespace) : base(nodeTree) {
            Namespace = _namespace;
        }

        public override string VisitASTType(ASTType astType)
        {
            return $@"";
        }
    }
}
