using System;
using System.Collections.Generic;
using Compiler;
using Compiler.AST;

namespace Mapper.Application.CSharp
{
    public class CSharpVisitor : VisitorDefault<string>
    {
        private readonly string Namespace;
        public CSharpVisitor(ASTGenerator generator) : base(generator) { }
        public CSharpVisitor(ASTGenerator generator, string _namespace) : base(generator) {
            Namespace = _namespace;
        }

        public override string VisitASTType(ASTType astType)
        {
            return $@"";
        }
    }
}
